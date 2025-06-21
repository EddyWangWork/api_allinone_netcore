using Allinone.BLL;
using Allinone.BLL.Diarys;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.Domain.Diarys;
using Allinone.Domain.Diarys.DiaryActivitys;
using Allinone.Domain.Diarys.DiaryBooks;
using Allinone.Domain.Diarys.DiaryEmotions;
using Allinone.Domain.Diarys.DiaryFoods;
using Allinone.Domain.Diarys.DiaryLocations;
using Allinone.Domain.Diarys.DiaryWeathers;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Services
{
    public class DiaryServiceTest
    {
        private readonly DiaryService _diaryService;

        private readonly int _memberId = 1;

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

        public DiaryServiceTest()
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
                //,new Diary
                //{
                //    ID = 2,
                //    Date = DateTime.Now,
                //    MemberID = _memberId
                //}
            );
            context.SaveChanges();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IMapModel, MapModel>();
            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheHelper>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();
            var memoryCacheHelper = services.BuildServiceProvider().GetRequiredService<MemoryCacheHelper>();

            var diaryActivityRepository = new DiaryActivityRepository(context);
            var diaryEmotionRepository = new DiaryEmotionRepository(context);
            var diaryFoodRepository = new DiaryFoodRepository(context);
            var diaryLocationRepository = new DiaryLocationRepository(context);
            var diaryBookRepository = new DiaryBookRepository(context);
            var diaryWeatherRepository = new DiaryWeatherRepository(context);
            var diaryRepository = new DiaryRepository(context);

            _diaryService = new DiaryService(
                diaryRepository,
                diaryActivityRepository,
                diaryEmotionRepository,
                diaryFoodRepository,
                diaryLocationRepository,
                diaryBookRepository,
                diaryWeatherRepository,
                memoryCacheHelper,
                mapModel);
        }

        [Fact]
        public async Task GetAllByMember_Returns_Success()
        {
            // Act
            var result = await _diaryService.GetAllByMemberOrderByDateAsync();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByMemberAsync_id_Returns_Success()
        {
            // Act
            var result = await _diaryService.GetByMemberAsync(_diaryId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new DiaryAddReq
            {
                Date = DateTime.Now,
                Activitys = [_diaryActivityId, _diaryActivityId2, 3],
                Emotions = [_diaryEmotionId],
                Foods = [_diaryFoodId],
                Locations = [_diaryLocationId],
                Books = [_diaryBookId],
                Weathers = [_diaryWeatherId]
            };

            // Act
            var result = await _diaryService.AddAsync(req);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new DiaryAddReq
            {
                Date = DateTime.Now.AddDays(2),
                Title = "Updated Diary Title",
                Description = "Updated Diary Description",
                Activitys = [_diaryActivityId, _diaryActivityId2, 3],
                Emotions = [_diaryEmotionId],
                Foods = [_diaryFoodId],
                Locations = [_diaryLocationId],
                Books = [_diaryBookId],
                Weathers = [_diaryWeatherId]
            };

            // Act
            var result = await _diaryService.UpdateAsync(_diaryId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(req.Title, result!.Title);
            Assert.Equal(req.Description, result!.Description);
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _diaryService.DeleteAsync(_diaryId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<DiaryNotFoundException>(async () =>
            {
                await _diaryService.GetByMemberAsync(_diaryId);
            });
        }
    }
}
