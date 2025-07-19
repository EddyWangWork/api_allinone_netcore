using Allinone.BLL;
using Allinone.BLL.Shops;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.DLL.UnitOfWork;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Shops;
using Allinone.Domain.Shops.ShopDiarys;
using Allinone.Domain.Shops.ShopTypes;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Services
{
    public class ShopDiaryServiceTest
    {
        private readonly ShopDiaryService _shopDiaryService;

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

        private readonly int _shopDiaryId = 1;
        private readonly string _shopDiaryComment = "shopDiaryComment";
        private readonly DateTime _shopDiaryDate = DateTime.UtcNow.AddHours(8);
        private readonly string _shopDiaryRemark = "shopDiaryRemark";

        private readonly int _shopDiaryId2 = 2;
        private readonly string _shopDiaryComment2 = "shopDiaryComment2";
        private readonly DateTime _shopDiaryDate2 = DateTime.UtcNow.AddHours(8);
        private readonly string _shopDiaryRemark2 = "shopDiaryRemark2";

        public ShopDiaryServiceTest()
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

            context.ShopDiary.AddRange(
                new ShopDiary
                {
                    ID = _shopDiaryId,
                    ShopID = _shopId,
                    Comment = _shopDiaryComment,
                    Date = _shopDiaryDate,
                    Remark = _shopDiaryRemark

                },
                new ShopDiary
                {
                    ID = _shopDiaryId2,
                    ShopID = _shopId2,
                    Comment = _shopDiaryComment2,
                    Date = _shopDiaryDate2,
                    Remark = _shopDiaryRemark2
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

            var shopRepository = new ShopRepository(context);
            var shopTypeRepository = new ShopTypeRepository(context);
            var shopDiaryRepository = new ShopDiaryRepository(context);

            _shopDiaryService = new ShopDiaryService(shopRepository, shopTypeRepository, shopDiaryRepository, mapModel);
        }

        [Fact]
        public async Task GetShopDiariesByShopId_Returns_Success()
        {
            // Act
            var result = await _shopDiaryService.GetShopDiaries(_shopId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetShopDiaries_Returns_Success()
        {
            // Act
            var result = await _shopDiaryService.GetShopDiaries();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAllByMemberAsync_Returns_Success()
        {
            // Act
            var result = await _shopDiaryService.GetAllByMemberAsync();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetById_Returns_Success()
        {
            // Act
            var result = await _shopDiaryService.Get(_shopDiaryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_shopDiaryId, result!.ID);
            Assert.Equal(_shopDiaryComment, result!.Comment);
            Assert.Equal(_shopDiaryRemark, result!.Remark);
            Assert.Equal(_shopDiaryDate, result!.Date);
            Assert.Equal(_shopId, result!.ShopID);
        }

        [Fact]
        public async Task Add_Returns_Failed()
        {
            // Assign
            var req = new ShopDiaryAddReq
            {
                Comment = "new Comment",
                Remark = "new Remark",
                Date = _shopDiaryDate,
                ShopID = _shopId2
            };

            // Act & Assert
            await Assert.ThrowsAsync<ShopBadRequestException>(async () =>
            {
                await _shopDiaryService.Add(req);
            });
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new ShopDiaryAddReq
            {
                Comment = "new Comment",
                Remark = "new Remark",
                Date = _shopDiaryDate,
                ShopID = _shopId
            };

            // Act
            var result = await _shopDiaryService.Add(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result!.ID);
            Assert.Equal(req.Comment, result!.Comment);
            Assert.Equal(req.Remark, result!.Remark);
            Assert.Equal(req.Date, result!.Date);
            Assert.Equal(req.ShopID, result!.ShopID);
        }

        [Fact]
        public async Task Update_Returns_ShopDiaryNotFoundException_Failed()
        {
            // Assign
            var req = new ShopDiaryAddReq
            {
                Comment = "updated Comment",
                Remark = "updated Remark",
                Date = _shopDiaryDate,
                ShopID = _shopId
            };

            // Act & Assert
            await Assert.ThrowsAsync<ShopDiaryNotFoundException>(async () =>
            {
                await _shopDiaryService.Update(_shopDiaryId2, req);
            });
        }

        [Fact]
        public async Task Update_Returns_ShopBadRequestException_Failed()
        {
            // Assign
            var req = new ShopDiaryAddReq
            {
                Comment = "updated Comment",
                Remark = "updated Remark",
                Date = _shopDiaryDate,
                ShopID = _shopId2
            };

            // Act & Assert
            await Assert.ThrowsAsync<ShopBadRequestException>(async () =>
            {
                await _shopDiaryService.Update(_shopDiaryId, req);
            });
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new ShopDiaryAddReq
            {
                Comment = "updated Comment",
                Remark = "updated Remark",
                Date = _shopDiaryDate,
                ShopID = _shopId
            };

            // Act
            var result = await _shopDiaryService.Update(_shopDiaryId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_shopDiaryId, result!.ID);
            Assert.Equal(req.Comment, result!.Comment);
            Assert.Equal(req.Remark, result!.Remark);
            Assert.Equal(req.Date, result!.Date);
            Assert.Equal(req.ShopID, result!.ShopID);
        }

        [Fact]
        public async Task Delete_Returns_Failed()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ShopDiaryNotFoundException>(async () =>
            {
                await _shopDiaryService.Delete(_shopDiaryId2);
            });
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _shopDiaryService.Delete(_shopDiaryId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<ShopDiaryNotFoundException>(async () =>
            {
                await _shopDiaryService.Get(_shopDiaryId);
            });
        }
    }
}
