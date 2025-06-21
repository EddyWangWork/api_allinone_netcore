using Allinone.BLL;
using Allinone.BLL.Diarys;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.Domain.Diarys.DiaryLocations;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Services
{
    public class DiaryLocationServiceTest
    {
        private readonly DiaryLocationService _diaryLocationService;

        private readonly int _memberId = 1;

        private readonly int _diaryLocationId = 1;
        private readonly string _diaryLocationName = "diaryLocationName";
        private readonly string _diaryLocationDesc = "diaryLocationDesc";

        public DiaryLocationServiceTest()
        {
            BaseBLL.MemberId = _memberId;

            var services = new ServiceCollection();

            // Setup InMemory DbContext with preset data
            var options = new DbContextOptionsBuilder<DSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new DSContext(options);

            context.DiaryLocation.AddRange(
                new DiaryLocation
                { ID = 1, Name = _diaryLocationName, Description = _diaryLocationDesc, MemberID = _memberId }
            );
            context.SaveChanges();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IMapModel, MapModel>();
            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheHelper>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();
            var memoryCacheHelper = services.BuildServiceProvider().GetRequiredService<MemoryCacheHelper>();

            var diaryLocationRepository = new DiaryLocationRepository(context);

            _diaryLocationService = new DiaryLocationService(diaryLocationRepository, memoryCacheHelper, mapModel);
        }

        [Fact]
        public async Task GetAllByMember_Returns_Success()
        {
            // Act
            var result = await _diaryLocationService.GetAllByMemberAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_diaryLocationName, result!.FirstOrDefault().Name);
            Assert.Equal(_diaryLocationDesc, result!.FirstOrDefault().Description);
        }

        [Fact]
        public async Task GetAllByMember_id_Returns_Success()
        {
            // Act
            var result = await _diaryLocationService.GetAllByMemberAsync(_diaryLocationId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_diaryLocationName, result!.Name);
            Assert.Equal(_diaryLocationDesc, result!.Description);
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new DiaryLocationAddReq
            {
                Name = "new name",
                Description = "new desc"
            };

            // Act
            var result = await _diaryLocationService.AddAsync(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(req.Name, result!.Name);
            Assert.Equal(req.Description, result!.Description);
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new DiaryLocationAddReq
            {
                Name = "update name",
                Description = "update desc"
            };

            // Act
            var result = await _diaryLocationService.UpdateAsync(_diaryLocationId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(req.Name, result!.Name);
            Assert.Equal(req.Description, result!.Description);
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _diaryLocationService.DeleteAsync(_diaryLocationId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<DiaryLocationNotFoundException>(async () =>
            {
                await _diaryLocationService.GetAllByMemberAsync(_diaryLocationId);
            });
        }
    }
}
