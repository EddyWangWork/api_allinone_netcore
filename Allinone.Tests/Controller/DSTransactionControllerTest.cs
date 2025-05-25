using Allinone.BLL.DS.Transactions;
using Allinone.BLL;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.DLL.UnitOfWork;
using Allinone.Domain.DS.Accounts;
using Allinone.Domain.DS.DSItems;
using Allinone.Domain.DS.Transactions;
using Allinone.Domain.Enums;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Allinone.API.Controllers;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Allinone.Domain.Exceptions;

namespace Allinone.Tests.Controller
{
    public class DSTransactionControllerTest
    {
        private readonly DSTransactionController _dsTransactionController;

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

        public DSTransactionControllerTest()
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

            var dsTransactionService = new DSTransactionService(
                unitOfWork,
                dsAccountRepository,
                dsItemRepository,
                dsTransactionRepository,
                memoryCacheHelper,
                mapModel);

            _dsTransactionController = new DSTransactionController(dsTransactionService);
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
            var result = await _dsTransactionController.GetDSTransactionsByDSAccountAsync(req);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            var clinetResultValues = (IEnumerable<DSTransactionDtoV2>)clinetResult.Value;
            var clinetResultValue = clinetResultValues.FirstOrDefault();

            Assert.NotNull(clinetResultValues);
            Assert.Equal(2, clinetResultValues.Count());
        }

        [Fact]
        public async Task GetAllByMember_Returns_Success()
        {
            // Act
            var result = await _dsTransactionController.GetAllByMemberIdAsync();

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            Assert.NotNull(clinetResult);
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
            var result = await _dsTransactionController.Add(req);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            var clinetResultValue = ((IEnumerable<DSTransaction>)clinetResult.Value).FirstOrDefault();

            Assert.Equal(_dsAccountId, clinetResultValue.DSAccountID);
            Assert.Equal((int)EnumDSTranType.Income, clinetResultValue.DSTypeID);
            Assert.Equal(_dsItemId2, clinetResultValue.DSItemID);
            Assert.Equal(200, clinetResultValue.Amount);
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
            var result = await _dsTransactionController.Add(req);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            var clinetResultValues = (IEnumerable<DSTransaction>)clinetResult.Value;
            var clinetResultValue = clinetResultValues.FirstOrDefault(x => x.DSTypeID == (int)EnumDSTranType.TransferOut);
            var clinetResultValue2 = clinetResultValues.FirstOrDefault(x => x.DSTypeID == (int)EnumDSTranType.TransferIn);

            Assert.NotNull(result);
            Assert.Equal(_dsAccountId, clinetResultValue.DSAccountID);
            Assert.Equal((int)EnumDSTranType.TransferOut, clinetResultValue.DSTypeID);
            Assert.Equal(100, clinetResultValue.Amount);

            Assert.Equal(_dsAccountId2, clinetResultValue2.DSAccountID);
            Assert.Equal((int)EnumDSTranType.TransferIn, clinetResultValue2.DSTypeID);
            Assert.Equal(100, clinetResultValue2.Amount);
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
            var result = await _dsTransactionController.Update(_dsTransactionId, req);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            var clinetResultValue = ((IEnumerable<DSTransaction>)clinetResult.Value).FirstOrDefault();

            Assert.NotNull(clinetResultValue);
            Assert.Equal(_dsAccountId2, clinetResultValue.DSAccountID);
            Assert.Equal((int)EnumDSTranType.Expense, clinetResultValue.DSTypeID);
            Assert.Equal(_dsItemId2, clinetResultValue.DSItemID);
            Assert.Equal(200, clinetResultValue.Amount);
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
            var result = await _dsTransactionController.Update(_dsTransactionId2, req);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            var clinetResultValues = (IEnumerable<DSTransaction>)clinetResult.Value;
            var clinetResultValue = clinetResultValues.FirstOrDefault();

            Assert.NotNull(clinetResultValues);
            Assert.Equal(1, clinetResultValues.Count());
            Assert.Equal(_dsAccountId2, clinetResultValue.DSAccountID);
            Assert.Equal((int)EnumDSTranType.Expense, clinetResultValue.DSTypeID);
            Assert.Equal(_dsItemId2, clinetResultValue.DSItemID);
            Assert.Equal(200, clinetResultValue.Amount);
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
            var result = await _dsTransactionController.Update(_dsTransactionId2, req);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            var clinetResultValues = (IEnumerable<DSTransaction>)clinetResult.Value;
            var clinetResultValue = clinetResultValues.FirstOrDefault(x => x.DSTypeID == (int)EnumDSTranType.TransferOut);
            var clinetResultValue2 = clinetResultValues.FirstOrDefault(x => x.DSTypeID == (int)EnumDSTranType.TransferIn);

            Assert.NotNull(clinetResultValues);
            Assert.Equal(2, clinetResultValues.Count());

            Assert.Equal(_dsAccountId2, clinetResultValue.DSAccountID);
            Assert.Equal((int)EnumDSTranType.TransferOut, clinetResultValue.DSTypeID);
            Assert.Equal(300, clinetResultValue.Amount);

            Assert.Equal(_dsAccountId, clinetResultValue2.DSAccountID);
            Assert.Equal((int)EnumDSTranType.TransferIn, clinetResultValue2.DSTypeID);
            Assert.Equal(300, clinetResultValue2.Amount);
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
            var result = await _dsTransactionController.Update(_dsTransactionId3, req);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            var clinetResultValues = (IEnumerable<DSTransaction>)clinetResult.Value;
            var clinetResultValue = clinetResultValues.FirstOrDefault();

            Assert.NotNull(clinetResultValues);
            Assert.Equal(1, clinetResultValues.Count());
            Assert.Equal(_dsAccountId2, clinetResultValue.DSAccountID);
            Assert.Equal((int)EnumDSTranType.Expense, clinetResultValue.DSTypeID);
            Assert.Equal(_dsItemId2, clinetResultValue.DSItemID);
            Assert.Equal(200, clinetResultValue.Amount);
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
            var result = await _dsTransactionController.Update(_dsTransactionId3, req);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            var clinetResultValues = (IEnumerable<DSTransaction>)clinetResult.Value;
            var clinetResultValue = clinetResultValues.FirstOrDefault(x => x.DSTypeID == (int)EnumDSTranType.TransferOut);
            var clinetResultValue2 = clinetResultValues.FirstOrDefault(x => x.DSTypeID == (int)EnumDSTranType.TransferIn);

            Assert.NotNull(clinetResultValues);
            Assert.Equal(2, clinetResultValues.Count());

            Assert.Equal(_dsAccountId2, clinetResultValue.DSAccountID);
            Assert.Equal((int)EnumDSTranType.TransferOut, clinetResultValue.DSTypeID);
            Assert.Equal(300, clinetResultValue.Amount);

            Assert.Equal(_dsAccountId, clinetResultValue2.DSAccountID);
            Assert.Equal((int)EnumDSTranType.TransferIn, clinetResultValue2.DSTypeID);
            Assert.Equal(300, clinetResultValue2.Amount);
        }

        [Fact]
        public async Task Delete_Income_Returns_Success()
        {
            // Act
            var result = await _dsTransactionController.Delete(_dsTransactionId);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            Assert.NotNull(clinetResult);

            // Act & Assert
            await Assert.ThrowsAsync<DSTransactionNotFoundException>(async () =>
            {
                await _dsTransactionController.Get(_dsTransactionId);
            });
        }

        [Fact]
        public async Task Delete_TransferOut_Returns_Success()
        {
            // Act
            var result = await _dsTransactionController.Delete(_dsTransactionId2);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            Assert.NotNull(clinetResult);

            // Act & Assert
            await Assert.ThrowsAsync<DSTransactionNotFoundException>(async () =>
            {
                await _dsTransactionController.Get(_dsTransactionId2);
            });

            await Assert.ThrowsAsync<DSTransactionNotFoundException>(async () =>
            {
                await _dsTransactionController.Get(_dsTransactionId3);
            });
        }

        [Fact]
        public async Task Delete_TransferIn_Returns_Success()
        {
            // Act
            var result = await _dsTransactionController.Delete(_dsTransactionId3);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            Assert.NotNull(clinetResult);

            // Act & Assert
            await Assert.ThrowsAsync<DSTransactionNotFoundException>(async () =>
            {
                await _dsTransactionController.Get(_dsTransactionId2);
            });

            await Assert.ThrowsAsync<DSTransactionNotFoundException>(async () =>
            {
                await _dsTransactionController.Get(_dsTransactionId3);
            });
        }
    }
}
