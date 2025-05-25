using Allinone.DLL.Repositories;
using Allinone.DLL.UnitOfWork;
using Allinone.Domain.DS.Transactions;
using Allinone.Domain.Enums;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Extension;
using Allinone.Helper.Mapper;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private static readonly List<int> _creditDebitTypes = [1, 2];
        private static readonly List<int> _creditItems = [19];
        private static readonly List<int> _debitItems = [1]; //commitment:1

        public async Task<IEnumerable<DSMonthlyItemExpenses>> GetDSMonthlyItemExpensesAsync(int year, int month, int monthDuration)
        {
            var dateCurrent = new DateTime(year, month, 1).AddMonths(1);
            var datePrev = dateCurrent.AddMonths(-monthDuration);

            var transactionAll = await GetAllByMemberIdCacheFuncAsync();
            transactionAll = transactionAll.Where(x => x.DSTypeID == 2 && x.CreatedDateTime >= datePrev && x.CreatedDateTime < dateCurrent);

            var distinctDatetime = transactionAll.GroupBy(x => x.CreatedDateTimeYearMonth).Select(x => x.First().CreatedDateTimeYearMonth).OrderByDescending(x => x).ToList();
            var resGroupbyName = transactionAll.GroupBy(x => new { x.CreatedDateTimeYearMonth, x.DSItemNameMain }).Select(y => new
            {
                y.FirstOrDefault().CreatedDateTimeYearMonth,
                y.FirstOrDefault().DSItemNameMain,
                Amount = y.Sum(x => x.Amount)
            }).ToList();
            var resGroupbySubName = transactionAll.GroupBy(x => new { x.CreatedDateTimeYearMonth, x.DSItemName }).Select(y => new
            {
                y.FirstOrDefault().CreatedDateTimeYearMonth,
                y.FirstOrDefault().DSItemName,
                Amount = y.Sum(x => x.Amount)
            }).ToList();

            var finalRes = new List<DSMonthlyItemExpenses>();

            for (int i = 0; i <= distinctDatetime.Count() - 2; i++)
            {
                var monthlyDatetime = distinctDatetime[i];
                var monthlyItems = new List<DSMonthlyItem>();
                var monthlySubItems = new List<DSMonthlyItem>();

                var allitems = resGroupbyName.Where(x => x.CreatedDateTimeYearMonth == distinctDatetime[i]).Select(x => x.DSItemNameMain).
                        Union(resGroupbyName.Where(x => x.CreatedDateTimeYearMonth == distinctDatetime[i + 1]).Select(x => x.DSItemNameMain)).OrderBy(x => x).ToList();
                var allsubitems = resGroupbySubName.Where(x => x.CreatedDateTimeYearMonth == distinctDatetime[i]).Select(x => x.DSItemName).
                        Union(resGroupbySubName.Where(x => x.CreatedDateTimeYearMonth == distinctDatetime[i + 1]).Select(x => x.DSItemName)).OrderBy(x => x).ToList();

                allitems.ForEach(x =>
                {
                    var amountCurrent = resGroupbyName.FirstOrDefault(y => y.CreatedDateTimeYearMonth == distinctDatetime[i] && y.DSItemNameMain == x)?.Amount ?? 0;
                    var amountLast = resGroupbyName.FirstOrDefault(y => y.CreatedDateTimeYearMonth == distinctDatetime[i + 1] && y.DSItemNameMain == x)?.Amount ?? 0;
                    var amountDiff = amountCurrent - amountLast;
                    var diffPercentageNumber = (amountCurrent == 0 || amountLast == 0) ?
                        amountCurrent == 0 ?
                        -100 : 100 :
                        (((amountCurrent / amountLast) - 1) * 100);
                    var diffPercentage = (amountCurrent == 0 || amountLast == 0) ?
                        amountCurrent == 0 ?
                        "-100" : "100" :
                        (((amountCurrent / amountLast) - 1) * 100).ToString("0.00");

                    monthlyItems.Add(new DSMonthlyItem
                    {
                        ItemName = x,
                        Amount = amountCurrent,
                        AmountLast = amountLast,
                        Diff = amountDiff,
                        DiffPercentageNumber = diffPercentageNumber,
                        AmountComparePercentage = diffPercentage
                    });
                });

                allsubitems.ForEach(x =>
                {
                    var amountCurrent = resGroupbySubName.FirstOrDefault(y => y.CreatedDateTimeYearMonth == distinctDatetime[i] && y.DSItemName == x)?.Amount ?? 0;
                    var amountLast = resGroupbySubName.FirstOrDefault(y => y.CreatedDateTimeYearMonth == distinctDatetime[i + 1] && y.DSItemName == x)?.Amount ?? 0;
                    var amountDiff = amountCurrent - amountLast;
                    var diffPercentageNumber = (amountCurrent == 0 || amountLast == 0) ?
                        amountCurrent == 0 ?
                        -100 : 100 :
                        (((amountCurrent / amountLast) - 1) * 100);
                    var diffPercentage = (amountCurrent == 0 || amountLast == 0) ?
                        amountCurrent == 0 ?
                        "-100" : "100" :
                        (((amountCurrent / amountLast) - 1) * 100).ToString("0.00");

                    monthlySubItems.Add(new DSMonthlyItem
                    {
                        ItemName = x,
                        Amount = amountCurrent,
                        AmountLast = amountLast,
                        Diff = amountDiff,
                        DiffPercentageNumber = diffPercentageNumber,
                        AmountComparePercentage = diffPercentage
                    });
                });

                monthlyItems.ForEach(x =>
                {
                    var monthlyExpensesItems = new List<DSMonthlyExpensesItem>();

                    transactionAll.Where(y => y.DSItemNameMain == x.ItemName && y.CreatedDateTimeYearMonth == monthlyDatetime).ToList().ForEach(yy =>
                    {
                        monthlyExpensesItems.Add(new DSMonthlyExpensesItem
                        {
                            ItemName = yy.DSItemNameSub,
                            Desc = yy.Description,
                            Amount = yy.Amount
                        });
                    });

                    x.ItemsDetail = monthlyExpensesItems.OrderByDescending(x => x.Amount).ToList();
                });

                finalRes.Add(new DSMonthlyItemExpenses
                {
                    YearMonthDatetime = monthlyDatetime,
                    DSMonthlyItems = monthlyItems.OrderByDescending(x => x.Diff).ToList(),
                    DSMonthlySubItems = monthlySubItems.OrderByDescending(x => x.Diff).ToList()
                });
            }

            return finalRes;
        }

        public async Task<IEnumerable<DSMonthlyPeriodCreditDebit>> GetDSMonthlyPeriodCreditDebitAsync(GetDSMonthlyPeriodCreditDebitReq req)
        {
            var dateCurrent = new DateTime(req.Year, req.Month, 1).AddMonths(1);
            var datePrev = dateCurrent.AddMonths(-req.MonthDuration);

            var transactionAll = await GetAllByMemberIdCacheFuncAsync();
            transactionAll = transactionAll.Where(x => _creditDebitTypes.Contains(x.DSTypeID) && x.CreatedDateTime >= datePrev && x.CreatedDateTime < dateCurrent);

            var resGroupby = transactionAll.GroupBy(x => new { x.CreatedDateTime.Year, x.CreatedDateTime.Month }).ToList();
            List<DSMonthlyPeriodCreditDebit> monthlyPeriodCreditDebit = new List<DSMonthlyPeriodCreditDebit>();

            resGroupby.ForEach(x =>
            {
                var yearMonthDatetime = new DateTime(x.Key.Year, x.Key.Month, 1);
                var yearMonth = $"{x.Key.Year}-{x.Key.Month}";

                var credit = req.CreditIds.Count > 0 ?
                x.Where(x => x.DSTypeID == 1 && (req.IsIncludeCredit ? req.CreditIds.Contains(x.DSItemID) : !req.CreditIds.Contains(x.DSItemID))).Sum(x => x.Amount) :
                x.Where(x => x.DSTypeID == 1).Sum(x => x.Amount);

                var debit = req.DebitIds.Count > 0 ?
                x.Where(x => x.DSTypeID == 2 && (req.IsIncludeDebit ? req.DebitIds.Contains(x.DSItemID) : !req.DebitIds.Contains(x.DSItemID))).Sum(x => x.Amount) :
                x.Where(x => x.DSTypeID == 2).Sum(x => x.Amount);

                var remain = credit - debit;
                var usage = (debit > 0 && credit > 0) ? ((debit / credit) * 100).ToString("0") : 0.ToString("0");

                monthlyPeriodCreditDebit.Add(new DSMonthlyPeriodCreditDebit
                {
                    YearMonthDatetime = yearMonthDatetime,
                    YearMonth = yearMonth,
                    Credit = credit,
                    Debit = debit,
                    Remain = remain,
                    Usage = usage
                });
            });

            monthlyPeriodCreditDebit = monthlyPeriodCreditDebit.OrderByDescending(x => x.YearMonthDatetime).ToList();

            for (int i = 0; i <= monthlyPeriodCreditDebit.Count - 2; i++)
            {
                var monthlyPeriodCreditDebitFirst = monthlyPeriodCreditDebit[i];
                var monthlyPeriodCreditDebitNext = monthlyPeriodCreditDebit[i + 1];
                var debitFirst = monthlyPeriodCreditDebitFirst.Debit;
                var debitNext = monthlyPeriodCreditDebitNext.Debit;

                var creditCompare = (monthlyPeriodCreditDebit[i].Credit <= 0 || monthlyPeriodCreditDebit[i + 1].Credit <= 0) ? 100.ToString("0.00") : (((monthlyPeriodCreditDebit[i].Credit / monthlyPeriodCreditDebit[i + 1].Credit) - 1) * 100).ToString("0.00");

                monthlyPeriodCreditDebit[i].CreditCompare = creditCompare;

                monthlyPeriodCreditDebit[i].DebitCompare = debitFirst == 0 || debitNext == 0 ?
                    0.ToString("0.00") :
                    (((debitFirst / debitNext) - 1) * 100).ToString("0.00");

                //monthlyPeriodCreditDebit[i].DebitCompare =
                //    (((monthlyPeriodCreditDebit[i].Debit /
                //    monthlyPeriodCreditDebit[i + 1].Debit) - 1)
                //    * 100)
                //    .ToString("0.00");

                monthlyPeriodCreditDebit[i].UsageCompare =
                    (Convert.ToInt32(monthlyPeriodCreditDebit[i].Usage) -
                    Convert.ToInt32(monthlyPeriodCreditDebit[i + 1].Usage)).ToString("0");
            }

            return monthlyPeriodCreditDebit;
        }

        public async Task<DSMonthlyExpenses> GetDSMonthlyCommitmentAndOtherAsync(GetDSMonthlyCommitmentAndOtherReq req)
        {
            req.DebitIds = req.DebitIds.Count > 0 ? req.DebitIds : _debitItems;
            var transactionAll = await GetAllByMemberIdCacheFuncAsync();

            var commitments =
                transactionAll.
                    Where(x => x.DSTypeID == 2
                        && req.DebitIds.Contains(x.DSItemID)
                        && x.CreatedDateTime.Year == req.Year
                        && x.CreatedDateTime.Month == req.Month).
                    Select(xx => new DSMonthlyExpensesItem
                    {
                        ItemName = xx.DSItemNameSub,
                        Desc = xx.Description,
                        Amount = xx.Amount
                    }).OrderByDescending(x => x.Amount).ToList();

            var others =
                transactionAll.
                    Where(x => x.DSTypeID == 2
                        && !req.DebitIds.Contains(x.DSItemID)
                        && x.CreatedDateTime.Year == req.Year
                        && x.CreatedDateTime.Month == req.Month).
                    GroupBy(xx => xx.DSItemNameMain).
                    Select(xxx => new DSMonthlyExpensesItem
                    {
                        ItemName = xxx.Key,
                        Amount = xxx.Sum(x => x.Amount)
                    }).OrderByDescending(x => x.Amount).ToList();

            var commitment = commitments;

            return new DSMonthlyExpenses()
            {
                Items = commitments,
                ItemsOther = others
            };
        }


        public async Task<DSYearExpenses> GetDSYearExpensesAsync(int year)
        {
            return await dsTransactionRepository.GetDSYearExpensesAsync(year);
        }

        public async Task<IEnumerable<DSYearCreditDebitDiff>> GetDSYearCreditDebitDiffAsync(int year)
        {
            return await dsTransactionRepository.GetDSYearCreditDebitDiffAsync(year);
        }

        public async Task<IEnumerable<DSDebitStat>> GetDSMonthlyExpensesAsync(int year, int month)
        {
            return await dsTransactionRepository.GetDSMonthlyExpensesAsync(year, month);
        }



        public async Task<IEnumerable<DSTransactionDtoV2>> GetDSTransactionAsyncV3(DSTransactionWithDateReq req)
        {
            var transactionAll = await GetAllByMemberIdCacheFuncAsync();

            var finalRes = new List<DSTransactionDtoV2>();

            var dsaccountids = transactionAll.DistinctBy(x => x.DSAccountID).Select(x => x.DSAccountID);
            List<int> expensesList =
                new List<int> { (int)EnumDSTranType.Expense, (int)EnumDSTranType.TransferOut, (int)EnumDSTranType.DebitTransferOut };
            int rowID = 0;

            foreach (var dsaccountid in dsaccountids)
            {
                decimal balance = 0;
                var dsTransactionsByAcc = transactionAll.Where(x => x.DSAccountID == dsaccountid).OrderBy(x => x.CreatedDateTime);

                foreach (var dsTransactionByAcc in dsTransactionsByAcc)
                {
                    var dsTransferOutTran = new DSTransactionDto();
                    if (dsTransactionByAcc.DSTypeID == 3)
                    {
                        dsTransferOutTran = transactionAll.FirstOrDefault(x => x.DSTransferOutID == dsTransactionByAcc.ID);
                    }

                    balance = expensesList.Contains(dsTransactionByAcc.DSTypeID) ? balance - dsTransactionByAcc.Amount : balance + dsTransactionByAcc.Amount;
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
            if (req.DateFrom.IsNullOrEmpty() || req.DateTo.IsNullOrEmpty())
            {
                if (req.DataLimit == 0)
                    return finalResOrdered;
                return finalResOrdered.Take(req.DataLimit);
            }
            var finalResWithDateOrdered = finalResOrdered.Where(x => x.CreatedDateTime >= req.DateFrom && x.CreatedDateTime <= req.DateTo);

            if (req.DataLimit == 0)
                return finalResWithDateOrdered;
            return finalResWithDateOrdered.Take(req.DataLimit);
        }

        public async Task<IEnumerable<DSTransactionDtoV2>> GetDSTransactionByDSAccountAsync(GetDSTransactionAsyncV2Req req)
        {
            var transactionAll = await GetAllByMemberIdCacheFuncAsync();

            var finalRes = new List<DSTransactionDtoV2>();

            List<int> dsaccountids = [req.DSAccountID];

            List<int> expensesList = [
                (int)EnumDSTranType.Expense,
                (int)EnumDSTranType.TransferOut,
                (int)EnumDSTranType.DebitTransferOut
            ];

            int rowID = 0;

            foreach (var dsaccountid in dsaccountids)
            {
                decimal balance = 0;
                var dsTransactionsByAcc = transactionAll.Where(x => x.DSAccountID == dsaccountid).OrderBy(x => x.CreatedDateTime);

                foreach (var dsTransactionByAcc in dsTransactionsByAcc)
                {
                    var dsTransferOutTran = new DSTransactionDto();
                    if (dsTransactionByAcc.DSTypeID == 3)
                    {
                        dsTransferOutTran = transactionAll.FirstOrDefault(x => x.DSTransferOutID == dsTransactionByAcc.ID);
                    }

                    balance = expensesList.Contains(dsTransactionByAcc.DSTypeID) ? balance - dsTransactionByAcc.Amount : balance + dsTransactionByAcc.Amount;
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
            if (req.DateFrom == null || req.DateTo == null)
            {
                if (req.DataLimit == 0)
                    return finalResOrdered;
                return finalResOrdered.Take(req.DataLimit);
            }
            var finalResWithDateOrdered = finalResOrdered.Where(x => x.CreatedDateTime >= req.DateFrom && x.CreatedDateTime <= req.DateTo);

            if (req.DataLimit == 0)
                return finalResWithDateOrdered;
            return finalResWithDateOrdered.Take(req.DataLimit);
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

            await IsValidAddReq(req);

            try
            {
                var entity = mapper.MapDto<DSTransactionReq, DSTransaction>(req);
                entity.MemberID = MemberId;

                await unitOfWork.BeginTransactionAsync();

                await unitOfWork.DSTransaction.AddAsync(entity);
                await unitOfWork.SaveAsync();

                if (req.DSTypeID == (int)EnumDSTranType.TransferOut)
                {
                    DSTransaction entityToAccount = new()
                    {
                        DSTypeID = (int)EnumDSTranType.TransferIn,
                        Amount = entity.Amount,
                        Description = entity.Description,
                        DSAccountID = req.DSAccountToID,
                        DSTransferOutID = entity.ID,
                        CreatedDateTime = req.CreatedDateTime,
                        MemberID = MemberId
                    };

                    await unitOfWork.DSTransaction.AddAsync(entityToAccount);
                    await unitOfWork.SaveAsync();

                    dsTransactions.Add(entityToAccount);
                }

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

            _ = await dsTransactionRepository.IsExistByMember(MemberId, id) ?
                true : throw new DSTransactionBadRequestException("Invalid DSTransaction id");

            await IsValidAddReq(req);

            List<DSTransaction> dsTransactions = [];

            try
            {
                await unitOfWork.BeginTransactionAsync();

                var origin = await dsTransactionRepository.GetAsync(id) ?? throw new DSItemNotFoundException();

                if (req.DSTypeID == (int)EnumDSTranType.TransferOut)
                {
                    if (req.DSAccountToID == 0)
                        throw new DSTransactionBadRequestException("Must insert a transfer to account");

                    if (req.DSAccountToID == req.DSAccountID)
                        throw new DSTransactionBadRequestException("Transfer out account cannot be same");
                }

                if (_transferTypes.Contains(origin.DSTypeID))
                {
                    //var originFromToAccount = _context.DSTransactions.
                    //    FirstOrDefault(x => origin.DSTypeID == 3 ? x.DSTransferOutID == id : x.ID == origin.DSTransferOutID);

                    var originFromToAccount = origin.DSTypeID == (int)EnumDSTranType.TransferOut ?
                        await dsTransactionRepository.GetByTransferOutIdAsync(id) :
                        await dsTransactionRepository.GetAsync(origin.DSTransferOutID);

                    if (!_transferTypes.Contains(req.DSTypeID)) //not transfer type
                    {
                        dsTransactionRepository.Delete(originFromToAccount);
                        mapper.Map(req, origin);

                        dsTransactionRepository.Update(origin);
                    }
                    else
                    {
                        origin.DSAccountID = origin.DSTypeID == (int)EnumDSTranType.TransferOut ?
                            req.DSAccountID : req.DSAccountToID;
                        origin.Amount = req.Amount;
                        origin.Description = req.Description;

                        originFromToAccount.DSAccountID = origin.DSTypeID == 3 ? req.DSAccountToID : req.DSAccountID;
                        originFromToAccount.Amount = req.Amount;
                        originFromToAccount.Description = req.Description;

                        dsTransactionRepository.Update(origin);
                        dsTransactionRepository.Update(originFromToAccount);

                        dsTransactions.Add(originFromToAccount);
                    }
                }
                else
                {
                    mapper.Map(req, origin);
                    dsTransactionRepository.Update(origin);

                    if (_transferTypes.Contains(origin.DSTypeID)) //not transfer type
                    {
                        var toAccount = new DSTransaction
                        {
                            DSTransferOutID = origin.ID,
                            DSTypeID = (int)EnumDSTranType.TransferIn,
                            DSAccountID = req.DSAccountToID,
                            Description = origin.Description,
                            Amount = origin.Amount,
                            CreatedDateTime = origin.CreatedDateTime,
                            MemberID = MemberId
                        };

                        await dsTransactionRepository.AddAsync(toAccount);

                        dsTransactions.Add(toAccount);
                    }
                }

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

                dsTransactionRepository.Delete(origin);

                if (_transferTypes.Contains(origin.DSTypeID))
                {
                    var originFromToAccount = (origin.DSTypeID == (int)EnumDSTranType.TransferOut ?
                        await dsTransactionRepository.GetByTransferOutIdAsync(id) :
                        await dsTransactionRepository.GetAsync(origin.DSTransferOutID)) ??
                        throw new DSTransactionNotFoundException();

                    dsTransactionRepository.Delete(originFromToAccount);
                    dsTransactions.Add(originFromToAccount);
                }

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

            //var origin = _context.DSTransactions.FirstOrDefault(x => x.ID == id);
            //if (origin == null)
            //{
            //    throw new NotFoundException("Transaction Record not found");
            //}

            //_context.DSTransactions.Remove(origin);

            //if (_transferTypes.Contains(origin.DSTypeID))
            //{
            //    var originFromToAcction = _context.DSTransactions.
            //        FirstOrDefault(x => origin.DSTypeID == 3 ? x.DSTransferOutID == id : x.ID == origin.DSTransferOutID);
            //    if (originFromToAcction == null)
            //    {
            //        throw new NotFoundException("Transaction Record not found");
            //    }
            //    _context.DSTransactions.Remove(originFromToAcction);
            //}

            //_context.SaveChanges();
            //await SetGlobalDSTransactions();

            //return true;
        }

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
                _ = (req.DSItemID == 0 && req.DSItemSubID == 0) ?
                throw new DSAccountNotFoundException("DSItemID and DSItemSubID must atleast have 1") : true;

                if (req.DSItemID != 0)
                {
                    _ = await dsItemRepository.IsExistByMemberAndIdAsync(MemberId, req.DSItemID) ?
                    true : throw new DSItemNotFoundException();
                }
                if (req.DSItemSubID != 0)
                {
                    _ = await dsItemRepository.IsExistByMemberAndSubIdAsync(MemberId, req.DSItemSubID) ?
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
