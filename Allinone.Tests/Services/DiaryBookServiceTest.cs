using Allinone.BLL;
using Allinone.BLL.Diarys;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.Domain.Diarys.DiaryBooks;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Services
{
    public class DiaryBookServiceTest
    {
        private readonly DiaryBookService _diaryBookService;

        private readonly int _memberId = 1;

        private readonly int _diaryBookId = 1;
        private readonly string _diaryBookName = "diaryBookName";
        private readonly string _diaryBookDesc = "diaryBookDesc";

        public DiaryBookServiceTest()
        {
            BaseBLL.MemberId = _memberId;

            var services = new ServiceCollection();

            // Setup InMemory DbContext with preset data
            var options = new DbContextOptionsBuilder<DSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new DSContext(options);

            context.DiaryBook.AddRange(
                new DiaryBook
                { ID = 1, Name = _diaryBookName, Description = _diaryBookDesc, MemberID = _memberId }
            );
            context.SaveChanges();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IMapModel, MapModel>();
            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheHelper>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();
            var memoryCacheHelper = services.BuildServiceProvider().GetRequiredService<MemoryCacheHelper>();

            var diaryBookRepository = new DiaryBookRepository(context);

            _diaryBookService = new DiaryBookService(diaryBookRepository, memoryCacheHelper, mapModel);
        }

        [Fact]
        public async Task GetAllByMember_Returns_Success()
        {
            // Act
            var result = await _diaryBookService.GetAllByMemberAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_diaryBookName, result!.FirstOrDefault().Name);
            Assert.Equal(_diaryBookDesc, result!.FirstOrDefault().Description);
        }

        [Fact]
        public async Task GetAllByMember_id_Returns_Success()
        {
            // Act
            var result = await _diaryBookService.GetAllByMemberAsync(_diaryBookId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_diaryBookName, result!.Name);
            Assert.Equal(_diaryBookDesc, result!.Description);
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new DiaryBookAddReq
            {
                Name = "new name",
                Description = "new desc"
            };

            // Act
            var result = await _diaryBookService.AddAsync(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(req.Name, result!.Name);
            Assert.Equal(req.Description, result!.Description);
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new DiaryBookAddReq
            {
                Name = "update name",
                Description = "update desc"
            };

            // Act
            var result = await _diaryBookService.UpdateAsync(_diaryBookId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(req.Name, result!.Name);
            Assert.Equal(req.Description, result!.Description);
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _diaryBookService.DeleteAsync(_diaryBookId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<DiaryBookNotFoundException>(async () =>
            {
                await _diaryBookService.GetAllByMemberAsync(_diaryBookId);
            });
        }
    }
}
