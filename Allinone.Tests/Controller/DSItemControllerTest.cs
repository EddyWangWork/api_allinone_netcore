using Allinone.BLL.DS.DSItems;
using Allinone.BLL;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
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
using Allinone.API.Controllers;
using Allinone.Domain.DS.Accounts;
using Microsoft.AspNetCore.Mvc;
using Allinone.Domain.Exceptions;
using Allinone.DLL.UnitOfWork;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Allinone.Tests.Controller
{
    public class DSItemControllerTest
    {
        private readonly DSItemController _dsItemController;

        private readonly int _memberId = 1;

        private readonly int _dsItemId = 1;
        private readonly string _dsItemName = "dsItemName";

        public DSItemControllerTest()
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
            context.SaveChanges();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IMapModel, MapModel>();
            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheHelper>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();
            var memoryCacheHelper = services.BuildServiceProvider().GetRequiredService<MemoryCacheHelper>();
            var unitOfWork = new UnitOfWork(context);

            var dsItemRepository = new DSItemRepository(context);

            var dsItemService = new DSItemService(unitOfWork, dsItemRepository, memoryCacheHelper, mapModel);
            _dsItemController = new DSItemController(dsItemService);
        }

        [Fact]
        public async Task GetAllWithSubByMember_ReturnsSuccess()
        {
            // Act
            var result = await _dsItemController.GetWithSubsAsync();

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            Assert.Equal(_dsItemName, ((IEnumerable<DSItemWithSubDtoV3>)clinetResult.Value).FirstOrDefault().Name);
        }

        [Fact]
        public async Task GetById_ReturnsSuccess()
        {
            // Act
            var result = await _dsItemController.Get(_dsItemId);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            Assert.Equal(_dsItemName, ((DSItem)clinetResult.Value).Name);
            Assert.Equal(true, ((DSItem)clinetResult.Value).IsActive);
        }

        [Fact]
        public async Task AddWithSubNoSub_ReturnsSuccess()
        {
            // Assign
            var req = new DSItemAddWithSubItemReq
            {
                Name = "newDSItem"
            };

            // Act
            var result = await _dsItemController.AddWithSub(req);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            // Act
            var result2 = await _dsItemController.Get(2);

            // Assert
            var clinetResult2 = Assert.IsType<OkObjectResult>(result2);
            Assert.Equal(200, clinetResult.StatusCode);

            Assert.Equal("newDSItem", ((DSItem)clinetResult2.Value).Name);
        }

        [Fact]
        public async Task AddWithSub_ReturnsSuccess()
        {
            // Assign
            var req = new DSItemAddWithSubItemReq
            {
                Name = "newDSItem",
                SubName = "newDSItemSub"
            };

            // Act
            var result = await _dsItemController.AddWithSub(req);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            // Act
            var result2 = await _dsItemController.Get(2);

            // Assert
            var clinetResult2 = Assert.IsType<OkObjectResult>(result2);
            Assert.Equal(200, clinetResult.StatusCode);

            Assert.Equal("newDSItem", ((DSItem)clinetResult2.Value).Name);
            Assert.Equal("newDSItemSub", ((DSItem)clinetResult2.Value).DSItemSubs.FirstOrDefault().Name);
        }

        [Fact]
        public async Task Add_ReturnsSuccess()
        {
            // Arrange
            var req = new DSItemAddReq
            {
                Name = "newDSItem",
                IsActive = true
            };

            // Act
            var result = await _dsItemController.Add(req);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            Assert.Equal("newDSItem", ((DSItem)clinetResult.Value).Name);
            Assert.Equal(true, ((DSItem)clinetResult.Value).IsActive);
        }

        [Fact]
        public async Task Update_ReturnsSuccess()
        {
            // Arrange
            var req = new DSItemAddReq
            {
                Name = "updatedDSItem",
                IsActive = false
            };

            // Act
            var result = await _dsItemController.Update(_dsItemId, req);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            Assert.Equal(_dsItemId, ((DSItem)clinetResult.Value).ID);
            Assert.Equal("updatedDSItem", ((DSItem)clinetResult.Value).Name);
            Assert.Equal(false, ((DSItem)clinetResult.Value).IsActive);
        }

        [Fact]
        public async Task Delete_ReturnsSuccess()
        {
            // Act
            var result = await _dsItemController.Delete(_dsItemId);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            Assert.Equal(_dsItemName, ((DSItem)clinetResult.Value).Name);
            Assert.Equal(true, ((DSItem)clinetResult.Value).IsActive);

            await Assert.ThrowsAsync<DSItemNotFoundException>(async () =>
            {
                await _dsItemController.Get(_dsItemId);
            });
        }
    }
}
