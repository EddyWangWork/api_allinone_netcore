using Allinone.BLL;
using Allinone.BLL.Diarys;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.Domain.Diarys.DiaryTypes;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Services
{
    public class DiaryTypeServiceTest
    {
        private readonly DiaryTypeService _diaryTypeService;

        private readonly int _memberId = 1;

        private readonly int _diaryTypeId = 1;
        private readonly string _diaryTypeName = "diaryTypeName";
        private readonly string _diaryTypeDesc = "diaryTypeDesc";

        public DiaryTypeServiceTest()
        {
            BaseBLL.MemberId = _memberId;

            var services = new ServiceCollection();

            // Setup InMemory DbContext with preset data
            var options = new DbContextOptionsBuilder<DSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new DSContext(options);

            context.DiaryType.AddRange(
                new DiaryType
                { ID = 1, Name = _diaryTypeName, Description = _diaryTypeDesc, MemberID = _memberId }
            );
            context.SaveChanges();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IMapModel, MapModel>();
            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheHelper>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();
            var memoryCacheHelper = services.BuildServiceProvider().GetRequiredService<MemoryCacheHelper>();

            var diaryTypeRepository = new DiaryTypeRepository(context);

            _diaryTypeService = new DiaryTypeService(diaryTypeRepository, memoryCacheHelper, mapModel);
        }

        [Fact]
        public async Task GetAllByMember_Returns_Success()
        {
            // Act
            var result = await _diaryTypeService.GetAllByMemberAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_diaryTypeName, result!.FirstOrDefault().Name);
            Assert.Equal(_diaryTypeDesc, result!.FirstOrDefault().Description);
        }

        [Fact]
        public async Task GetAllByMember_id_Returns_Success()
        {
            // Act
            var result = await _diaryTypeService.GetAllByMemberAsync(_diaryTypeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_diaryTypeName, result!.Name);
            Assert.Equal(_diaryTypeDesc, result!.Description);
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new DiaryTypeAddReq
            {
                Name = "new name",
                Description = "new desc"
            };

            // Act
            var result = await _diaryTypeService.AddAsync(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(req.Name, result!.Name);
            Assert.Equal(req.Description, result!.Description);
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new DiaryTypeAddReq
            {
                Name = "update name",
                Description = "update desc"
            };

            // Act
            var result = await _diaryTypeService.UpdateAsync(_diaryTypeId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(req.Name, result!.Name);
            Assert.Equal(req.Description, result!.Description);
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _diaryTypeService.DeleteAsync(_diaryTypeId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<DiaryTypeNotFoundException>(async () =>
            {
                await _diaryTypeService.GetAllByMemberAsync(_diaryTypeId);
            });
        }
    }
}
