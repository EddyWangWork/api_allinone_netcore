using Allinone.BLL;
using Allinone.BLL.Diarys;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.Domain.Diarys;
using Allinone.Domain.Diarys.DiaryActivitys;
using Allinone.Domain.Diarys.DiaryBooks;
using Allinone.Domain.Diarys.DiaryDetails;
using Allinone.Domain.Diarys.DiaryEmotions;
using Allinone.Domain.Diarys.DiaryFoods;
using Allinone.Domain.Diarys.DiaryLocations;
using Allinone.Domain.Diarys.DiaryTypes;
using Allinone.Domain.Diarys.DiaryWeathers;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Services
{
    public class DiaryDetailServiceTest
    {
        private readonly DiaryDetailService _diaryDetailService;

        private readonly int _memberId = 1;

        private readonly int _diaryTypeId = 1;
        private readonly string _diaryTypeName = "diaryTypeName";
        private readonly string _diaryTypeDesc = "diaryTypeDesc";

        private readonly int _diaryTypeId2 = 2;
        private readonly string _diaryTypeName2 = "diaryTypeName2";
        private readonly string _diaryTypeDesc2 = "diaryTypeDesc2";

        private readonly int _diaryDetailId = 1;
        private readonly string _diaryDetailTitle = "diaryDetailTitle";
        private readonly string _diaryDetailDesc = "diaryDetailDesc";

        private readonly int _diaryId = 1;
        private readonly string _diaryTitle = "diaryTitle";
        private readonly string _diaryDesc = "diaryDesc";
        private readonly string _diaryActivitys = "1,2";
        private readonly string _diaryEmotions = "1";
        private readonly string _diaryFoods = "1";
        private readonly string _diaryLocations = "1";
        private readonly string _diaryBooks = "1";
        private readonly string _diaryWeathers = "1";

        private readonly int _diaryActivityId = 1;
        private readonly string _diaryActivityName = "diaryActivityName";
        private readonly string _diaryActivityDesc = "diaryActivityDesc";
        private readonly int _diaryActivityId2 = 2;
        private readonly string _diaryActivityName2 = "diaryActivityName2";
        private readonly string _diaryActivityDesc2 = "diaryActivityDesc2";

        private readonly int _diaryEmotionId = 1;
        private readonly string _diaryEmotionName = "diaryEmotionName";
        private readonly string _diaryEmotionDesc = "diaryEmotionDesc";

        private readonly int _diaryFoodId = 1;
        private readonly string _diaryFoodName = "diaryFoodName";
        private readonly string _diaryFoodDesc = "diaryFoodDesc";

        private readonly int _diaryLocationId = 1;
        private readonly string _diaryLocationName = "diaryLocationName";
        private readonly string _diaryLocationDesc = "diaryLocationDesc";

        private readonly int _diaryBookId = 1;
        private readonly string _diaryBookName = "diaryBookName";
        private readonly string _diaryBookDesc = "diaryBookDesc";

        private readonly int _diaryWeatherId = 1;
        private readonly string _diaryWeatherName = "diaryWeatherName";
        private readonly string _diaryWeatherDesc = "diaryWeatherDesc";

        public DiaryDetailServiceTest()
        {
            BaseBLL.MemberId = _memberId;

            var services = new ServiceCollection();

            // Setup InMemory DbContext with preset data
            var options = new DbContextOptionsBuilder<DSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new DSContext(options);

            context.DiaryActivity.AddRange(
                new DiaryActivity
                { ID = 1, Name = _diaryActivityName, Description = _diaryActivityDesc, MemberID = _memberId },
                new DiaryActivity
                { ID = 2, Name = _diaryActivityName2, Description = _diaryActivityDesc2, MemberID = _memberId }
            );

            context.DiaryEmotion.AddRange(
                new DiaryEmotion
                { ID = 1, Name = _diaryEmotionName, Description = _diaryEmotionDesc, MemberID = _memberId }
            );

            context.DiaryFood.AddRange(
                new DiaryFood
                { ID = 1, Name = _diaryFoodName, Description = _diaryFoodDesc, MemberID = _memberId }
            );

            context.DiaryLocation.AddRange(
                new DiaryLocation
                { ID = 1, Name = _diaryLocationName, Description = _diaryLocationDesc, MemberID = _memberId }
            );

            context.DiaryBook.AddRange(
                new DiaryBook
                { ID = 1, Name = _diaryBookName, Description = _diaryBookDesc, MemberID = _memberId }
            );

            context.DiaryWeather.AddRange(
                new DiaryWeather
                { ID = 1, Name = _diaryWeatherName, Description = _diaryWeatherDesc, MemberID = _memberId }
            );

            context.Diary.AddRange(
                new Diary
                {
                    ID = 1,
                    Date = DateTime.Now,
                    Title = _diaryTitle,
                    Description = _diaryDesc,
                    ActivityIDs = _diaryActivitys,
                    EmotionIDs = _diaryEmotions,
                    FoodIDs = _diaryFoods,
                    LocationIDs = _diaryLocations,
                    BookIDs = _diaryBooks,
                    WeatherIDs = _diaryWeathers,
                    MemberID = _memberId
                }
            );

            context.DiaryType.AddRange(
                new DiaryType
                { ID = 1, Name = _diaryTypeName, Description = _diaryTypeDesc, MemberID = _memberId },
                new DiaryType
                { ID = 2, Name = _diaryTypeName2, Description = _diaryTypeDesc2, MemberID = _memberId }
            );

            context.DiaryDetail.AddRange(
                new DiaryDetail
                {
                    ID = 1,
                    DiaryID = _diaryId,
                    DiaryTypeID = _diaryTypeId,
                    Title = _diaryDetailTitle,
                    Description = _diaryDetailDesc,
                    UpdateDate = DateTime.Now,
                }
            );
            context.SaveChanges();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IMapModel, MapModel>();
            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheHelper>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();
            var memoryCacheHelper = services.BuildServiceProvider().GetRequiredService<MemoryCacheHelper>();

            var diaryRepository = new DiaryRepository(context);
            var diaryTypeRepository = new DiaryTypeRepository(context);
            var diaryDetailRepository = new DiaryDetailRepository(context);

            _diaryDetailService = new DiaryDetailService(
                diaryRepository,
                diaryTypeRepository,
                diaryDetailRepository,
                memoryCacheHelper,
                mapModel);
        }

        [Fact]
        public async Task GetAllByMember_Returns_Success()
        {
            // Act
            var result = await _diaryDetailService.GetAllByMemberAsync();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByMemberAsync_id_Returns_Success()
        {
            // Act
            var result = await _diaryDetailService.GetByMemberAsync(_diaryDetailId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_diaryDetailTitle, result!.Title);
            Assert.Equal(_diaryDetailDesc, result!.Description);
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new DiaryDetailAddReq
            {
                DiaryID = _diaryId,
                DiaryTypeID = _diaryTypeId,
                Title = "new title",
                Description = "new description"
            };

            // Act
            var result = await _diaryDetailService.AddAsync(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(req.Title, result!.Title);
            Assert.Equal(req.Description, result!.Description);
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new DiaryDetailAddReq
            {
                DiaryID = _diaryId,
                DiaryTypeID = _diaryTypeId,
                Title = "update title",
                Description = "update description"
            };

            // Act
            var result = await _diaryDetailService.UpdateAsync(_diaryDetailId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(req.Title, result!.Title);
            Assert.Equal(req.Description, result!.Description);
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _diaryDetailService.DeleteAsync(_diaryDetailId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<DiaryDetailNotFoundException>(async () =>
            {
                await _diaryDetailService.GetByMemberAsync(_diaryDetailId);
            });
        }
    }
}
