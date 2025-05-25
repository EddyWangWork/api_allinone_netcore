using Allinone.BLL;
using Allinone.BLL.Trips;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.DLL.UnitOfWork;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Trips;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Services
{
    public class TripDetailTypeServiceTest
    {
        private readonly TripDetailTypeService _tripDetailTypeService;

        private readonly int _memberId = 1;

        private readonly int _tripDetailTypeId = 1;
        private readonly string _tripDetailTypeName = "tripDetailTypeName";

        private readonly int _tripDetailTypeId2 = 2;
        private readonly string _tripDetailTypeName2 = "tripDetailTypeName2";

        public TripDetailTypeServiceTest()
        {
            BaseBLL.MemberId = _memberId;

            var services = new ServiceCollection();

            var options = new DbContextOptionsBuilder<DSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var context = new DSContext(options);

            context.TripDetailType.AddRange(
                new TripDetailType
                {
                    ID = _tripDetailTypeId,
                    Name = _tripDetailTypeName
                },
                new TripDetailType
                {
                    ID = _tripDetailTypeId2,
                    Name = _tripDetailTypeName2
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

            var tripDetailTypeRepository = new TripDetailTypeRepository(context);

            _tripDetailTypeService = new TripDetailTypeService(tripDetailTypeRepository, mapModel);
        }

        [Fact]
        public async Task GetAllAsync_Returns_Success()
        {
            // Act
            var result = await _tripDetailTypeService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new TripDetailTypeAddReq
            {
                Name = "new TripDetailType"
            };

            // Act
            var result = await _tripDetailTypeService.Add(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result!.ID);
            Assert.Equal(req.Name, result!.Name);
        }

        [Fact]
        public async Task Update_Returns_TripDetailTypeNotFoundException_Failed()
        {
            // Assign
            var req = new TripDetailTypeAddReq
            {
                Name = "updated TripDetailType"
            };

            // Act & Assert
            await Assert.ThrowsAsync<TripDetailTypeNotFoundException>(async () =>
            {
                await _tripDetailTypeService.Update(3, req);
            });
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new TripDetailTypeAddReq
            {
                Name = "updated TripDetailType"
            };

            // Act
            var result = await _tripDetailTypeService.Update(_tripDetailTypeId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_tripDetailTypeId, result!.ID);
            Assert.Equal(req.Name, result!.Name);
        }

        [Fact]
        public async Task Delete_Returns_Failed()
        {
            // Act & Assert
            await Assert.ThrowsAsync<TripDetailTypeNotFoundException>(async () =>
            {
                await _tripDetailTypeService.Delete(3);
            });
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _tripDetailTypeService.Delete(_tripDetailTypeId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<TripDetailTypeNotFoundException>(async () =>
            {
                await _tripDetailTypeService.Get(_tripDetailTypeId);
            });
        }
    }
}
