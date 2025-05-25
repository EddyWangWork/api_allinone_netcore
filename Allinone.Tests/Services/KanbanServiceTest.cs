using Allinone.BLL.DS.DSItems;
using Allinone.BLL;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.DLL.UnitOfWork;
using Allinone.Domain.DS.DSItems;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Allinone.Domain.Kanbans;
using Allinone.BLL.Kanbans;
using Allinone.Domain.Exceptions;

namespace Allinone.Tests.Services
{
    public class KanbanServiceTest
    {
        private readonly KanbanService _kanbanService;

        private readonly int _memberId = 1;

        private readonly int _kanbanId = 1;
        private readonly int _kanbanType = 1;
        private readonly string _kanbanTitle = "kanbanTitle";
        private readonly string _kanbanContent = "kanbanContent";
        private readonly int _kanbanStatus = 1;
        private readonly int _kanbanPriority = 0;
        private readonly DateTime _kanbanUpdatedDatetime = DateTime.UtcNow.AddHours(8);

        private readonly int _kanbanId2 = 2;
        private readonly int _kanbanType2 = 2;
        private readonly string _kanbanTitle2 = "kanbanTitle2";
        private readonly string _kanbanContent2 = "kanbanContent2";
        private readonly int _kanbanStatus2 = 2;
        private readonly int _kanbanPriority2 = 1;
        private readonly DateTime _kanbanUpdatedDatetime2 = DateTime.UtcNow.AddHours(8);

        public KanbanServiceTest()
        {
            BaseBLL.MemberId = _memberId;

            var services = new ServiceCollection();

            var options = new DbContextOptionsBuilder<DSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var context = new DSContext(options);

            context.Kanban.AddRange(
                new Kanban
                {
                    ID = _kanbanId,
                    Content = _kanbanContent,
                    Priority = _kanbanPriority,
                    Status = _kanbanStatus,
                    Title = _kanbanTitle,
                    Type = _kanbanType,
                    UpdatedTime = _kanbanUpdatedDatetime,
                    MemberID = _memberId
                },
                new Kanban
                {
                    ID = _kanbanId2,
                    Content = _kanbanContent2,
                    Priority = _kanbanPriority2,
                    Status = _kanbanStatus2,
                    Title = _kanbanTitle2,
                    Type = _kanbanType2,
                    UpdatedTime = _kanbanUpdatedDatetime2,
                    MemberID = 2
                }
            );
            context.SaveChanges();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IMapModel, MapModel>();
            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheHelper>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();
            var memoryCacheHelper = services.BuildServiceProvider().GetRequiredService<MemoryCacheHelper>();
            var unitOfWork = new UnitOfWork(context);

            var kanbanRepository = new KanbanRepository(context);

            _kanbanService = new KanbanService(kanbanRepository, mapModel);
        }

        [Fact]
        public async Task GetKanbans_Returns_Success()
        {
            // Act
            var result = await _kanbanService.GetKanbansAsync();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetById_Returns_Success()
        {
            // Act
            var result = await _kanbanService.Get(_kanbanId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_kanbanId, result!.ID);
            Assert.Equal(_kanbanType, result!.Type);
            Assert.Equal(_kanbanTitle, result!.Title);
            Assert.Equal(_kanbanStatus, result!.Status);
            Assert.Equal(_kanbanContent, result!.Content);
            Assert.Equal(_kanbanPriority, result!.Priority);
            Assert.NotEqual(DateTime.MinValue, result!.UpdatedTime);
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new KanbanAddReq
            {
                Type = _kanbanType,
                Title = "new kanbanTitle",
                Status = _kanbanStatus,
                Content = _kanbanContent,
                Priority = _kanbanPriority
            };

            // Act
            var result = await _kanbanService.Add(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result!.ID);
            Assert.Equal(_kanbanType, result!.Type);
            Assert.Equal("new kanbanTitle", result!.Title);
            Assert.Equal(_kanbanStatus, result!.Status);
            Assert.Equal(_kanbanContent, result!.Content);
            Assert.Equal(_kanbanPriority, result!.Priority);
            Assert.NotEqual(DateTime.MinValue, result!.UpdatedTime);
        }

        [Fact]
        public async Task Update_Returns_Failed()
        {
            // Assign
            var req = new KanbanAddReq
            {
                Type = 2,
                Title = "updated kanbanTitle",
                Status = 2,
                Content = "updated kanbanContent",
                Priority = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<KanbanNotFoundException>(async () =>
            {
                await _kanbanService.Update(_kanbanId2, req);
            });
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new KanbanAddReq
            {
                Type = 2,
                Title = "updated kanbanTitle",
                Status = 2,
                Content = "updated kanbanContent",
                Priority = 1
            };

            // Act
            var result = await _kanbanService.Update(_kanbanId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_kanbanId, result!.ID);
            Assert.Equal(2, result!.Type);
            Assert.Equal("updated kanbanTitle", result!.Title);
            Assert.Equal(2, result!.Status);
            Assert.Equal("updated kanbanContent", result!.Content);
            Assert.Equal(1, result!.Priority);
            Assert.NotEqual(DateTime.MinValue, result!.UpdatedTime);
        }

        [Fact]
        public async Task Delete_Returns_Failed()
        {
            // Act & Assert
            await Assert.ThrowsAsync<KanbanNotFoundException>(async () =>
            {
                await _kanbanService.Delete(_kanbanId2);
            });
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _kanbanService.Delete(_kanbanId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<KanbanNotFoundException>(async () =>
            {
                await _kanbanService.Get(_kanbanId);
            });
        }
    }
}
