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
using Microsoft.EntityFrameworkCore.Diagnostics;
using Allinone.Domain.Exceptions;

namespace Allinone.Tests.Services
{
    public class DSItemSubServiceTest
    {
        private readonly DSItemSubService _dsItemSubService;

        private readonly int _memberId = 1;

        private readonly int _dsItemId = 1;
        private readonly string _dsItemName = "dsItemName";

        private readonly int _dsItemSubId = 1;
        private readonly string _dsItemSubName = "dsItemSubName";

        public DSItemSubServiceTest()
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
            var dsItemSubRepository = new DSItemSubRepository(context);

            _dsItemSubService = new DSItemSubService(dsItemRepository, dsItemSubRepository, mapModel);
        }

        [Fact]
        public async Task GetAllByMember_Returns_Success()
        {
            // Act
            var result = await _dsItemSubService.GetAllByMemberAsync();

            var singleResult = result.FirstOrDefault();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_dsItemSubId, singleResult.ID);
            Assert.Equal(_dsItemSubName, singleResult.Name);
            Assert.Equal(_dsItemId, singleResult.DSItemID);
            Assert.Equal(_dsItemName, singleResult.DSItemName);
            Assert.Equal(true, singleResult.IsActive);
        }

        [Fact]
        public async Task GetById_Returns_Success()
        {
            // Act
            var result = await _dsItemSubService.Get(_dsItemSubId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_dsItemSubName, result!.Name);
            Assert.Equal(true, result!.IsActive);
        }

        [Fact]
        public async Task Add_Returns_Failed()
        {
            // Assign
            var req = new DSItemSubAddReq
            {
                Name = "new dsItemSubName",
                IsActive = true,
                DSItemID = 100
            };

            // Act & Assert
            await Assert.ThrowsAsync<DSItemNotFoundException>(async () =>
            {
                await _dsItemSubService.Add(req);
            });
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new DSItemSubAddReq
            {
                Name = "new dsItemSubName",
                IsActive = true,
                DSItemID = _dsItemId
            };

            // Act
            var result = await _dsItemSubService.Add(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result!.ID);
            Assert.Equal(_dsItemId, result!.DSItemID);
            Assert.Equal("new dsItemSubName", result!.Name);
            Assert.Equal(true, result!.IsActive);
        }

        [Fact]
        public async Task Update_Returns_DSItemNotFound_Failed()
        {
            // Assign
            var req = new DSItemSubAddReq
            {
                Name = "updated dsItemSubName",
                IsActive = false,
                DSItemID = 100
            };

            // Act & Assert
            await Assert.ThrowsAsync<DSItemNotFoundException>(async () =>
            {
                await _dsItemSubService.Update(_dsItemSubId, req);
            });
        }

        [Fact]
        public async Task Update_Returns_DSItemSubNotFound_Failed()
        {
            // Assign
            var req = new DSItemSubAddReq
            {
                Name = "updated dsItemSubName",
                IsActive = false,
                DSItemID = _dsItemId
            };

            // Act & Assert
            await Assert.ThrowsAsync<DSItemSubNotFoundException>(async () =>
            {
                await _dsItemSubService.Update(100, req);
            });
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new DSItemSubAddReq
            {
                Name = "updated dsItemSubName",
                IsActive = false,
                DSItemID = _dsItemId
            };

            // Act
            var result = await _dsItemSubService.Update(_dsItemSubId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_dsItemId, result!.DSItemID);
            Assert.Equal("updated dsItemSubName", result!.Name);
            Assert.Equal(false, result!.IsActive);
        }

        [Fact]
        public async Task Delete_Returns_Failed()
        {
            // Act & Assert
            await Assert.ThrowsAsync<DSItemSubNotFoundException>(async () =>
            {
                await _dsItemSubService.Delete(100);
            });
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _dsItemSubService.Delete(_dsItemSubId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<DSItemSubNotFoundException>(async () =>
            {
                await _dsItemSubService.Get(_dsItemSubId);
            });
        }
    }
}
