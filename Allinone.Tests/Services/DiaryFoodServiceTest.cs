using Allinone.BLL;
using Allinone.BLL.Diarys;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.Domain.Diarys.DiaryFoods;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Services
{
    public class DiaryFoodServiceTest
    {
        private readonly DiaryFoodService _diaryFoodService;

        private readonly int _memberId = 1;

        private readonly int _diaryFoodId = 1;
        private readonly string _diaryFoodName = "diaryFoodName";
        private readonly string _diaryFoodDesc = "diaryFoodDesc";

        public DiaryFoodServiceTest()
        {
            BaseBLL.MemberId = _memberId;

            var services = new ServiceCollection();

            // Setup InMemory DbContext with preset data
            var options = new DbContextOptionsBuilder<DSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new DSContext(options);

            context.DiaryFood.AddRange(
                new DiaryFood
                { ID = 1, Name = _diaryFoodName, Description = _diaryFoodDesc, MemberID = _memberId }
            );
            context.SaveChanges();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IMapModel, MapModel>();
            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheHelper>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();
            var memoryCacheHelper = services.BuildServiceProvider().GetRequiredService<MemoryCacheHelper>();

            var diaryFoodRepository = new DiaryFoodRepository(context);

            _diaryFoodService = new DiaryFoodService(diaryFoodRepository, memoryCacheHelper, mapModel);
        }

        [Fact]
        public async Task GetAllByMember_Returns_Success()
        {
            // Act
            var result = await _diaryFoodService.GetAllByMemberAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_diaryFoodName, result!.FirstOrDefault().Name);
            Assert.Equal(_diaryFoodDesc, result!.FirstOrDefault().Description);
        }

        [Fact]
        public async Task GetAllByMember_id_Returns_Success()
        {
            // Act
            var result = await _diaryFoodService.GetAllByMemberAsync(_diaryFoodId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_diaryFoodName, result!.Name);
            Assert.Equal(_diaryFoodDesc, result!.Description);
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new DiaryFoodAddReq
            {
                Name = "new name",
                Description = "new desc"
            };

            // Act
            var result = await _diaryFoodService.AddAsync(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(req.Name, result!.Name);
            Assert.Equal(req.Description, result!.Description);
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new DiaryFoodAddReq
            {
                Name = "update name",
                Description = "update desc"
            };

            // Act
            var result = await _diaryFoodService.UpdateAsync(_diaryFoodId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(req.Name, result!.Name);
            Assert.Equal(req.Description, result!.Description);
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _diaryFoodService.DeleteAsync(_diaryFoodId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<DiaryFoodNotFoundException>(async () =>
            {
                await _diaryFoodService.GetAllByMemberAsync(_diaryFoodId);
            });
        }
    }
}
