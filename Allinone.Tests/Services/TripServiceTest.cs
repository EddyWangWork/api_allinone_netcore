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
    public class TripServiceTest
    {
        private readonly TripService _tripService;

        private readonly int _memberId = 1;

        private readonly int _tripDetailTypeId = 1;
        private readonly string _tripDetailTypeName = "tripDetailTypeName";

        private readonly int _tripDetailTypeId2 = 2;
        private readonly string _tripDetailTypeName2 = "tripDetailTypeName2";

        private readonly int _tripId = 1;
        private readonly string _tripName = "tripName";
        private readonly DateTime _tripFromDate = DateTime.UtcNow.AddHours(8);
        private readonly DateTime _tripToDate = DateTime.UtcNow.AddHours(8).AddDays(1);

        private readonly int _tripId2 = 2;
        private readonly string _tripName2 = "tripName2";
        private readonly DateTime _tripFromDate2 = DateTime.UtcNow.AddHours(8);
        private readonly DateTime _tripToDate2 = DateTime.UtcNow.AddHours(8).AddDays(1);

        private readonly int _tripDetailId = 1;
        private readonly string _tripDetailName = "tripDetailName";
        private readonly DateTime _tripDetailDate = DateTime.UtcNow.AddHours(8);
        private readonly string _tripDetailLinkName = "tripDetailLinkName";

        private readonly int _tripDetailId2 = 2;
        private readonly string _tripDetailName2 = "tripDetailName2";
        private readonly DateTime _tripDetailDate2 = DateTime.UtcNow.AddHours(8);
        private readonly string _tripDetailLinkName2 = "tripDetailLinkName2";

        public TripServiceTest()
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

            context.Trip.AddRange(
                new Trip
                {
                    ID = _tripId,
                    Name = _tripName,
                    FromDate = _tripFromDate,
                    ToDate = _tripToDate
                },
                new Trip
                {
                    ID = _tripId2,
                    Name = _tripName2,
                    FromDate = _tripFromDate2,
                    ToDate = _tripToDate2
                }
            );

            context.TripDetail.AddRange(
                new TripDetail
                {
                    ID = _tripId,
                    Name = _tripDetailName,
                    TripID = _tripId,
                    TripDetailTypeID = _tripDetailTypeId,
                    Date = _tripDetailDate,
                    LinkName = _tripDetailLinkName
                },
                new TripDetail
                {
                    ID = _tripId2,
                    Name = _tripDetailName2,
                    TripID = _tripId2,
                    TripDetailTypeID = _tripDetailTypeId2,
                    Date = _tripDetailDate2,
                    LinkName = _tripDetailLinkName2
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

            var tripRepository = new TripRepository(context);

            _tripService = new TripService(tripRepository, mapModel);
        }

        [Fact]
        public async Task GetAllDetailsV2Async_Returns_Success()
        {
            // Act
            var result = await _tripService.GetAllDetailsV2Async();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAllAsync_Returns_Success()
        {
            // Act
            var result = await _tripService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new TripAddReq
            {
                Name = "new Trip",
                FromDate = DateTime.UtcNow.AddHours(8),
                ToDate = DateTime.UtcNow.AddHours(8).AddDays(1),
            };

            // Act
            var result = await _tripService.Add(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result!.ID);
            Assert.Equal(req.Name, result!.Name);
            Assert.Equal(req.FromDate, result!.FromDate);
            Assert.Equal(req.ToDate, result!.ToDate);
        }

        [Fact]
        public async Task Update_Returns_TripNotFoundException_Failed()
        {
            // Assign
            var req = new TripAddReq
            {
                Name = "updated Trip",
                FromDate = DateTime.UtcNow.AddHours(8),
                ToDate = DateTime.UtcNow.AddHours(8).AddDays(1),
            };

            // Act & Assert
            await Assert.ThrowsAsync<TripNotFoundException>(async () =>
            {
                await _tripService.Update(3, req);
            });
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new TripAddReq
            {
                Name = "updated Trip",
                FromDate = DateTime.UtcNow.AddHours(8),
                ToDate = DateTime.UtcNow.AddHours(8).AddDays(1),
            };

            // Act
            var result = await _tripService.Update(_tripId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_tripId, result!.ID);
            Assert.Equal(req.Name, result!.Name);
            Assert.Equal(req.FromDate, result!.FromDate);
            Assert.Equal(req.ToDate, result!.ToDate);
        }

        [Fact]
        public async Task Delete_Returns_Failed()
        {
            // Act & Assert
            await Assert.ThrowsAsync<TripNotFoundException>(async () =>
            {
                await _tripService.Delete(3);
            });
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _tripService.Delete(_tripId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<TripNotFoundException>(async () =>
            {
                await _tripService.Get(_tripId);
            });
        }
    }
}
