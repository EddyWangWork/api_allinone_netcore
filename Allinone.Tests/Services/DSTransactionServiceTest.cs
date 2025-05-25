using Allinone.BLL.DS.DSItems;
using Allinone.BLL;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.DLL.UnitOfWork;
using Allinone.Domain.DS.DSItems;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Allinone.BLL.DS.Transactions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Allinone.Domain.DS.Transactions;
using Allinone.Domain.Enums;
using Allinone.Domain.DS.Accounts;
using Allinone.Domain.Exceptions;

namespace Allinone.Tests.Services
{
    public class DSTransactionServiceTest
    {
        private readonly DSTransactionService _dsTransactionService;

        private readonly int _memberId = 1;

        private readonly int _dsAccountId = 1;
        private readonly int _dsAccountId2 = 2;
        private readonly string _dsAccountName = "dsAccountName";
        private readonly string _dsAccountName2 = "dsAccountName2";

        private readonly int _dsItemId = 1;
        private readonly int _dsItemId2 = 2;
        private readonly string _dsItemName = "dsItemName";
        private readonly string _dsItemName2 = "dsItemName2";

        private readonly int _dsTransactionId = 1;
        private readonly int _dsTransactionId2 = 2;
        private readonly int _dsTransactionId3 = 3;
        private readonly string _dsTransactionName = "dsTransactionName";
        private readonly string _dsTransactionName2 = "dsTransactionName2";
        private readonly string _dsTransactionName3 = "dsTransactionName3";

        public DSTransactionServiceTest()
        {
            BaseBLL.MemberId = _memberId;

            var services = new ServiceCollection();

            // Setup InMemory DbContext with preset data
            var options = new DbContextOptionsBuilder<DSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var context = new DSContext(options);

            context.DSAccount.AddRange(
                new DSAccount { ID = _dsAccountId, Name = _dsAccountName, MemberID = _memberId },
                new DSAccount { ID = _dsAccountId2, Name = _dsAccountName2, MemberID = _memberId }
            );

            context.DSItem.AddRange(
                new DSItem { ID = _dsItemId, Name = _dsItemName, IsActive = true, MemberID = _memberId },
                new DSItem { ID = _dsItemId2, Name = _dsItemName2, IsActive = true, MemberID = _memberId }
            );

            context.DSType.AddRange(
                new DSType { ID = (int)EnumDSTranType.Income, Name = EnumDSTranType.Income.ToString() },
                new DSType { ID = (int)EnumDSTranType.Expense, Name = EnumDSTranType.Expense.ToString() },
                new DSType { ID = (int)EnumDSTranType.TransferOut, Name = EnumDSTranType.TransferOut.ToString() },
                new DSType { ID = (int)EnumDSTranType.TransferIn, Name = EnumDSTranType.TransferIn.ToString() },
                new DSType { ID = (int)EnumDSTranType.DebitTransferOut, Name = EnumDSTranType.DebitTransferOut.ToString() },
                new DSType { ID = (int)EnumDSTranType.CreditTransferIn, Name = EnumDSTranType.CreditTransferIn.ToString() }
            );

            context.DSTransaction.AddRange(
                new DSTransaction //income
                {
                    ID = _dsTransactionId,
                    DSTypeID = (int)EnumDSTranType.Income,
                    DSAccountID = _dsAccountId,
                    //DSTransferOutID = 0,
                    DSItemID = _dsItemId,
                    CreatedDateTime = DateTime.UtcNow.AddHours(8),
                    Amount = 100,
                    MemberID = _memberId
                },
                new DSTransaction // transferOut
                {
                    ID = _dsTransactionId2,
                    DSTypeID = (int)EnumDSTranType.TransferOut,
                    DSAccountID = _dsAccountId,
                    //DSItemID = _dsItemId,
                    //DSTransferOutID = 0,
                    CreatedDateTime = DateTime.UtcNow.AddHours(8),
                    Amount = 100,
                    MemberID = _memberId
                },
                new DSTransaction // transferIn
                {
                    ID = _dsTransactionId3,
                    DSTypeID = (int)EnumDSTranType.TransferIn,
                    DSAccountID = _dsAccountId2,
                    //DSItemID = _dsItemId,
                    DSTransferOutID = _dsTransactionId2,
                    CreatedDateTime = DateTime.UtcNow.AddHours(8),
                    Amount = 100,
                    MemberID = _memberId
                }
            );
            context.SaveChanges();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IMapModel, MapModel>();
            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheHelper>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();
            var memoryCacheHelper = services.BuildServiceProvider().GetRequiredService<MemoryCacheHelper>();
            var unitOfWork = new UnitOfWork(context);

            var dsAccountRepository = new DSAccountRepository(context);
            var dsItemRepository = new DSItemRepository(context);
            var dsTransactionRepository = new DSTransactionRepository(context);

            _dsTransactionService = new DSTransactionService(
                unitOfWork,
                dsAccountRepository,
                dsItemRepository,
                dsTransactionRepository,
                memoryCacheHelper,
                mapModel);
        }

        [Fact]
        public async Task GetDSMonthlyExpensesAsync_Returns_Success()
        {
            // Act
            var result = await _dsTransactionService.GetDSMonthlyExpensesAsync(2025, 5);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetDSYearCreditDebitDiffAsync_Returns_Success()
        {
            // Act
            var result = await _dsTransactionService.GetDSYearCreditDebitDiffAsync(2025);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetDSYearExpensesAsync_Returns_Success()
        {
            // Act
            var result = await _dsTransactionService.GetDSYearExpensesAsync(2025);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetDSMonthlyCommitmentAndOtherAsync_Returns_Success()
        {
            // Assign
            var req = new GetDSMonthlyCommitmentAndOtherReq
            {
                DebitIds = [],
                Year = 2025,
                Month = 5
            };

            // Act
            var result = await _dsTransactionService.GetDSMonthlyCommitmentAndOtherAsync(req);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetDSMonthlyPeriodCreditDebitAsync_Returns_Success()
        {
            // Assign
            var req = new GetDSMonthlyPeriodCreditDebitReq
            {
                CreditIds = [],
                DebitIds = [],
                IsIncludeCredit = true,
                IsIncludeDebit = true,
                Year = 2025,
                Month = 3,
                MonthDuration = 1
            };

            // Act
            var result = await _dsTransactionService.GetDSMonthlyPeriodCreditDebitAsync(req);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetDSMonthlyItemExpensesAsync_Returns_Success()
        {
            // Act
            var result = await _dsTransactionService.GetDSMonthlyItemExpensesAsync(2025, 4, 3);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAllByMemberIdCacheAsync_Returns_Success()
        {
            // Act
            var result = await _dsTransactionService.GetAllByMemberIdCacheAsync();
            var result2 = await _dsTransactionService.GetAllByMemberIdCacheAsync();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAllDSTransaction_Returns_Success()
        {
            // Assign
            var req = new DSTransactionWithDateReq
            {
            };

            // Act
            var result = await _dsTransactionService.GetDSTransactionAsyncV3(req);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAllByDSAccount_Returns_Success()
        {
            // Assign
            var req = new GetDSTransactionAsyncV2Req
            {
                DSAccountID = _dsAccountId
            };

            // Act
            var result = await _dsTransactionService.GetDSTransactionByDSAccountAsync(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result!.Count());
        }

        [Fact]
        public async Task GetAllByMember_Returns_Success()
        {
            // Act
            var result = await _dsTransactionService.GetAllByMemberIdAsync();

            // Assert
            Assert.NotNull(result);
            //Assert.Equal(_dsItemName, result!.FirstOrDefault().Name);
        }

        [Fact]
        public async Task Add_Returns_Failed()
        {
            // Assign
            var req = new DSTransactionReq
            {
                DSTypeID = (int)EnumDSTranType.Income,
                DSAccountID = _dsAccountId,
                Amount = 100,
                DSItemID = _dsItemId,
                //DSItemSubID = _dsItemId,
                CreatedDateTime = DateTime.UtcNow.AddHours(8)
            };

            // Act
            var result = await _dsTransactionService.Add(req);

            // Assert
            var resultValue = result!.FirstOrDefault();

            //Assert.NotNull(result);
            //Assert.Equal(_dsAccountId, resultValue.DSAccountID);
            //Assert.Equal((int)EnumDSTranType.Income, resultValue.DSTypeID);
            //Assert.Equal(_dsItemId2, resultValue.DSItemID);
            //Assert.Equal(200, resultValue.Amount);
        }

        [Fact]
        public async Task AddIncome_Returns_Success()
        {
            // Assign
            var req = new DSTransactionReq
            {
                DSAccountID = _dsAccountId,
                DSTypeID = (int)EnumDSTranType.Income,
                DSItemID = _dsItemId2,
                Amount = 200,
                CreatedDateTime = DateTime.UtcNow.AddHours(8)
            };

            // Act
            var result = await _dsTransactionService.Add(req);

            // Assert
            var resultValue = result!.FirstOrDefault();

            Assert.NotNull(result);
            Assert.Equal(_dsAccountId, resultValue.DSAccountID);
            Assert.Equal((int)EnumDSTranType.Income, resultValue.DSTypeID);
            Assert.Equal(_dsItemId2, resultValue.DSItemID);
            Assert.Equal(200, resultValue.Amount);
        }

        [Fact]
        public async Task AddTransferOut_Returns_Success()
        {
            // Assign
            var req = new DSTransactionReq
            {
                DSAccountID = _dsAccountId,
                DSTypeID = (int)EnumDSTranType.TransferOut,
                DSAccountToID = _dsAccountId2,
                Amount = 100,
                CreatedDateTime = DateTime.UtcNow.AddHours(8)
            };

            // Act
            var result = await _dsTransactionService.Add(req);

            // Assert
            var resultValue = result!.FirstOrDefault(x => x.DSTypeID == (int)EnumDSTranType.TransferOut);
            var resultValue2 = result!.FirstOrDefault(x => x.DSTypeID == (int)EnumDSTranType.TransferIn);

            Assert.NotNull(result);
            Assert.Equal(_dsAccountId, resultValue.DSAccountID);
            Assert.Equal((int)EnumDSTranType.TransferOut, resultValue.DSTypeID);
            Assert.Equal(100, resultValue.Amount);

            Assert.Equal(_dsAccountId2, resultValue2.DSAccountID);
            Assert.Equal((int)EnumDSTranType.TransferIn, resultValue2.DSTypeID);
            Assert.Equal(100, resultValue2.Amount);
        }

        [Fact]
        public async Task UpdateIncome_ToExpense_Returns_Failed()
        {
            // Assign
            var req = new DSTransactionReq
            {
                DSAccountID = _dsAccountId2,
                DSTypeID = (int)EnumDSTranType.Expense,
                DSItemID = _dsItemId2,
                Amount = 200,
                CreatedDateTime = DateTime.UtcNow.AddHours(8)
            };

            // Act & Assert
            await Assert.ThrowsAsync<DSTransactionBadRequestException>(async () =>
            {
                await _dsTransactionService.Update(33, req);
            });
        }

        [Fact]
        public async Task UpdateIncome_ToExpense_Returns_Success()
        {
            // Assign
            var req = new DSTransactionReq
            {
                DSAccountID = _dsAccountId2,
                DSTypeID = (int)EnumDSTranType.Expense,
                DSItemID = _dsItemId2,
                Amount = 200,
                CreatedDateTime = DateTime.UtcNow.AddHours(8)
            };

            // Act
            var result = await _dsTransactionService.Update(_dsTransactionId, req);

            // Assert
            var resultValue = result!.FirstOrDefault();

            Assert.NotNull(result);
            Assert.Equal(_dsAccountId2, resultValue.DSAccountID);
            Assert.Equal((int)EnumDSTranType.Expense, resultValue.DSTypeID);
            Assert.Equal(_dsItemId2, resultValue.DSItemID);
            Assert.Equal(200, resultValue.Amount);
        }

        [Fact]
        public async Task UpdateTransferOut_ToExpense_Returns_Success()
        {
            // Assign
            var req = new DSTransactionReq
            {
                DSAccountID = _dsAccountId2,
                DSTypeID = (int)EnumDSTranType.Expense,
                DSItemID = _dsItemId2,
                Amount = 200,
                CreatedDateTime = DateTime.UtcNow.AddHours(8)
            };

            // Act
            var result = await _dsTransactionService.Update(_dsTransactionId2, req);

            // Assert
            var resultValue = result!.FirstOrDefault();

            Assert.NotNull(result);
            Assert.Equal(1, result!.Count());
            Assert.Equal(_dsAccountId2, resultValue.DSAccountID);
            Assert.Equal((int)EnumDSTranType.Expense, resultValue.DSTypeID);
            Assert.Equal(_dsItemId2, resultValue.DSItemID);
            Assert.Equal(200, resultValue.Amount);
        }

        [Fact]
        public async Task UpdateTransferOut_ChangeDSAccount_Returns_Success()
        {
            // Assign
            var req = new DSTransactionReq
            {
                DSAccountID = _dsAccountId2,
                DSTypeID = (int)EnumDSTranType.TransferOut,
                DSAccountToID = _dsAccountId,
                Amount = 300,
                CreatedDateTime = DateTime.UtcNow.AddHours(8)
            };

            // Act
            var result = await _dsTransactionService.Update(_dsTransactionId2, req);

            // Assert
            var resultValue = result!.FirstOrDefault(x => x.DSTypeID == (int)EnumDSTranType.TransferOut);
            var resultValue2 = result!.FirstOrDefault(x => x.DSTypeID == (int)EnumDSTranType.TransferIn);

            Assert.NotNull(result);
            Assert.Equal(2, result!.Count());

            Assert.Equal(_dsAccountId2, resultValue.DSAccountID);
            Assert.Equal((int)EnumDSTranType.TransferOut, resultValue.DSTypeID);
            Assert.Equal(300, resultValue.Amount);

            Assert.Equal(_dsAccountId, resultValue2.DSAccountID);
            Assert.Equal((int)EnumDSTranType.TransferIn, resultValue2.DSTypeID);
            Assert.Equal(300, resultValue2.Amount);
        }

        [Fact]
        public async Task UpdateTransferIn_ToExpense_Returns_Success()
        {
            // Assign
            var req = new DSTransactionReq
            {
                DSAccountID = _dsAccountId2,
                DSTypeID = (int)EnumDSTranType.Expense,
                DSItemID = _dsItemId2,
                Amount = 200,
                CreatedDateTime = DateTime.UtcNow.AddHours(8)
            };

            // Act
            var result = await _dsTransactionService.Update(_dsTransactionId3, req);

            // Assert
            var resultValue = result!.FirstOrDefault();

            Assert.NotNull(result);
            Assert.Equal(1, result!.Count());
            Assert.Equal(_dsAccountId2, resultValue.DSAccountID);
            Assert.Equal((int)EnumDSTranType.Expense, resultValue.DSTypeID);
            Assert.Equal(_dsItemId2, resultValue.DSItemID);
            Assert.Equal(200, resultValue.Amount);
        }

        [Fact]
        public async Task UpdateTransferIn_ChangeDSAccount_Returns_Success()
        {
            // Assign
            var req = new DSTransactionReq
            {
                DSAccountID = _dsAccountId2,
                DSTypeID = (int)EnumDSTranType.TransferOut,
                DSAccountToID = _dsAccountId,
                Amount = 300,
                CreatedDateTime = DateTime.UtcNow.AddHours(8)
            };

            // Act
            var result = await _dsTransactionService.Update(_dsTransactionId3, req);

            // Assert
            var resultValue = result!.FirstOrDefault(x => x.DSTypeID == (int)EnumDSTranType.TransferOut);
            var resultValue2 = result!.FirstOrDefault(x => x.DSTypeID == (int)EnumDSTranType.TransferIn);

            Assert.NotNull(result);
            Assert.Equal(2, result!.Count());

            Assert.Equal(_dsAccountId2, resultValue.DSAccountID);
            Assert.Equal((int)EnumDSTranType.TransferOut, resultValue.DSTypeID);
            Assert.Equal(300, resultValue.Amount);

            Assert.Equal(_dsAccountId, resultValue2.DSAccountID);
            Assert.Equal((int)EnumDSTranType.TransferIn, resultValue2.DSTypeID);
            Assert.Equal(300, resultValue2.Amount);
        }

        [Fact]
        public async Task Delete_Income_Returns_Failed()
        {
            // Act & Assert
            await Assert.ThrowsAsync<DSTransactionBadRequestException>(async () =>
            {
                await _dsTransactionService.Delete(333);
            });
        }

        [Fact]
        public async Task Delete_Income_Returns_Success()
        {
            // Act
            var result = await _dsTransactionService.Delete(_dsTransactionId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<DSTransactionNotFoundException>(async () =>
            {
                await _dsTransactionService.Get(_dsTransactionId);
            });
        }

        [Fact]
        public async Task Delete_TransferOut_Returns_Success()
        {
            // Act
            var result = await _dsTransactionService.Delete(_dsTransactionId2);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<DSTransactionNotFoundException>(async () =>
            {
                await _dsTransactionService.Get(_dsTransactionId2);
            });

            await Assert.ThrowsAsync<DSTransactionNotFoundException>(async () =>
            {
                await _dsTransactionService.Get(_dsTransactionId3);
            });
        }

        [Fact]
        public async Task Delete_TransferIn_Returns_Success()
        {
            // Act
            var result = await _dsTransactionService.Delete(_dsTransactionId3);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<DSTransactionNotFoundException>(async () =>
            {
                await _dsTransactionService.Get(_dsTransactionId2);
            });

            await Assert.ThrowsAsync<DSTransactionNotFoundException>(async () =>
            {
                await _dsTransactionService.Get(_dsTransactionId3);
            });
        }
    }
}
