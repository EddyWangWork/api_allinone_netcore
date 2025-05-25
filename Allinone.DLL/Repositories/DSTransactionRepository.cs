using Allinone.DLL.Data;
using Allinone.Domain.DS.Transactions;
using Allinone.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IDSTransactionRepository
    {
        Task<DSYearExpenses> GetDSYearExpensesAsync(int year);
        Task<IEnumerable<DSYearCreditDebitDiff>> GetDSYearCreditDebitDiffAsync(int year);
        Task<IEnumerable<DSDebitStat>> GetDSMonthlyExpensesAsync(int year, int month);

        Task<IEnumerable<DSTransactionDtoV2>> GetDSTransactionAsyncV3(int memberId);
        Task<IEnumerable<DSTransactionDtoV2>> GetDSTransactionByDSAccountAsync(int dsAccountID, int memberId);
        Task<IEnumerable<DSTransactionDto>> GetAllByMemberIdAsync(int memberId);
        Task<DSTransaction>? GetByTransferOutIdAsync(int id);
        Task<DSTransaction>? GetAsync(int id);
        Task<bool>? IsExistByMember(int memberid, int id);
        Task<IEnumerable<DSTransaction>> GetAllAsync();
        Task AddAsync(DSTransaction entity);
        void Update(DSTransaction entity);
        void Delete(DSTransaction entity);
    }

    public class DSTransactionRepository(DSContext context) : IDSTransactionRepository
    {
        private static readonly List<int> _transferTypes = [3, 4];
        private static readonly List<int> _creditItems = [19];
        private static readonly List<int> _debitItems = [2, 3, 31]; //commitment:1

        public async Task<DSYearExpenses> GetDSYearExpensesAsync(int year)
        {
            var responses = (
                 from a in context.DSTransaction
                 join b in context.DSItem on a.DSItemID equals b.ID into bb
                 from b2 in bb.DefaultIfEmpty()
                 join c in context.DSItemSub on a.DSItemSubID equals c.ID into cc
                 from c2 in cc.DefaultIfEmpty()
                 join d in context.DSItem on c2.DSItemID equals d.ID into dd
                 from d2 in dd.DefaultIfEmpty()
                 where
                    a.DSTypeID == (int)EnumDSTranType.Expense &&
                    (a.CreatedDateTime.Year == year)
                 select new
                 {
                     DSItemName = b2.ID > 0 ? b2.Name : $"{d2.Name}",
                     DSYearMonthOri = a.CreatedDateTime,
                     DSYearMonth = $"{a.CreatedDateTime.Month}",
                     //DSYearMonth = $"{a.CreatedDateTime.Year}-{a.CreatedDateTime.Month}",
                     Amount = a.Amount,
                 }).ToListAsync();

            var res = await responses;
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
        public async Task<IEnumerable<DSYearCreditDebitDiff>> GetDSYearCreditDebitDiffAsync(int year)
        {
            List<int> CreditDebitList = new List<int> { (int)EnumDSTranType.Income, (int)EnumDSTranType.Expense };

            var responses = (
                 from a in context.DSTransaction
                 join b in context.DSItem on a.DSItemID equals b.ID into bb
                 from b2 in bb.DefaultIfEmpty()
                 join c in context.DSItemSub on a.DSItemSubID equals c.ID into cc
                 from c2 in cc.DefaultIfEmpty()
                 join d in context.DSItem on c2.DSItemID equals d.ID into dd
                 from d2 in dd.DefaultIfEmpty()
                 where
                    (a.DSTypeID == (int)EnumDSTranType.Expense ||
                    (a.DSTypeID == (int)EnumDSTranType.Income && _creditItems.Contains(b2 != null ? b2.ID : c2.DSItemID))) &&
                    (a.CreatedDateTime.Year == year)
                 select new
                 {
                     DSItemName = b2.ID > 0 ? b2.Name : $"{d2.Name}",
                     a.DSTypeID,
                     DSYearMonthOri = a.CreatedDateTime,
                     DSYearMonth = $"{a.CreatedDateTime.Year}-{a.CreatedDateTime.Month}",
                     Amount = a.Amount,
                 }).ToListAsync();

            var res = await responses;
            var resGroupby = res.GroupBy(x => new { x.DSYearMonth, x.DSTypeID }).
                Select(y => new
                {
                    YearMonth = y.First().DSYearMonth,
                    Type = y.First().DSTypeID,
                    Amount = y.Sum(x => x.Amount)
                }).ToList();
            List<DSYearCreditDebitDiff> dsYearCreditDebitDiff = new List<DSYearCreditDebitDiff>();
            var ssssff = res.DistinctBy(x => x.DSYearMonth);
            foreach (var yearMonth in res.OrderBy(x => x.DSYearMonthOri).DistinctBy(x => x.DSYearMonth).Select(x => x.DSYearMonth))
            {
                var credit = resGroupby.FirstOrDefault(x => x.YearMonth == yearMonth && x.Type == (int)EnumDSTranType.Income)?.Amount;
                var debit = resGroupby.FirstOrDefault(x => x.YearMonth == yearMonth && x.Type == (int)EnumDSTranType.Expense)?.Amount;
                var diff = credit - debit;

                dsYearCreditDebitDiff.Add(new DSYearCreditDebitDiff
                {
                    YearMonth = yearMonth,
                    Credit = credit ?? 0,
                    Debit = debit ?? 0,
                    Diff = diff ?? 0
                });
            }
            return dsYearCreditDebitDiff;
        }
        public async Task<IEnumerable<DSDebitStat>> GetDSMonthlyExpensesAsync(int year, int month)
        {
            var responses = (
                 from a in context.DSTransaction
                 join b in context.DSItem on a.DSItemID equals b.ID into bb
                 from b2 in bb.DefaultIfEmpty()
                 join c in context.DSItemSub on a.DSItemSubID equals c.ID into cc
                 from c2 in cc.DefaultIfEmpty()
                 join d in context.DSItem on c2.DSItemID equals d.ID into dd
                 from d2 in dd.DefaultIfEmpty()
                 where
                    a.DSTypeID == (int)EnumDSTranType.Expense &&
                    (a.CreatedDateTime.Year == year && a.CreatedDateTime.Month == month)
                 select new DSDebitStat
                 {
                     DSItemName = b2.ID > 0 ? b2.Name : $"{d2.Name}",
                     Amount = a.Amount,
                 }).ToListAsync();

            var res = await responses;
            res = res.GroupBy(x => x.DSItemName).Select(y => new DSDebitStat { DSItemName = y.First().DSItemName, Amount = y.Sum(x => x.Amount) }).ToList();
            return res.OrderByDescending(x => x.Amount);
        }

        public async Task<IEnumerable<DSTransactionDtoV2>> GetDSTransactionAsyncV3(int memberId)
        {
            var transactionAll = await GetAllByMemberIdAsync(memberId);

            var finalRes = new List<DSTransactionDtoV2>();

            var dsaccountids = transactionAll.DistinctBy(x => x.DSAccountID).Select(x => x.DSAccountID);

            int rowID = 0;

            foreach (var dsaccountid in dsaccountids)
            {
                decimal balance = 0;
                var dsTransactionsByAcc = transactionAll.Where(x => x.DSAccountID == dsaccountid).OrderBy(x => x.CreatedDateTime);

                foreach (var dsTransactionByAcc in dsTransactionsByAcc)
                {
                    var dsTransferOutTran = new DSTransactionDto();
                    if (dsTransactionByAcc.DSTypeID == (int)EnumDSTranType.TransferOut)
                    {
                        dsTransferOutTran = transactionAll.FirstOrDefault(x => x.DSTransferOutID == dsTransactionByAcc.ID);
                    }

                    balance = _debitItems.Contains(dsTransactionByAcc.DSTypeID) ? balance - dsTransactionByAcc.Amount : balance + dsTransactionByAcc.Amount;
                    finalRes.Add(new DSTransactionDtoV2
                    {
                        RowID = rowID++,
                        DSTypeName = dsTransactionByAcc.DSTypeName,
                        DSAccountName = dsTransactionByAcc.DSAccountName,
                        DSItemName = dsTransactionByAcc.DSTypeID == 3 ?
                            dsTransferOutTran.DSAccountName :
                            dsTransactionByAcc.DSItemName,
                        ID = dsTransactionByAcc.ID,
                        DSTypeID = dsTransactionByAcc.DSTypeID,
                        DSAccountID = dsTransactionByAcc.DSAccountID,
                        DSAccountToID = dsTransactionByAcc.DSTypeID == 3 ? dsTransferOutTran.DSAccountID : 0,
                        DSTransferOutID = dsTransactionByAcc.DSTransferOutID,
                        Description = dsTransactionByAcc.Description,
                        CreatedDateTime = dsTransactionByAcc.CreatedDateTime,
                        Amount = dsTransactionByAcc.Amount,
                        Balance = balance
                    }); ;
                }
            }
            var finalResOrdered = finalRes.OrderByDescending(x => x.CreatedDateTime).ThenByDescending(x => x.RowID);
            return [.. finalResOrdered];
        }

        public async Task<IEnumerable<DSTransactionDtoV2>> GetDSTransactionByDSAccountAsync(int dsAccountID, int memberId)
        {
            var transactionAll = await GetAllByMemberIdAsync(memberId);

            var finalRes = new List<DSTransactionDtoV2>();

            var dsaccountids = new List<int> { dsAccountID };

            int rowID = 0;

            foreach (var dsaccountid in dsaccountids)
            {
                decimal balance = 0;
                var dsTransactionsByAcc = transactionAll.Where(x => x.DSAccountID == dsaccountid).OrderBy(x => x.CreatedDateTime);

                foreach (var dsTransactionByAcc in dsTransactionsByAcc)
                {
                    var dsTransferOutTran = new DSTransactionDto();
                    if (dsTransactionByAcc.DSTypeID == (int)EnumDSTranType.TransferOut)
                    {
                        dsTransferOutTran = transactionAll.FirstOrDefault(x => x.DSTransferOutID == dsTransactionByAcc.ID);
                    }

                    balance = _debitItems.Contains(dsTransactionByAcc.DSTypeID) ? balance - dsTransactionByAcc.Amount : balance + dsTransactionByAcc.Amount;
                    finalRes.Add(new DSTransactionDtoV2
                    {
                        RowID = rowID++,
                        DSTypeName = dsTransactionByAcc.DSTypeName,
                        DSAccountName = dsTransactionByAcc.DSAccountName,
                        DSItemName = dsTransactionByAcc.DSTypeID == 3 ?
                            dsTransferOutTran.DSAccountName :
                            dsTransactionByAcc.DSItemName,
                        ID = dsTransactionByAcc.ID,
                        DSTypeID = dsTransactionByAcc.DSTypeID,
                        DSAccountID = dsTransactionByAcc.DSAccountID,
                        DSAccountToID = dsTransactionByAcc.DSTypeID == 3 ? dsTransferOutTran.DSAccountID : 0,
                        DSTransferOutID = dsTransactionByAcc.DSTransferOutID,
                        Description = dsTransactionByAcc.Description,
                        CreatedDateTime = dsTransactionByAcc.CreatedDateTime,
                        Amount = dsTransactionByAcc.Amount,
                        Balance = balance
                    });
                }
            }
            var finalResOrdered = finalRes.OrderByDescending(x => x.CreatedDateTime).ThenByDescending(x => x.RowID);

            return [.. finalResOrdered];
        }

        public async Task<IEnumerable<DSTransactionDto>> GetAllByMemberIdAsync(int memberId)
        {
            var finalRes = new List<DSTransactionDto>();

            var responses = (
                 from a in context.DSTransaction
                 join b in context.DSAccount on a.DSAccountID equals b.ID
                 join c in context.DSType on a.DSTypeID equals c.ID
                 join d in context.DSItem on a.DSItemID equals d.ID into dd
                 from d2 in dd.DefaultIfEmpty()
                 join e in context.DSItemSub on a.DSItemSubID equals e.ID into ee
                 from e2 in ee.DefaultIfEmpty()
                 join f in context.DSItem on e2.DSItemID equals f.ID into ff
                 from f2 in ff.DefaultIfEmpty()
                 join g in context.DSTransaction on a.DSTransferOutID equals g.ID into gg
                 from g2 in gg.DefaultIfEmpty()
                 join h in context.DSAccount on g2.DSAccountID equals h.ID into hh
                 from h2 in hh.DefaultIfEmpty()
                 where a.MemberID == memberId
                 select new DSTransactionDto
                 {
                     DSTypeName = c.Name,
                     DSAccountName = b.Name,
                     CreatedDateTime = a.CreatedDateTime,
                     CreatedDateTimeYearMonth = new DateTime(a.CreatedDateTime.Year, a.CreatedDateTime.Month, 1),
                     DSItemName = c.ID == 4 ? h2.Name : d2.ID > 0 ? d2.Name : $"{f2.Name}|{e2.Name}",
                     DSItemNameMain = d2 != null ? d2.Name : f2.Name,
                     DSItemNameSub = d2 != null ? d2.Name : e2.Name,
                     ID = a.ID,
                     DSTypeID = a.DSTypeID,
                     DSAccountID = a.DSAccountID,
                     DSTransferOutID = a.DSTransferOutID,
                     DSItemID = _transferTypes.Contains(c.ID) ? 0 : a.DSItemID > 0 ? a.DSItemID : e2 != null ? e2.DSItemID : 999,
                     DSItemSubID = a.DSItemSubID,
                     Description = a.Description,
                     Amount = a.Amount,
                 }).ToListAsync();

            var dsTransactions = await responses;
            return dsTransactions;
        }

        public async Task<DSTransaction>? GetByTransferOutIdAsync(int id) =>
            await context.DSTransaction.FirstOrDefaultAsync(x => x.DSTransferOutID == id);

        public async Task<DSTransaction>? GetAsync(int id) =>
            await context.DSTransaction.FindAsync(id);

        public async Task<bool>? IsExistByMember(int memberid, int id) =>
            await context.DSTransaction.AnyAsync(x => x.MemberID == memberid && x.ID == id);

        public async Task<IEnumerable<DSTransaction>> GetAllAsync() =>
            await context.DSTransaction.ToListAsync();

        public async Task AddAsync(DSTransaction entity)
        {
            await context.DSTransaction.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public void Update(DSTransaction entity)
        {
            context.DSTransaction.Update(entity);
            context.SaveChanges();
        }

        public void Delete(DSTransaction entity)
        {
            context.DSTransaction.Remove(entity);
            context.SaveChanges();
        }
    }
}
