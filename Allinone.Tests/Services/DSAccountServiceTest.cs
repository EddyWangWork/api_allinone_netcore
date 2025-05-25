using Allinone.BLL.Members;
using Allinone.BLL.Todolists;
using Allinone.BLL;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.Domain.Enums;
using Allinone.Domain.Todolists;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Allinone.Domain.DS.Accounts;
using Allinone.BLL.DS.Accounts;
using Allinone.Domain.Exceptions;
using Allinone.DLL.UnitOfWork;
using Allinone.BLL.DS.Transactions;

namespace Allinone.Tests.Services
{
    public class DSAccountServiceTest
    {
        private readonly DSAccountService _dsAccountService;

        private readonly int _memberId = 1;

        private readonly int _dsAccountId = 1;
        private readonly string _dsAccountName = "dsAccountName";

        public DSAccountServiceTest()
        {
            BaseBLL.MemberId = _memberId;

            var services = new ServiceCollection();


            // Setup InMemory DbContext with preset data
            var options = new DbContextOptionsBuilder<DSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new DSContext(options);

            context.DSAccount.AddRange(
                new DSAccount { ID = 1, Name = _dsAccountName, MemberID = _memberId }
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

            var dsTransactionService = new DSTransactionService
                (unitOfWork, dsAccountRepository, dsItemRepository,
                dsTransactionRepository, memoryCacheHelper, mapModel);

            _dsAccountService = new DSAccountService(dsTransactionService, dsAccountRepository, memoryCacheHelper, mapModel);
        }

        [Fact]
        public async Task GetAllByMember_Returns_Success()
        {
            // Act
            var result = await _dsAccountService.Get();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_dsAccountName, result!.FirstOrDefault().Name);
        }

        [Fact]
        public async Task GetById_Returns_Success()
        {
            // Act
            var result = await _dsAccountService.Get(_dsAccountId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_dsAccountName, result!.Name);
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new DSAccountAddReq
            {
                Name = "newDSAccount"
            };

            // Act
            var result = await _dsAccountService.Add(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newDSAccount", result!.Name);
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new DSAccountAddReq
            {
                Name = "updatedDSAccount",
                IsActive = false
            };

            // Act
            var result = await _dsAccountService.Update(_dsAccountId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_dsAccountId, result!.ID);
            Assert.Equal("updatedDSAccount", result!.Name);
            Assert.Equal(false, result!.IsActive);
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _dsAccountService.Delete(_dsAccountId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<DSAccountNotFoundException>(async () =>
            {
                await _dsAccountService.Get(_dsAccountId);
            });
        }
    }
}
