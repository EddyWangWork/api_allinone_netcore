using Allinone.BLL;
using Allinone.BLL.Diarys;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.Domain.Diarys.DiaryWeathers;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Services
{
    public class DiaryWeatherServiceTest
    {
        private readonly DiaryWeatherService _diaryWeatherService;

        private readonly int _memberId = 1;

        private readonly int _diaryWeatherId = 1;
        private readonly string _diaryWeatherName = "diaryWeatherName";
        private readonly string _diaryWeatherDesc = "diaryWeatherDesc";

        public DiaryWeatherServiceTest()
        {
            BaseBLL.MemberId = _memberId;

            var services = new ServiceCollection();

            // Setup InMemory DbContext with preset data
            var options = new DbContextOptionsBuilder<DSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new DSContext(options);

            context.DiaryWeather.AddRange(
                new DiaryWeather
                { ID = 1, Name = _diaryWeatherName, Description = _diaryWeatherDesc, MemberID = _memberId }
            );
            context.SaveChanges();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IMapModel, MapModel>();
            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheHelper>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();
            var memoryCacheHelper = services.BuildServiceProvider().GetRequiredService<MemoryCacheHelper>();

            var diaryWeatherRepository = new DiaryWeatherRepository(context);

            _diaryWeatherService = new DiaryWeatherService(diaryWeatherRepository, memoryCacheHelper, mapModel);
        }

        [Fact]
        public async Task GetAllByMember_Returns_Success()
        {
            // Act
            var result = await _diaryWeatherService.GetAllByMemberAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_diaryWeatherName, result!.FirstOrDefault().Name);
            Assert.Equal(_diaryWeatherDesc, result!.FirstOrDefault().Description);
        }

        [Fact]
        public async Task GetAllByMember_id_Returns_Success()
        {
            // Act
            var result = await _diaryWeatherService.GetAllByMemberAsync(_diaryWeatherId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_diaryWeatherName, result!.Name);
            Assert.Equal(_diaryWeatherDesc, result!.Description);
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new DiaryWeatherAddReq
            {
                Name = "new name",
                Description = "new desc"
            };

            // Act
            var result = await _diaryWeatherService.AddAsync(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(req.Name, result!.Name);
            Assert.Equal(req.Description, result!.Description);
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new DiaryWeatherAddReq
            {
                Name = "update name",
                Description = "update desc"
            };

            // Act
            var result = await _diaryWeatherService.UpdateAsync(_diaryWeatherId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(req.Name, result!.Name);
            Assert.Equal(req.Description, result!.Description);
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _diaryWeatherService.DeleteAsync(_diaryWeatherId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<DiaryWeatherNotFoundException>(async () =>
            {
                await _diaryWeatherService.GetAllByMemberAsync(_diaryWeatherId);
            });
        }
    }
}
