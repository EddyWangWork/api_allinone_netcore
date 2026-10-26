using Allinone.DLL.Repositories;
using Allinone.DLL.UnitOfWork;
using Allinone.Domain.DS.Transactions;
using Allinone.Domain.Enums;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Extension;
using Allinone.Helper.Mapper;


namespace Allinone.BLL.DS.Transactions
{
    public interface IDSTransactionService
    {
        Task<IEnumerable<DSMonthlyItemExpenses>> GetDSMonthlyItemExpensesAsync(int year, int month, int monthDuration);
        Task<IEnumerable<DSMonthlyPeriodCreditDebit>> GetDSMonthlyPeriodCreditDebitAsync(GetDSMonthlyPeriodCreditDebitReq req);
        Task<DSMonthlyExpenses> GetDSMonthlyCommitmentAndOtherAsync(GetDSMonthlyCommitmentAndOtherReq req);

        Task<DSYearExpenses> GetDSYearExpensesAsync(int year);
        Task<IEnumerable<DSYearCreditDebitDiff>> GetDSYearCreditDebitDiffAsync(int year);
        Task<IEnumerable<DSDebitStat>> GetDSMonthlyExpensesAsync(int year, int month);

        Task<IEnumerable<DSTransactionDtoV2>> GetDSTransactionAsyncV3(DSTransactionWithDateReq req);
        Task<IEnumerable<DSTransactionDtoV2>> GetDSTransactionByDSAccountAsync(GetDSTransactionAsyncV2Req req);

        Task<IEnumerable<DSTransactionDto>> GetAllByMemberIdCacheAsync();
        Task<IEnumerable<DSTransactionDto>> GetAllByMemberIdAsync();

        Task<DSTransaction> Get(int id);
        Task<IEnumerable<DSTransaction>> Add(DSTransactionReq req);
        Task<IEnumerable<DSTransaction>> Update(int id, DSTransactionReq req);
        Task<IEnumerable<DSTransaction>> Delete(int id);
    }

    public class DSTransactionService(
        IUnitOfWork unitOfWork,
        IDSAccountRepository dsAccountRepository,
        IDSItemRepository dsItemRepository,
        IDSTransactionRepository dsTransactionRepository,
        MemoryCacheHelper memoryCacheHelper,
        IMapModel mapper) : BaseBLL, IDSTransactionService
    {
        private static readonly List<int> _transferTypes = [3, 4];
        private static readonly List<int> _expenseTypes =
            [(int)EnumDSTranType.Expense, (int)EnumDSTranType.TransferOut, (int)EnumDSTranType.DebitTransferOut];
        private static readonly List<int> _creditDebitTypes = [1, 2];
        private static readonly List<int> _creditItems = [19];
        private static readonly List<int> _debitItems = [1]; //commitment:1

        public async Task<IEnumerable<DSMonthlyItemExpenses>> GetDSMonthlyItemExpensesAsync(int year, int month, int monthDuration)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(monthDuration);
            var months = GetMonths(year, month, monthDuration);
            var transactionAll = (await GetAllByMemberIdCacheFuncAsync())
                .Where(x => x.DSTypeID == (int)EnumDSTranType.Expense
                    && x.CreatedDateTimeYearMonth >= months[^1]
                    && x.CreatedDateTimeYearMonth <= months[0])
                .ToList();

            var mainTotals = transactionAll
                .GroupBy(x => (x.CreatedDateTimeYearMonth, x.DSItemNameMain))
                .ToDictionary(group => group.Key, group => group.Sum(x => x.Amount));
            var subTotals = transactionAll
                .GroupBy(x => (x.CreatedDateTimeYearMonth, x.DSItemName))
                .ToDictionary(group => group.Key, group => group.Sum(x => x.Amount));
            var detailsByMonthAndItem = transactionAll.ToLookup(x => (x.CreatedDateTimeYearMonth, x.DSItemNameMain));
            var results = new List<DSMonthlyItemExpenses>();

            for (var i = 0; i < months.Count - 1; i++)
            {
                var currentMonth = months[i];
                var previousMonth = months[i + 1];
                var mainNames = GetNamesForMonths(transactionAll, currentMonth, previousMonth, x => x.DSItemNameMain);
                var subNames = GetNamesForMonths(transactionAll, currentMonth, previousMonth, x => x.DSItemName);
                var monthlyItems = CreateMonthlyItems(mainNames, mainTotals, currentMonth, previousMonth);
                var monthlySubItems = CreateMonthlyItems(subNames, subTotals, currentMonth, previousMonth);

                foreach (var item in monthlyItems)
                {
                    item.ItemsDetail = detailsByMonthAndItem[(currentMonth, item.ItemName)]
                        .Select(x => new DSMonthlyExpensesItem
                        {
                            ItemName = x.DSItemNameSub,
                            Desc = x.Description,
                            Amount = x.Amount
                        })
                        .OrderByDescending(x => x.Amount)
                        .ToList();
                }

                results.Add(new DSMonthlyItemExpenses
                {
                    YearMonthDatetime = currentMonth,
                    DSMonthlyItems = monthlyItems.OrderByDescending(x => x.Diff).ToList(),
                    DSMonthlySubItems = monthlySubItems.OrderByDescending(x => x.Diff).ToList()
                });
            }

            return results;
        }

        public async Task<IEnumerable<DSMonthlyPeriodCreditDebit>> GetDSMonthlyPeriodCreditDebitAsync(GetDSMonthlyPeriodCreditDebitReq req)
        {
            ArgumentNullException.ThrowIfNull(req);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(req.MonthDuration);
            var months = GetMonths(req.Year, req.Month, req.MonthDuration);
            var creditIds = req.CreditIds ?? [];
            var debitIds = req.DebitIds ?? [];
            var transactionAll = (await GetAllByMemberIdCacheFuncAsync())
                .Where(x => _creditDebitTypes.Contains(x.DSTypeID)
                    && x.CreatedDateTimeYearMonth >= months[^1]
                    && x.CreatedDateTimeYearMonth <= months[0])
                .ToLookup(x => x.CreatedDateTimeYearMonth);

            var monthlyPeriodCreditDebit = months.Select(month =>
            {
                var transactions = transactionAll[month];
                var creditTransactions = transactions.Where(x => x.DSTypeID == (int)EnumDSTranType.Income);
                var debitTransactions = transactions.Where(x => x.DSTypeID == (int)EnumDSTranType.Expense);

                if (creditIds.Count > 0)
                    creditTransactions = creditTransactions.Where(x => req.IsIncludeCredit
                        ? creditIds.Contains(x.DSItemID)
                        : !creditIds.Contains(x.DSItemID));
                if (debitIds.Count > 0)
                    debitTransactions = debitTransactions.Where(x => req.IsIncludeDebit
                        ? debitIds.Contains(x.DSItemID)
                        : !debitIds.Contains(x.DSItemID));

                var credit = creditTransactions.Sum(x => x.Amount);
                var debit = debitTransactions.Sum(x => x.Amount);

                return new DSMonthlyPeriodCreditDebit
                {
                    YearMonthDatetime = month,
                    YearMonth = $"{month.Year}-{month.Month}",
                    Credit = credit,
                    Debit = debit,
                    Remain = credit - debit,
                    Usage = credit > 0 && debit > 0 ? ((debit / credit) * 100).ToString("0") : "0"
                };
            }).ToList();

            for (var i = 0; i < monthlyPeriodCreditDebit.Count - 1; i++)
            {
                var current = monthlyPeriodCreditDebit[i];
                var previous = monthlyPeriodCreditDebit[i + 1];

                current.CreditCompare = current.Credit <= 0 || previous.Credit <= 0
                    ? "100.00"
                    : (((current.Credit / previous.Credit) - 1) * 100).ToString("0.00");
                current.DebitCompare = current.Debit == 0 || previous.Debit == 0
                    ? "0.00"
                    : (((current.Debit / previous.Debit) - 1) * 100).ToString("0.00");
                current.UsageCompare = (Convert.ToInt32(current.Usage) - Convert.ToInt32(previous.Usage)).ToString("0");
            }

            return monthlyPeriodCreditDebit;
        }

        public async Task<DSMonthlyExpenses> GetDSMonthlyCommitmentAndOtherAsync(GetDSMonthlyCommitmentAndOtherReq req)
        {
            ArgumentNullException.ThrowIfNull(req);
            var debitIds = req.DebitIds?.Count > 0 ? req.DebitIds : _debitItems;
            var transactionAll = await GetAllByMemberIdCacheFuncAsync();

            var monthlyExpenses = transactionAll.Where(x => x.DSTypeID == (int)EnumDSTranType.Expense
                && x.CreatedDateTime.Year == req.Year
                && x.CreatedDateTime.Month == req.Month);
            var commitments = monthlyExpenses
                .Where(x => debitIds.Contains(x.DSItemID))
                .Select(x => new DSMonthlyExpensesItem
                {
                    ItemName = x.DSItemNameSub,
                    Desc = x.Description,
                    Amount = x.Amount
                })
                .OrderByDescending(x => x.Amount)
                .ToList();
            var others = monthlyExpenses
                .Where(x => !debitIds.Contains(x.DSItemID))
                .GroupBy(x => x.DSItemNameMain)
                .Select(group => new DSMonthlyExpensesItem
                {
                    ItemName = group.Key,
                    Amount = group.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.Amount)
                .ToList();

            return new DSMonthlyExpenses()
            {
                Items = commitments,
                ItemsOther = others
            };
        }

        private static List<DateTime> GetMonths(int year, int month, int duration)
        {
            var endMonth = new DateTime(year, month, 1);
            return Enumerable.Range(0, duration)
                .Select(offset => endMonth.AddMonths(-offset))
                .ToList();
        }

        private static IEnumerable<string> GetNamesForMonths(
            IEnumerable<DSTransactionDto> transactions,
            DateTime currentMonth,
            DateTime previousMonth,
            Func<DSTransactionDto, string> getName) =>
            transactions
                .Where(x => x.CreatedDateTimeYearMonth == currentMonth || x.CreatedDateTimeYearMonth == previousMonth)
                .Select(getName)
                .Distinct()
                .OrderBy(name => name);

        private static List<DSMonthlyItem> CreateMonthlyItems(
            IEnumerable<string> names,
            IReadOnlyDictionary<(DateTime CreatedDateTimeYearMonth, string Name), decimal> totals,
            DateTime currentMonth,
            DateTime previousMonth) =>
            names.Select(name =>
            {
                totals.TryGetValue((currentMonth, name), out var currentAmount);
                totals.TryGetValue((previousMonth, name), out var previousAmount);
                var (percentageNumber, percentage) = GetComparePercentage(currentAmount, previousAmount);

                return new DSMonthlyItem
                {
                    ItemName = name,
                    Amount = currentAmount,
                    AmountLast = previousAmount,
                    Diff = currentAmount - previousAmount,
                    DiffPercentageNumber = percentageNumber,
                    AmountComparePercentage = percentage
                };
            }).ToList();

        private static (decimal Number, string Text) GetComparePercentage(decimal currentAmount, decimal previousAmount)
        {
            if (currentAmount == 0 || previousAmount == 0)
            {
                var percentage = currentAmount == 0 ? -100 : 100;
                return (percentage, percentage.ToString());
            }

            var result = ((currentAmount / previousAmount) - 1) * 100;
            return (result, result.ToString("0.00"));
        }


        public async Task<DSYearExpenses> GetDSYearExpensesAsync(int year)
        {
            return await dsTransactionRepository.GetDSYearExpensesAsync(year, MemberId);
        }

        public async Task<IEnumerable<DSYearCreditDebitDiff>> GetDSYearCreditDebitDiffAsync(int year)
        {
            return await dsTransactionRepository.GetDSYearCreditDebitDiffAsync(year, MemberId);
        }

        public async Task<IEnumerable<DSDebitStat>> GetDSMonthlyExpensesAsync(int year, int month)
        {
            return await dsTransactionRepository.GetDSMonthlyExpensesAsync(year, month, MemberId);
        }



        public async Task<IEnumerable<DSTransactionDtoV2>> GetDSTransactionAsyncV3(DSTransactionWithDateReq req)
        {
            var transactionAll = await GetAllByMemberIdCacheFuncAsync();
            var rows = BuildTransactionRows(transactionAll, transactionAll.Select(x => x.DSAccountID).Distinct());

            if (req.DateFrom.IsNullOrEmpty() || req.DateTo.IsNullOrEmpty())
                return ApplyLimit(rows, req.DataLimit);

            return ApplyLimit(rows.Where(x => x.CreatedDateTime >= req.DateFrom && x.CreatedDateTime <= req.DateTo), req.DataLimit);
        }

        public async Task<IEnumerable<DSTransactionDtoV2>> GetDSTransactionByDSAccountAsync(GetDSTransactionAsyncV2Req req)
        {
            var transactionAll = await GetAllByMemberIdCacheFuncAsync();
            var rows = BuildTransactionRows(transactionAll, [req.DSAccountID]);

            if (req.DateFrom == null || req.DateTo == null)
                return ApplyLimit(rows, req.DataLimit);

            return ApplyLimit(rows.Where(x => x.CreatedDateTime >= req.DateFrom && x.CreatedDateTime <= req.DateTo), req.DataLimit);
        }

        private static IEnumerable<DSTransactionDtoV2> ApplyLimit(IEnumerable<DSTransactionDtoV2> rows, int dataLimit) =>
            dataLimit == 0 ? rows : rows.Take(dataLimit);

        // Builds per-account running balance rows, newest first.
        private static List<DSTransactionDtoV2> BuildTransactionRows(IEnumerable<DSTransactionDto> transactionAll, IEnumerable<int> dsAccountIds)
        {
            var finalRes = new List<DSTransactionDtoV2>();
            var transferOutLookup = transactionAll
                .Where(x => x.TransferOutTransactionID != 0)
                .GroupBy(x => x.TransferOutTransactionID)
                .ToDictionary(g => g.Key, g => g.First());
            var byAccount = transactionAll.ToLookup(x => x.DSAccountID);
            int rowID = 0;

            foreach (var dsAccountId in dsAccountIds)
            {
                decimal balance = 0;

                foreach (var tran in byAccount[dsAccountId].OrderBy(x => x.CreatedDateTime))
                {
                    var isTransferOut = tran.DSTypeID == (int)EnumDSTranType.TransferOut;
                    transferOutLookup.TryGetValue(tran.ID, out var transferIn);

                    balance = _expenseTypes.Contains(tran.DSTypeID) ? balance - tran.Amount : balance + tran.Amount;
                    finalRes.Add(new DSTransactionDtoV2
                    {
                        RowID = rowID++,
                        DSTypeName = tran.DSTypeName,
                        DSAccountName = tran.DSAccountName,
                        DSItemName = isTransferOut ? transferIn?.DSAccountName : tran.DSItemName,
                        ID = tran.ID,
                        DSTypeID = tran.DSTypeID,
                        DSAccountID = tran.DSAccountID,
                        DSAccountToID = isTransferOut ? transferIn?.DSAccountID ?? 0 : 0,
                        TransferOutTransactionID = tran.TransferOutTransactionID,
                        Description = tran.Description,
                        CreatedDateTime = tran.CreatedDateTime,
                        Amount = tran.Amount,
                        Balance = balance
                    });
                }
            }

            return [.. finalRes.OrderByDescending(x => x.CreatedDateTime).ThenByDescending(x => x.RowID)];
        }

        public async Task<IEnumerable<DSTransactionDto>> GetAllByMemberIdCacheAsync()
        {
            return await GetAllByMemberIdCacheFuncAsync();
        }

        public async Task<IEnumerable<DSTransactionDto>> GetAllByMemberIdAsync()
        {
            return await dsTransactionRepository.GetAllByMemberIdAsync(MemberId);
        }

        public async Task<DSTransaction> Get(int id)
        {
            return await dsTransactionRepository.GetAsync(id) ?? throw new DSTransactionNotFoundException();
        }

        public async Task<IEnumerable<DSTransaction>> Add(DSTransactionReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();
            List<DSTransaction> dsTransactions = [];

            NormalizeReq(req);
            await IsValidAddReq(req);

            if (req.DSTypeID == (int)EnumDSTranType.TransferIn)
                throw new DSTransactionBadRequestException("Transfer in is created by a transfer out");

            try
            {
                var entity = mapper.MapDto<DSTransactionReq, DSTransaction>(req);
                entity.MemberID = MemberId;

                await unitOfWork.BeginTransactionAsync();

                await dsTransactionRepository.AddAsync(entity);

                if (req.DSTypeID == (int)EnumDSTranType.TransferOut)
                {
                    var entityToAccount = CreateTransferIn(entity, req.DSAccountToID);
                    await dsTransactionRepository.AddAsync(entityToAccount);
                    dsTransactions.Add(entityToAccount);
                }

                await unitOfWork.SaveAsync();
                await unitOfWork.CommitTransactionAsync();

                dsTransactions.Add(entity);

                await SetAllByMemberIdCacheFuncAsync();

                return dsTransactions;
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<IEnumerable<DSTransaction>> Update(int id, DSTransactionReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            NormalizeReq(req);

            _ = await dsTransactionRepository.IsExistByMember(MemberId, id) ?
                true : throw new DSTransactionBadRequestException("Invalid DSTransaction id");

            await IsValidAddReq(req);

            List<DSTransaction> dsTransactions = [];

            try
            {
                await unitOfWork.BeginTransactionAsync();

                var origin = await dsTransactionRepository.GetAsync(id) ?? throw new DSTransactionNotFoundException();

                if (_transferTypes.Contains(origin.DSTypeID))
                {
                    var pair = await GetTransferPairAsync(origin) ?? throw new DSTransactionNotFoundException();

                    if (_transferTypes.Contains(req.DSTypeID))
                    {
                        var isOriginOut = origin.DSTypeID == (int)EnumDSTranType.TransferOut;
                        origin.DSAccountID = isOriginOut ? req.DSAccountID : req.DSAccountToID;
                        pair.DSAccountID = isOriginOut ? req.DSAccountToID : req.DSAccountID;

                        foreach (var row in new[] { origin, pair })
                        {
                            row.Amount = req.Amount;
                            row.Description = req.Description;
                            row.CreatedDateTime = req.CreatedDateTime;
                        }

                        dsTransactions.Add(pair);
                    }
                    else
                    {
                        origin.TransferOutTransactionID = null;
                        dsTransactionRepository.Delete(pair);
                        mapper.Map(req, origin);
                    }
                }
                else
                {
                    if (req.DSTypeID == (int)EnumDSTranType.TransferIn)
                        throw new DSTransactionBadRequestException("Transfer in is created by a transfer out");

                    mapper.Map(req, origin);

                    if (req.DSTypeID == (int)EnumDSTranType.TransferOut)
                    {
                        var toAccount = CreateTransferIn(origin, req.DSAccountToID);
                        await dsTransactionRepository.AddAsync(toAccount);
                        dsTransactions.Add(toAccount);
                    }
                }

                await unitOfWork.SaveAsync();
                await unitOfWork.CommitTransactionAsync();

                dsTransactions.Add(origin);

                await SetAllByMemberIdCacheFuncAsync();

                return dsTransactions;
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<IEnumerable<DSTransaction>> Delete(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            _ = await dsTransactionRepository.IsExistByMember(MemberId, id) ?
                true : throw new DSTransactionBadRequestException("Invalid DSTransaction id");

            List<DSTransaction> dsTransactions = [];

            try
            {
                await unitOfWork.BeginTransactionAsync();

                var origin = await dsTransactionRepository.GetAsync(id) ?? throw new DSTransactionNotFoundException();

                if (_transferTypes.Contains(origin.DSTypeID))
                {
                    var pair = await GetTransferPairAsync(origin) ?? throw new DSTransactionNotFoundException();

                    dsTransactionRepository.Delete(pair);
                    dsTransactions.Add(pair);
                }

                dsTransactionRepository.Delete(origin);

                await unitOfWork.SaveAsync();
                await unitOfWork.CommitTransactionAsync();

                dsTransactions.Add(origin);

                await SetAllByMemberIdCacheFuncAsync();

                return dsTransactions;
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        private static void NormalizeReq(DSTransactionReq req)
        {
            if (req.DSItemID == 0) req.DSItemID = null;
            if (req.DSItemSubID == 0) req.DSItemSubID = null;

            if (_transferTypes.Contains(req.DSTypeID))
            {
                req.DSItemID = null;
                req.DSItemSubID = null;
            }
        }

        private static DSTransaction CreateTransferIn(DSTransaction transferOut, int dsAccountToId) => new()
        {
            DSTypeID = (int)EnumDSTranType.TransferIn,
            DSAccountID = dsAccountToId,
            Amount = transferOut.Amount,
            Description = transferOut.Description,
            CreatedDateTime = transferOut.CreatedDateTime,
            MemberID = transferOut.MemberID,
            TransferOutTransaction = transferOut
        };

        private async Task<DSTransaction?> GetTransferPairAsync(DSTransaction transaction) =>
            transaction.DSTypeID == (int)EnumDSTranType.TransferOut ?
                await dsTransactionRepository.GetByTransferOutIdAsync(transaction.ID) :
                await dsTransactionRepository.GetAsync(transaction.TransferOutTransactionID ?? 0);

        private async Task IsValidAddReq(DSTransactionReq req)
        {
            _ = req.CreatedDateTime != DateTime.MinValue ?
                true : throw new DSTransactionBadRequestException("Incorrect Date time");

            _ = Enum.IsDefined(typeof(EnumDSTranType), req.DSTypeID) ?
                true : throw new DSTransactionBadRequestException("Incorrect DSTypeID");

            _ = req.Amount > 0 ?
                true : throw new DSTransactionBadRequestException("Amount must not 0");

            _ = await dsAccountRepository.IsExistByMember(MemberId, req.DSAccountID) ?
                true : throw new DSAccountNotFoundException();

            if (_transferTypes.Contains(req.DSTypeID))
            {
                _ = (req.DSTypeID == (int)EnumDSTranType.TransferOut && req.DSAccountToID == 0) ?
                    throw new DSTransactionBadRequestException("Transfer out to account not found") : true;

                _ = await dsAccountRepository.IsExistByMember(MemberId, req.DSAccountToID) ?
                    true : throw new DSTransactionBadRequestException("Transfer out to account not valid");

                _ = (req.DSTypeID == (int)EnumDSTranType.TransferOut
                        && req.DSAccountID == req.DSAccountToID) ?
                    throw new DSTransactionBadRequestException("Transfer out account cannot be same") : true;
            }
            else
            {
                _ = (req.DSItemID is null && req.DSItemSubID is null) ?
                throw new DSAccountNotFoundException("DSItemID and DSItemSubID must atleast have 1") : true;

                if (req.DSItemID is int itemId)
                {
                    _ = await dsItemRepository.IsExistByMemberAndIdAsync(MemberId, itemId) ?
                    true : throw new DSItemNotFoundException();
                }
                if (req.DSItemSubID is int itemSubId)
                {
                    _ = await dsItemRepository.IsExistByMemberAndSubIdAsync(MemberId, itemSubId) ?
                    true : throw new DSItemSubNotFoundException();
                }

            }
        }

        private async Task<IEnumerable<DSTransactionDto>> GetAllByMemberIdCacheFuncAsync()
        {
            var data = await memoryCacheHelper.GetOrCreateAsync(
             $"DSTransaction-{MemberId}",
             async () =>
             {
                 return await dsTransactionRepository.GetAllByMemberIdAsync(MemberId);
             },
             TimeSpan.FromMinutes(120));

            return data;
        }

        private async Task SetAllByMemberIdCacheFuncAsync()
        {
            await memoryCacheHelper.SetAsync(
             $"DSTransaction-{MemberId}",
             async () =>
             {
                 return await dsTransactionRepository.GetAllByMemberIdAsync(MemberId);
             },
             TimeSpan.FromMinutes(120));
        }
    }
}
