using Allinone.BLL;
using Allinone.BLL.Shops;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.DLL.UnitOfWork;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Shops.ShopTypes;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Services
{
    public class ShopTypeServiceTest
    {
        private readonly ShopTypeService _shopTypeService;

        private readonly int _memberId = 1;

        private readonly int _shopTypeId = 1;
        private readonly string _shopTypeName = "shopTypeName";

        private readonly int _shopTypeId2 = 2;
        private readonly string _shopTypeName2 = "shopTypeName2";

        public ShopTypeServiceTest()
        {
            BaseBLL.MemberId = _memberId;

            var services = new ServiceCollection();

            var options = new DbContextOptionsBuilder<DSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var context = new DSContext(options);

            context.ShopType.AddRange(
                new ShopType
                {
                    ID = _shopTypeId,
                    MemberID = _memberId,
                    Name = _shopTypeName
                },
                new ShopType
                {
                    ID = _shopTypeId2,
                    MemberID = 100,
                    Name = _shopTypeName2
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

            var shopTypeRepository = new ShopTypeRepository(context);

            _shopTypeService = new ShopTypeService(shopTypeRepository, mapModel);
        }

        [Fact]
        public async Task GetAllByMemberAndIdsAsync_Returns_Success()
        {
            //Assign
            var ids = new List<int> { _shopTypeId, _shopTypeId2 };

            // Act
            var result = await _shopTypeService.GetAllByMemberAsync(ids);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result!.Count());
        }

        [Fact]
        public async Task GetAllByMemberAsync_Returns_Success()
        {
            // Act
            var result = await _shopTypeService.GetAllByMemberAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result!.Count());
        }

        [Fact]
        public async Task GetById_Returns_Success()
        {
            // Act
            var result = await _shopTypeService.Get(_shopTypeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_shopTypeId, result!.ID);
            Assert.Equal(_shopTypeName, result!.Name);
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new ShopTypeAddReq
            {
                Name = "new ShopType"
            };

            // Act
            var result = await _shopTypeService.Add(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result!.ID);
            Assert.Equal("new ShopType", result!.Name);
        }

        [Fact]
        public async Task Update_Returns_Failed()
        {
            // Assign
            var req = new ShopTypeAddReq
            {
                Name = "updated ShopType"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ShopTypeNotFoundException>(async () =>
            {
                await _shopTypeService.Update(_shopTypeId2, req);
            });
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new ShopTypeAddReq
            {
                Name = "updated ShopType"
            };

            // Act
            var result = await _shopTypeService.Update(_shopTypeId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_shopTypeId, result!.ID);
            Assert.Equal("updated ShopType", result!.Name);
        }

        [Fact]
        public async Task Delete_Returns_Failed()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ShopTypeNotFoundException>(async () =>
            {
                await _shopTypeService.Delete(_shopTypeId2);
            });
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _shopTypeService.Delete(_shopTypeId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<ShopTypeNotFoundException>(async () =>
            {
                await _shopTypeService.Get(_shopTypeId);
            });
        }
    }
}
