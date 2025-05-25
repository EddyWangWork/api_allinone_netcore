using Allinone.API.Controllers;
using Allinone.BLL.Todolists;
using Allinone.BLL;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.Domain.Enums;
using Allinone.Domain.Todolists;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Allinone.Domain.Exceptions;

namespace Allinone.Tests.Controller
{
    public class TodolistDoneControllerTest
    {
        private readonly TodolistDoneController _todolistDoneController;

        private readonly int _memberId = 1;

        private readonly string _name = "user";
        private readonly string _password = "user";

        private readonly int _todolistId = 1;
        private readonly string _todolistName = "todolistA";
        private readonly string _todolistName2 = "todolistB";

        private readonly int _todolistDoneId = 1;

        public TodolistDoneControllerTest()
        {
            BaseBLL.MemberId = _memberId;

            var services = new ServiceCollection();

            // Setup InMemory DbContext with preset data
            var options = new DbContextOptionsBuilder<DSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new DSContext(options);

            context.Todolist.AddRange(
                new Todolist { ID = 1, Name = _todolistName, CategoryID = (int)EnumTodolistType.Normal, MemberID = 1 },
                new Todolist { ID = 2, Name = _todolistName2, CategoryID = (int)EnumTodolistType.Normal, MemberID = 1 }
            );

            context.TodolistDone.AddRange(
                new TodolistDone { ID = _todolistDoneId, TodolistID = _todolistId, UpdateDate = DateTime.UtcNow }
            );
            context.SaveChanges();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IMapModel, MapModel>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();

            var todolistDoneRepository = new TodolistDoneRepository(context);

            var todolistDoneService = new TodolistDoneService(todolistDoneRepository, mapModel);

            _todolistDoneController = new TodolistDoneController(todolistDoneService);
        }

        [Fact]
        public async Task GetTodolistDone_ReturnsSuccess()
        {
            // Act
            var result = await _todolistDoneController.GetAsync();

            // Assert
            var clientResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clientResult.StatusCode);

            Assert.Equal(_todolistName, ((IEnumerable<TodolistDoneDto>)clientResult.Value).FirstOrDefault().TodolistName);
        }

        [Fact]
        public async Task GetTodolistDone_ReturnsNoRecord()
        {
            //Assign
            BaseBLL.MemberId = 2;

            // Act
            var result = await _todolistDoneController.GetAsync();

            // Assert
            var clientResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clientResult.StatusCode);

            Assert.Equal(0, ((IEnumerable<TodolistDoneDto>)clientResult.Value).Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsSuccess()
        {
            // Act
            var result = await _todolistDoneController.GetByIdAsync(_todolistDoneId);

            // Assert
            var clientResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clientResult.StatusCode);

            Assert.Equal(_todolistId, ((TodolistDone)clientResult.Value).TodolistID);
        }

        [Fact]
        public async Task Add_ReturnsSuccess()
        {
            // Arrange
            var req = new TodolistDoneAddReq
            {
                TodolistID = 2
            };

            // Act
            var result = await _todolistDoneController.Add(req);

            // Assert
            var clientResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clientResult.StatusCode);

            Assert.Equal(2, ((TodolistDone)clientResult.Value).TodolistID);
        }

        [Fact]
        public async Task Add_ReturnsNotFound()
        {
            // Assign
            var req = new TodolistDoneAddReq
            {
                TodolistID = 3
            };

            // Act & Assert
            await Assert.ThrowsAsync<TodolistNotFoundException>(async () =>
            {
                await _todolistDoneController.Add(req);
            });
        }

        [Fact]
        public async Task Add_ReturnsAlreadyDone()
        {
            // Assign
            var req = new TodolistDoneAddReq
            {
                TodolistID = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<TodolistAlreadyDoneException>(async () =>
            {
                await _todolistDoneController.Add(req);
            });
        }

        [Fact]
        public async Task Update_ReturnsSuccess()
        {
            // Assign
            var req = new TodolistDoneAddReq
            {
                Remark = "updatedTodolistDone"
            };

            // Act
            var result = await _todolistDoneController.Update(_todolistDoneId, req);

            // Assert
            var clientResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, clientResult.StatusCode);

            Assert.Equal(_todolistDoneId, ((TodolistDone)clientResult.Value).ID);
            Assert.Equal("updatedTodolistDone", ((TodolistDone)clientResult.Value).Remark);
        }

        [Fact]
        public async Task Delete_ReturnsSuccess()
        {
            // Act
            var result = await _todolistDoneController.Delete(_todolistDoneId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            Assert.Equal(_todolistId, ((TodolistDone)okResult.Value).TodolistID);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound()
        {
            // Act & Assert
            await Assert.ThrowsAsync<TodolistDoneNotFoundException>(async () =>
            {
                await _todolistDoneController.Delete(2);
            });
        }
    }
}
