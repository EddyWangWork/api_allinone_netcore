using Allinone.DLL.Data;
using Allinone.Domain.DS.Transactions;
using Allinone.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IDSTransactionRepository
    {
        Task<DSYearExpenses> GetDSYearExpensesAsync(int year, int memberId);
        Task<IEnumerable<DSYearCreditDebitDiff>> GetDSYearCreditDebitDiffAsync(int year, int memberId);
        Task<IEnumerable<DSDebitStat>> GetDSMonthlyExpensesAsync(int year, int month, int memberId);

        Task<IEnumerable<DSTransactionDto>> GetAllByMemberIdAsync(int memberId);
        Task<DSTransaction?> GetByTransferOutIdAsync(int id);
        Task<DSTransaction?> GetAsync(int id);
        Task<bool> IsExistByMember(int memberid, int id);
        Task<IEnumerable<DSTransaction>> GetAllAsync();

        // Add/Update/Delete only track changes; callers commit through IUnitOfWork.SaveAsync.
        Task AddAsync(DSTransaction entity);
        void Update(DSTransaction entity);
        void Delete(DSTransaction entity);
    }

    public class DSTransactionRepository(DSContext context) : IDSTransactionRepository
    {
        private static readonly List<int> _transferTypes = [(int)EnumDSTranType.TransferOut, (int)EnumDSTranType.TransferIn];
        private static readonly List<int> _creditItems = [19];

        public async Task<DSYearExpenses> GetDSYearExpensesAsync(int year, int memberId)
        {
            var res = await context.DSTransaction
                .AsNoTracking()
                .Where(a => a.MemberID == memberId
                    && a.DSTypeID == (int)EnumDSTranType.Expense
                    && a.CreatedDateTime.Year == year)
                .Select(a => new
                {
                    DSItemName = a.DSItem != null ? a.DSItem.Name : a.DSItemSub!.DSItem.Name,
                    DSYearMonthOri = a.CreatedDateTime,
                    DSYearMonth = $"{a.CreatedDateTime.Month}",
                    a.Amount,
                })
                .ToListAsync();

            var resGroupby = res.GroupBy(x => new { x.DSYearMonth, x.DSItemName }).
                Select(y => new
                {
                    YearMonth = y.First().DSYearMonth,
                    ItemName = y.First().DSItemName,
                    Amount = y.Sum(x => x.Amount)
                }).ToList();

            var distYearMonths = res.OrderBy(x => x.DSYearMonthOri).DistinctBy(x => x.DSYearMonth).Select(x => x.DSYearMonth);
            var distItemNames = res.OrderBy(x => x.DSItemName).DistinctBy(x => x.DSItemName).Select(x => x.DSItemName);
            DSYearExpenses dsYearExpenses = new DSYearExpenses { DSItemNames = distItemNames.ToList() };

            foreach (var yearMonth in distYearMonths)
            {
                DSYearDetails dsYearDetails = new DSYearDetails() { YearMonth = yearMonth };

                foreach (var distItemName in distItemNames)
                {
                    var amount = resGroupby.FirstOrDefault(x => x.YearMonth == yearMonth && x.ItemName == distItemName)?.Amount;
                    dsYearDetails.Amount.Add(amount ?? 0);
                }

                dsYearExpenses.DSYearDetails.Add(dsYearDetails);
            }
            return dsYearExpenses;
        }

        public async Task<IEnumerable<DSYearCreditDebitDiff>> GetDSYearCreditDebitDiffAsync(int year, int memberId)
        {
            var res = await context.DSTransaction
                .AsNoTracking()
                .Where(a => a.MemberID == memberId
                    && a.CreatedDateTime.Year == year
                    && (a.DSTypeID == (int)EnumDSTranType.Expense
                        || (a.DSTypeID == (int)EnumDSTranType.Income
                            && _creditItems.Contains(a.DSItemID ?? a.DSItemSub!.DSItemID))))
                .Select(a => new
                {
                    a.DSTypeID,
                    DSYearMonthOri = a.CreatedDateTime,
                    DSYearMonth = $"{a.CreatedDateTime.Year}-{a.CreatedDateTime.Month}",
                    a.Amount,
                })
                .ToListAsync();

            var resGroupby = res.GroupBy(x => new { x.DSYearMonth, x.DSTypeID }).
                Select(y => new
                {
                    YearMonth = y.First().DSYearMonth,
                    Type = y.First().DSTypeID,
                    Amount = y.Sum(x => x.Amount)
                }).ToList();

            List<DSYearCreditDebitDiff> dsYearCreditDebitDiff = [];
            foreach (var yearMonth in res.OrderBy(x => x.DSYearMonthOri).DistinctBy(x => x.DSYearMonth).Select(x => x.DSYearMonth))
            {
                var credit = resGroupby.FirstOrDefault(x => x.YearMonth == yearMonth && x.Type == (int)EnumDSTranType.Income)?.Amount ?? 0;
                var debit = resGroupby.FirstOrDefault(x => x.YearMonth == yearMonth && x.Type == (int)EnumDSTranType.Expense)?.Amount ?? 0;

                dsYearCreditDebitDiff.Add(new DSYearCreditDebitDiff
                {
                    YearMonth = yearMonth,
                    Credit = credit,
                    Debit = debit,
                    Diff = credit - debit
                });
            }
            return dsYearCreditDebitDiff;
        }

        public async Task<IEnumerable<DSDebitStat>> GetDSMonthlyExpensesAsync(int year, int month, int memberId)
        {
            var res = await context.DSTransaction
                .AsNoTracking()
                .Where(a => a.MemberID == memberId
                    && a.DSTypeID == (int)EnumDSTranType.Expense
                    && a.CreatedDateTime.Year == year
                    && a.CreatedDateTime.Month == month)
                .Select(a => new DSDebitStat
                {
                    DSItemName = a.DSItem != null ? a.DSItem.Name : a.DSItemSub!.DSItem.Name,
                    Amount = a.Amount,
                })
                .ToListAsync();

            return res
                .GroupBy(x => x.DSItemName)
                .Select(y => new DSDebitStat { DSItemName = y.Key, Amount = y.Sum(x => x.Amount) })
                .OrderByDescending(x => x.Amount)
                .ToList();
        }

        public async Task<IEnumerable<DSTransactionDto>> GetAllByMemberIdAsync(int memberId)
        {
            var rows = await context.DSTransaction
                .AsNoTracking()
                .Where(a => a.MemberID == memberId)
                .Select(a => new
                {
                    a.ID,
                    a.DSTypeID,
                    TypeName = a.DSType!.Name,
                    a.DSAccountID,
                    AccountName = a.DSAccount!.Name,
                    a.DSItemID,
                    a.DSItemSubID,
                    SubItemParentID = a.DSItemSub != null ? (int?)a.DSItemSub.DSItemID : null,
                    a.TransferOutTransactionID,
                    ItemName = a.DSItem != null ? a.DSItem.Name : null,
                    SubItemName = a.DSItemSub != null ? a.DSItemSub.Name : null,
                    ParentItemName = a.DSItemSub != null ? a.DSItemSub.DSItem.Name : null,
                    TransferFromAccountName = a.TransferOutTransaction != null ? a.TransferOutTransaction.DSAccount!.Name : null,
                    a.Description,
                    a.Amount,
                    a.CreatedDateTime,
                })
                .ToListAsync();

            return rows.Select(a =>
            {
                var isTransfer = _transferTypes.Contains(a.DSTypeID);
                var hasItem = a.ItemName != null;

                return new DSTransactionDto
                {
                    ID = a.ID,
                    DSTypeID = a.DSTypeID,
                    DSTypeName = a.TypeName,
                    DSAccountID = a.DSAccountID,
                    DSAccountName = a.AccountName,
                    DSItemID = isTransfer ? 0 : a.DSItemID ?? a.SubItemParentID ?? 999,
                    DSItemSubID = a.DSItemSubID ?? 0,
                    TransferOutTransactionID = a.TransferOutTransactionID ?? 0,
                    DSItemName = a.DSTypeID == (int)EnumDSTranType.TransferIn
                        ? a.TransferFromAccountName!
                        : hasItem ? a.ItemName! : $"{a.ParentItemName}|{a.SubItemName}",
                    DSItemNameMain = hasItem ? a.ItemName! : a.ParentItemName!,
                    DSItemNameSub = hasItem ? a.ItemName! : a.SubItemName!,
                    Description = a.Description,
                    Amount = a.Amount,
                    CreatedDateTime = a.CreatedDateTime,
                    CreatedDateTimeYearMonth = new DateTime(a.CreatedDateTime.Year, a.CreatedDateTime.Month, 1),
                };
            }).ToList();
        }

        public async Task<DSTransaction?> GetByTransferOutIdAsync(int id) =>
            await context.DSTransaction.FirstOrDefaultAsync(x => x.TransferOutTransactionID == id);

        public async Task<DSTransaction?> GetAsync(int id) =>
            await context.DSTransaction.FindAsync(id);

        public async Task<bool> IsExistByMember(int memberid, int id) =>
            await context.DSTransaction.AnyAsync(x => x.MemberID == memberid && x.ID == id);

        public async Task<IEnumerable<DSTransaction>> GetAllAsync() =>
            await context.DSTransaction.ToListAsync();

        public async Task AddAsync(DSTransaction entity) =>
            await context.DSTransaction.AddAsync(entity);

        public void Update(DSTransaction entity) =>
            context.DSTransaction.Update(entity);

        public void Delete(DSTransaction entity) =>
            context.DSTransaction.Remove(entity);
    }
}
