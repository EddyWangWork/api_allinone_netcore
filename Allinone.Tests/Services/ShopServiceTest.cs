using Allinone.BLL;
using Allinone.BLL.Shops;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.DLL.UnitOfWork;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Shops;
using Allinone.Domain.Shops.ShopTypes;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.Tests.Services
{
    public class ShopServiceTest
    {
        private readonly ShopService _shopService;

        private readonly int _memberId = 1;

        private readonly int _shopTypeId = 1;
        private readonly string _shopTypeName = "shopTypeName";

        private readonly int _shopTypeId2 = 2;
        private readonly string _shopTypeName2 = "shopTypeName2";

        private readonly int _shopTypeId3 = 3;
        private readonly string _shopTypeName3 = "shopTypeName3";

        private readonly int _shopId = 1;
        private readonly string _shopName = "shopName";
        private readonly string _shopComment = "shopComment";
        private readonly bool _shopIsVisited = true;
        private readonly string _shopLocation = "shopLocation";
        private readonly string _shopRemark = "shopRemark";
        private readonly int _shopStar = 1;
        private readonly string _shopTypes = "1,2";

        private readonly int _shopId2 = 2;
        private readonly string _shopName2 = "shopNam2e";
        private readonly string _shopComment2 = "shopComment2";
        private readonly bool _shopIsVisited2 = true;
        private readonly string _shopLocation2 = "shopLocation2";
        private readonly string _shopRemark2 = "shopRemark2";
        private readonly int _shopStar2 = 1;
        private readonly string _shopTypes2 = "1,2";

        public ShopServiceTest()
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
                    MemberID = _memberId,
                    Name = _shopTypeName2
                },
                new ShopType
                {
                    ID = _shopTypeId3,
                    MemberID = 100,
                    Name = _shopTypeName3
                }
            );

            context.Shop.AddRange(
                new Shop
                {
                    ID = _shopId,
                    Comment = _shopComment,
                    IsVisited = _shopIsVisited,
                    Location = _shopLocation,
                    MemberID = _memberId,
                    Name = _shopName,
                    Remark = _shopRemark,
                    Star = _shopStar,
                    Types = _shopTypes
                },
                new Shop
                {
                    ID = _shopId2,
                    Comment = _shopComment2,
                    IsVisited = _shopIsVisited2,
                    Location = _shopLocation2,
                    MemberID = 100,
                    Name = _shopName2,
                    Remark = _shopRemark2,
                    Star = _shopStar2,
                    Types = _shopTypes2
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
            var shopRepository = new ShopRepository(context);

            _shopService = new ShopService(shopTypeRepository, shopRepository, mapModel);
        }

        [Fact]
        public async Task GetAllByMemberAsync_Returns_Success()
        {
            // Act
            var result = await _shopService.GetAllByMemberAsync();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetById_Returns_Success()
        {
            // Act
            var result = await _shopService.Get(_shopId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_shopId, result!.ID);
            Assert.Equal(_shopName, result!.Name);
            Assert.Equal($"{_shopTypeId},{_shopTypeId2}", result!.Types);
            Assert.Equal(_shopComment, result!.Comment);
            Assert.Equal(_shopIsVisited, result!.IsVisited);
            Assert.Equal(_shopLocation, result!.Location);
            Assert.Equal(_shopStar, result!.Star);
        }

        [Fact]
        public async Task Add_Returns_Failed()
        {
            // Assign
            var req = new ShopAddReq
            {
                Name = "new Shop",
                //TypeList = [_shopTypeId, _shopTypeId2],
                //TypeList = [_shopTypeId2],
                Comment = "new Shop Comment",
                IsVisited = true,
                Location = "kd",
                Star = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<ShopTypeNotFoundException>(async () =>
            {
                await _shopService.Add(req);
            });
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new ShopAddReq
            {
                Name = "new Shop",
                //TypeList = [_shopTypeId],
                TypeList = [_shopTypeId, _shopTypeId2],
                Comment = "new Shop Comment",
                IsVisited = true,
                Location = "new Shop Location",
                Star = 1
            };

            // Act
            var result = await _shopService.Add(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result!.ID);
            Assert.Equal("new Shop", result!.Name);
            Assert.Equal($"{_shopTypeId},{_shopTypeId2}", result!.Types);
            Assert.Equal("new Shop Comment", result!.Comment);
            Assert.Equal(true, result!.IsVisited);
            Assert.Equal("new Shop Location", result!.Location);
            Assert.Equal(1, result!.Star);
        }

        [Fact]
        public async Task Update_Returns_ShopNotFoundException_Failed()
        {
            // Assign
            var req = new ShopAddReq
            {
                Name = "updated Shop",
                //TypeList = [_shopTypeId, _shopTypeId2],
                //TypeList = [_shopTypeId2],
                Comment = "updated Shop Comment",
                IsVisited = false,
                Location = "updated kd",
                Star = 2
            };

            // Act & Assert
            await Assert.ThrowsAsync<ShopNotFoundException>(async () =>
            {
                await _shopService.Update(_shopId2, req);
            });
        }

        [Fact]
        public async Task Update_Returns_ShopTypeNotFoundException_Failed()
        {
            // Assign
            var req = new ShopAddReq
            {
                Name = "updated Shop",
                //TypeList = [_shopTypeId, _shopTypeId2],
                TypeList = [_shopTypeId3],
                Comment = "updated Shop Comment",
                IsVisited = false,
                Location = "updated kd",
                Star = 2
            };

            // Act & Assert
            await Assert.ThrowsAsync<ShopTypeNotFoundException>(async () =>
            {
                await _shopService.Update(_shopId, req);
            });
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new ShopAddReq
            {
                Name = "updated Shop",
                TypeList = [_shopTypeId],
                Comment = "updated Shop Comment",
                IsVisited = false,
                Location = "updated Shop Location",
                Star = 2
            };

            // Act
            var result = await _shopService.Update(_shopId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_shopId, result!.ID);
            Assert.Equal("updated Shop", result!.Name);
            Assert.Equal($"{_shopTypeId}", result!.Types);
            Assert.Equal("updated Shop Comment", result!.Comment);
            Assert.Equal(false, result!.IsVisited);
            Assert.Equal("updated Shop Location", result!.Location);
            Assert.Equal(2, result!.Star);
        }

        [Fact]
        public async Task Delete_Returns_Failed()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ShopNotFoundException>(async () =>
            {
                await _shopService.Delete(_shopId2);
            });
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _shopService.Delete(_shopId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<ShopNotFoundException>(async () =>
            {
                await _shopService.Get(_shopId);
            });
        }
    }
}
