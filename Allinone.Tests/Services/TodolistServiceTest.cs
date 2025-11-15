using Allinone.BLL;
using Allinone.BLL.Auditlogs;
using Allinone.BLL.Members;
using Allinone.BLL.Todolists;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.Domain.Enums;
using Allinone.Domain.Members;
using Allinone.Domain.Todolists;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Services
{
    public class TodolistServiceTest
    {
        private readonly IMemberService _memberService;
        private readonly TodolistService _todolistService;

        private readonly int _memberId = 1;

        private readonly string _name = "user";
        private readonly string _password = "user";

        private readonly int _todolistId = 1;
        private readonly string _todolistName = "todolistA";
        private readonly string _todolistName2 = "todolistB";

        public TodolistServiceTest()
        {
            BaseBLL.MemberId = _memberId;

            var services = new ServiceCollection();

            // Setup InMemory DbContext with preset data
            var options = new DbContextOptionsBuilder<DSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new DSContext(options);

            context.Member.AddRange(
                new Member { ID = 1, Name = _name, Password = _password }
            );

            context.Todolist.AddRange(
                new Todolist { ID = 1, Name = _todolistName, CategoryID = (int)EnumTodolistType.Monthly, MemberID = 1 },
                new Todolist { ID = 2, Name = _todolistName, CategoryID = (int)EnumTodolistType.Monthly, MemberID = 1 }
            );
            context.SaveChanges();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IMapModel, MapModel>();
            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheHelper>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();
            var memoryCacheHelper = services.BuildServiceProvider().GetRequiredService<MemoryCacheHelper>();

            var todolistRepository = new TodolistRepository(context);
            var memberRepository = new MemberRepository(context);
            var auditlogRepository = new AuditlogRepository(context);
            var auditlogService = new AuditlogService(auditlogRepository, mapModel);

            _memberService = new MemberService(auditlogService, memberRepository, mapModel);
            _todolistService = new TodolistService(auditlogService, todolistRepository, memoryCacheHelper, mapModel);
        }

        [Fact]
        public async Task GetUndone_Returns_Success()
        {
            // Act
            var result = _todolistService.GetTodolistsUndone();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_todolistName, result!.FirstOrDefault().Name);
            Assert.Equal(EnumTodolistType.Monthly.ToString(), result!.FirstOrDefault().CategoryName);
        }

        [Fact]
        public async Task GetUndone_Returns_NoRecords()
        {
            //Assign
            BaseBLL.MemberId = 2;

            // Act
            var result = _todolistService.GetTodolistsUndone();

            // Assert
            Assert.Equal(0, result!.Count());
        }

        [Fact]
        public async Task Get_Returns_Success()
        {
            // Act
            var result = await _todolistService.Get(_todolistId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_todolistName, result!.Name);
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new TodolistAddReq
            {
                Name = "newTodolist",
                CategoryId = (int)EnumTodolistType.Normal
            };

            var result2 = await _todolistService.Add(req);

            // Assert
            Assert.NotNull(result2);
            Assert.Equal("newTodolist", result2!.Name);
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new TodolistAddReq
            {
                Name = "updatedTodolist",
                CategoryId = (int)EnumTodolistType.Monthly
            };

            var result = await _todolistService.Update(1, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result!.ID);
            Assert.Equal("updatedTodolist", result!.Name);
            Assert.Equal((int)EnumTodolistType.Monthly, result!.CategoryID);
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _todolistService.Delete(_todolistId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_todolistName, result!.Name);
        }
    }
}
