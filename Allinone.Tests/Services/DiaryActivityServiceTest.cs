using Allinone.BLL;
using Allinone.BLL.Diarys;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.DLL.UnitOfWork;
using Allinone.Domain.Diarys.DiaryActivitys;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Services
{
    public class DiaryActivityServiceTest
    {
        private readonly DiaryActivityService _diaryActivityService;

        private readonly int _memberId = 1;

        private readonly int _diaryActivityId = 1;
        private readonly string _diaryActivityName = "diaryActivityName";
        private readonly string _diaryActivityDesc = "diaryActivityDesc";

        public DiaryActivityServiceTest()
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
                { ID = 1, Name = _diaryActivityName, Description = _diaryActivityDesc, MemberID = _memberId }
            );
            context.SaveChanges();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IMapModel, MapModel>();
            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheHelper>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();
            var memoryCacheHelper = services.BuildServiceProvider().GetRequiredService<MemoryCacheHelper>();

            var unitOfWork = new UnitOfWork(context);

            var diaryActivityRepository = new DiaryActivityRepository(context);

            _diaryActivityService = new DiaryActivityService(diaryActivityRepository, memoryCacheHelper, mapModel);
        }

        [Fact]
        public async Task GetAllByMember_Returns_Success()
        {
            // Act
            var result = await _diaryActivityService.GetAllByMemberAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_diaryActivityName, result!.FirstOrDefault().Name);
            Assert.Equal(_diaryActivityDesc, result!.FirstOrDefault().Description);
        }

        [Fact]
        public async Task GetAllByMember_id_Returns_Success()
        {
            // Act
            var result = await _diaryActivityService.GetAllByMemberAsync(_diaryActivityId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_diaryActivityName, result!.Name);
            Assert.Equal(_diaryActivityDesc, result!.Description);
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new DiaryActivityAddReq
            {
                Name = "newDiaryActivity",
                Description = "newDiaryActivityDesc"
            };

            // Act
            var result = await _diaryActivityService.AddAsync(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(req.Name, result!.Name);
            Assert.Equal(req.Description, result!.Description);
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new DiaryActivityAddReq
            {
                Name = "updateDiaryActivity",
                Description = "updateDiaryActivityDesc"
            };

            // Act
            var result = await _diaryActivityService.UpdateAsync(_diaryActivityId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(req.Name, result!.Name);
            Assert.Equal(req.Description, result!.Description);
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _diaryActivityService.DeleteAsync(_diaryActivityId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<DiaryActivityNotFoundException>(async () =>
            {
                await _diaryActivityService.GetAllByMemberAsync(_diaryActivityId);
            });
        }
    }
}
