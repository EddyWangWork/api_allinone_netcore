using Allinone.BLL;
using Allinone.BLL.Auditlogs;
using Allinone.BLL.Members;
using Allinone.BLL.Todolists;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.Domain.Enums;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Todolists;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.Tests.Services
{
    public class TodolistDoneServiceTest
    {
        private readonly TodolistDoneService _todolistDoneService;

        private readonly int _memberId = 1;

        private readonly int _todolistId = 1;
        private readonly string _todolistName = "todolistA";
        private readonly string _todolistName2 = "todolistB";

        private readonly int _todolistDoneId = 1;

        public TodolistDoneServiceTest()
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
                new TodolistDone { ID = _todolistDoneId, TodolistID = _todolistId, UpdateDate = DateTime.UtcNow.AddHours(8) }
            );
            context.SaveChanges();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IMapModel, MapModel>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();

            var todolistDoneRepository = new TodolistDoneRepository(context);

            var auditlogRepository = new AuditlogRepository(context);
            var auditlogService = new AuditlogService(auditlogRepository, mapModel);

            _todolistDoneService = new TodolistDoneService(auditlogService, todolistDoneRepository, mapModel);
        }

        [Fact]
        public async Task Get_Returns_Success()
        {
            var result = await _todolistDoneService.GetAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_todolistName, result!.FirstOrDefault().TodolistName);
        }

        [Fact]
        public async Task Get_Returns_NoRecords()
        {
            BaseBLL.MemberId = 2;

            var result = await _todolistDoneService.GetAsync();

            // Assert
            Assert.Equal(0, result!.Count());
        }

        [Fact]
        public async Task GetByIdAsync_Returns_Success()
        {
            var result = await _todolistDoneService.GetByIdAsync(_todolistDoneId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_todolistId, result!.TodolistID);
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new TodolistDoneAddReq
            {
                TodolistID = 2
            };

            var result = await _todolistDoneService.Add(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result!.TodolistID);
        }

        [Fact]
        public async Task Add_Returns_NotFound()
        {
            // Assign
            var req = new TodolistDoneAddReq
            {
                TodolistID = 3
            };

            // Act & Assert
            await Assert.ThrowsAsync<TodolistNotFoundException>(async () =>
            {
                await _todolistDoneService.Add(req);
            });
        }

        [Fact]
        public async Task Add_Returns_AlreadyDone()
        {
            // Assign
            var req = new TodolistDoneAddReq
            {
                TodolistID = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<TodolistAlreadyDoneException>(async () =>
            {
                await _todolistDoneService.Add(req);
            });
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new TodolistDoneAddReq
            {
                Remark = "updatedTodolistDone"
            };

            var result = await _todolistDoneService.Update(_todolistDoneId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_todolistDoneId, result!.ID);
            Assert.Equal("updatedTodolistDone", result!.Remark);
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _todolistDoneService.Delete(_todolistDoneId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Delete_Returns_NotFound()
        {
            // Act & Assert
            await Assert.ThrowsAsync<TodolistDoneNotFoundException>(async () =>
            {
                await _todolistDoneService.Delete(2);
            });
        }
    }
}
