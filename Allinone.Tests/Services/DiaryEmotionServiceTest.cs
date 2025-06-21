using Allinone.BLL;
using Allinone.BLL.Diarys;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.Domain.Diarys.DiaryEmotions;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Services
{
    public class DiaryEmotionServiceTest
    {
        private readonly DiaryEmotionService _diaryEmotionService;

        private readonly int _memberId = 1;

        private readonly int _diaryEmotionId = 1;
        private readonly string _diaryEmotionName = "diaryActivityName";
        private readonly string _diaryEmotionDesc = "diaryActivityDesc";

        public DiaryEmotionServiceTest()
        {
            BaseBLL.MemberId = _memberId;

            var services = new ServiceCollection();

            // Setup InMemory DbContext with preset data
            var options = new DbContextOptionsBuilder<DSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new DSContext(options);

            context.DiaryEmotion.AddRange(
                new DiaryEmotion
                { ID = 1, Name = _diaryEmotionName, Description = _diaryEmotionDesc, MemberID = _memberId }
            );
            context.SaveChanges();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IMapModel, MapModel>();
            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheHelper>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();
            var memoryCacheHelper = services.BuildServiceProvider().GetRequiredService<MemoryCacheHelper>();

            var diaryEmotionRepository = new DiaryEmotionRepository(context);

            _diaryEmotionService = new DiaryEmotionService(diaryEmotionRepository, memoryCacheHelper, mapModel);
        }

        [Fact]
        public async Task GetAllByMember_Returns_Success()
        {
            // Act
            var result = await _diaryEmotionService.GetAllByMemberAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_diaryEmotionName, result!.FirstOrDefault().Name);
            Assert.Equal(_diaryEmotionDesc, result!.FirstOrDefault().Description);
        }

        [Fact]
        public async Task GetAllByMember_id_Returns_Success()
        {
            // Act
            var result = await _diaryEmotionService.GetAllByMemberAsync(_diaryEmotionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_diaryEmotionName, result!.Name);
            Assert.Equal(_diaryEmotionDesc, result!.Description);
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new DiaryEmotionAddReq
            {
                Name = "new name",
                Description = "new desc"
            };

            // Act
            var result = await _diaryEmotionService.AddAsync(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(req.Name, result!.Name);
            Assert.Equal(req.Description, result!.Description);
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new DiaryEmotionAddReq
            {
                Name = "update name",
                Description = "update desc"
            };

            // Act
            var result = await _diaryEmotionService.UpdateAsync(_diaryEmotionId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(req.Name, result!.Name);
            Assert.Equal(req.Description, result!.Description);
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _diaryEmotionService.DeleteAsync(_diaryEmotionId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<DiaryEmotionNotFoundException>(async () =>
            {
                await _diaryEmotionService.GetAllByMemberAsync(_diaryEmotionId);
            });
        }
    }
}
