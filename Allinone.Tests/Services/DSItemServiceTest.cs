using Allinone.BLL;
using Allinone.BLL.DS.DSItems;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.DLL.UnitOfWork;
using Allinone.Domain.DS.DSItems;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Services
{
    public class DSItemServiceTest
    {
        private readonly DSItemService _dsItemService;

        private readonly int _memberId = 1;

        private readonly int _dsItemId = 1;
        private readonly string _dsItemName = "dsItemName";

        private readonly int _dsItemSubId = 1;
        private readonly string _dsItemSubName = "dsItemSubName";

        public DSItemServiceTest()
        {
            BaseBLL.MemberId = _memberId;

            var services = new ServiceCollection();

            // Setup InMemory DbContext with preset data
            var options = new DbContextOptionsBuilder<DSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var context = new DSContext(options);

            context.DSItem.AddRange(
                new DSItem { ID = _dsItemId, Name = _dsItemName, IsActive = true, MemberID = _memberId }
            );

            context.DSItemSub.AddRange(
                new DSItemSub { ID = _dsItemSubId, Name = _dsItemSubName, IsActive = true, DSItemID = _dsItemId }
            );
            context.SaveChanges();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IMapModel, MapModel>();
            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheHelper>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();
            var memoryCacheHelper = services.BuildServiceProvider().GetRequiredService<MemoryCacheHelper>();
            var unitOfWork = new UnitOfWork(context);

            var dsItemRepository = new DSItemRepository(context);

            _dsItemService = new DSItemService(unitOfWork, dsItemRepository, memoryCacheHelper, mapModel);
        }

        [Fact]
        public async Task GetDSItems_Returns_Success()
        {
            // Act
            var result = await _dsItemService.GetDSItems();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetDSItemWithSubV2_Returns_Success()
        {
            // Act
            var result = await _dsItemService.GetDSItemWithSubV2();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetDSItemWithSubV3_Returns_Success()
        {
            // Act
            var result = await _dsItemService.GetDSItemWithSubV3();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_dsItemName, result!.FirstOrDefault().Name);
        }

        [Fact]
        public async Task AddWithSubNoSub_Returns_Success()
        {
            // Assign
            var req = new DSItemAddWithSubItemReq
            {
                Name = "newDSItem"
            };

            // Act
            var result = await _dsItemService.AddWithSub(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(true, result!);

            // Act
            var result2 = await _dsItemService.Get(2);

            // Assert
            Assert.NotNull(result2);
            Assert.Equal("newDSItem", result2!.Name);
        }

        [Fact]
        public async Task AddWithSub_Returns_Success()
        {
            // Assign
            var req = new DSItemAddWithSubItemReq
            {
                Name = "newDSItem",
                SubName = "newDSItemSub"
            };

            // Act
            var result = await _dsItemService.AddWithSub(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(true, result!);

            // Act
            var result2 = await _dsItemService.Get(2);

            // Assert
            Assert.NotNull(result2);
            Assert.Equal("newDSItem", result2!.Name);
            Assert.Equal("newDSItemSub", result2!.DSItemSubs.FirstOrDefault().Name);
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new DSItemAddReq
            {
                Name = "newDSItem",
                IsActive = true
            };

            // Act
            var result = await _dsItemService.Add(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newDSItem", result!.Name);
            Assert.Equal(true, result!.IsActive);
        }

        [Fact]
        public async Task IsExistByMemberAndSubId_Returns_Failed()
        {
            // Act
            var result = await _dsItemService.IsExistByMemberAndSubIdAsync(100);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(false, result!);
        }

        [Fact]
        public async Task IsExistByMemberAndSubId_Returns_Success()
        {
            // Act
            var result = await _dsItemService.IsExistByMemberAndSubIdAsync(_dsItemSubId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(true, result!);
        }

        [Fact]
        public async Task GetAllByMember_Returns_Success()
        {
            // Act
            var result = await _dsItemService.GetAllByMemberAsync();

            // Assert
            Assert.NotNull(result);
            //Assert.Equal(_dsItemName, result!.Name);
            //Assert.Equal(true, result!.IsActive);
        }

        [Fact]
        public async Task GetById_Returns_Success()
        {
            // Act
            var result = await _dsItemService.Get(_dsItemId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_dsItemName, result!.Name);
            Assert.Equal(true, result!.IsActive);
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new DSItemAddReq
            {
                Name = "updatedDSItem",
                IsActive = false
            };

            // Act
            var result = await _dsItemService.Update(_dsItemId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("updatedDSItem", result!.Name);
            Assert.Equal(false, result!.IsActive);
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _dsItemService.Delete(_dsItemId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<DSItemNotFoundException>(async () =>
            {
                await _dsItemService.Get(_dsItemId);
            });
        }
    }
}
