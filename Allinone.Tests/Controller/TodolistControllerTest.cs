using Allinone.API.Controllers;
using Allinone.BLL;
using Allinone.BLL.Todolists;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.Domain.Enums;
using Allinone.Domain.Todolists;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Controller
{
    public class TodolistControllerTest
    {
        private readonly TodolistController _todolistController;

        private readonly int _memberId = 1;

        private readonly int _todolistId = 1;
        private readonly string _todolistName = "todolistA";
        private readonly string _todolistName2 = "todolistB";

        public TodolistControllerTest()
        {
            BaseBLL.MemberId = _memberId;

            var services = new ServiceCollection();

            // Setup InMemory DbContext with preset data
            var options = new DbContextOptionsBuilder<DSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new DSContext(options);

            context.Todolist.AddRange(
                new Todolist { ID = 1, Name = _todolistName, CategoryID = (int)EnumTodolistType.Monthly, MemberID = 1 },
                new Todolist { ID = 2, Name = _todolistName2, CategoryID = (int)EnumTodolistType.Monthly, MemberID = 1 }
            );

            context.TodolistDone.AddRange(
                new TodolistDone { ID = 1, TodolistID = 2, UpdateDate = DateTime.UtcNow }
            );
            context.SaveChanges();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IMapModel, MapModel>();
            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheHelper>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();
            var memoryCacheHelper = services.BuildServiceProvider().GetRequiredService<MemoryCacheHelper>();

            var todolistRepository = new TodolistRepository(context);

            var todolistService = new TodolistService(todolistRepository, memoryCacheHelper, mapModel);

            _todolistController = new TodolistController(todolistService);
        }

        [Fact]
        public async Task GetTodolistsUndone_ReturnsSuccess()
        {
            // Act
            var result = await _todolistController.GetTodolistsUndone();

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            Assert.Equal(_todolistName, ((IEnumerable<TodolistDto>)clinetResult.Value).FirstOrDefault().Name);
            Assert.Equal(EnumTodolistType.Monthly.ToString(), ((IEnumerable<TodolistDto>)clinetResult.Value).FirstOrDefault().CategoryName);
        }

        [Fact]
        public async Task GetTodolistsUndone_ReturnsNoRecords()
        {
            //Assign
            BaseBLL.MemberId = 2;

            // Act
            var result = await _todolistController.GetTodolistsUndone();

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            Assert.Equal(0, ((IEnumerable<TodolistDto>)clinetResult.Value).Count());
        }

        [Fact]
        public async Task Get_ReturnsSuccess()
        {
            // Act
            var result = await _todolistController.Get(_todolistId);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            Assert.Equal(_todolistName, ((Todolist)clinetResult.Value).Name);
        }

        [Fact]
        public async Task Add_ReturnsSuccess()
        {
            // Arrange
            var req = new TodolistAddReq
            {
                Name = "newTodolist",
                CategoryId = (int)EnumTodolistType.Normal
            };

            // Act
            var result = await _todolistController.Add(req);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            Assert.Equal("newTodolist", ((Todolist)clinetResult.Value).Name);
        }

        [Fact]
        public async Task Update_ReturnsSuccess()
        {
            // Assign
            var req = new TodolistAddReq
            {
                Name = "updatedTodolist",
                CategoryId = (int)EnumTodolistType.Monthly
            };

            // Act
            var result = await _todolistController.Add(req);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            Assert.Equal("updatedTodolist", ((Todolist)clinetResult.Value).Name);
            Assert.Equal((int)EnumTodolistType.Monthly, ((Todolist)clinetResult.Value).CategoryID);
        }

        [Fact]
        public async Task Delete_ReturnsSuccess()
        {
            // Act
            var result = await _todolistController.Delete(_todolistId);

            // Assert
            var clinetResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clinetResult.StatusCode);

            Assert.Equal(_todolistName, ((Todolist)clinetResult.Value).Name);
        }
    }
}
