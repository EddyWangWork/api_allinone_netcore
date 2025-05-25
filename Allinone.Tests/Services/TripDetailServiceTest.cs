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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.Tests.Services
{
    public class TripDetailServiceTest
    {
        private readonly TripDetailService _tripDetailService;

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

        public TripDetailServiceTest()
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
            var tripDetailTypeRepository = new TripDetailTypeRepository(context);
            var tripDetailRepository = new TripDetailRepository(context);

            _tripDetailService = new TripDetailService(
                tripRepository,
                tripDetailTypeRepository,
                tripDetailRepository,
                mapModel);
        }

        [Fact]
        public async Task GetAllAsync_Returns_Success()
        {
            // Act
            var result = await _tripDetailService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAsync_Returns_Success()
        {
            // Act
            var result = await _tripDetailService.Get(_tripDetailId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_tripDetailId, result!.ID);
            Assert.Equal(_tripDetailName, result!.Name);
            Assert.Equal(_tripDetailLinkName, result!.LinkName);
            Assert.Equal(_tripDetailDate, result!.Date);
            Assert.Equal(_tripId, result!.TripID);
            Assert.Equal(_tripDetailTypeId, result!.TripDetailTypeID);
        }

        [Fact]
        public async Task Add_Returns_TripBadRequestException_Failed()
        {
            // Assign
            var req = new TripDetailAddReq
            {
                Name = "new TripDetail",
                LinkName = "new LinkName",
                Date = _tripDetailDate,
                TripID = 3,
                TripDetailTypeID = _tripDetailTypeId
            };

            // Act & Assert
            await Assert.ThrowsAsync<TripBadRequestException>(async () =>
            {
                await _tripDetailService.Add(req);
            });
        }

        [Fact]
        public async Task Add_Returns_TripDetailTypeBadRequestException_Failed()
        {
            // Assign
            var req = new TripDetailAddReq
            {
                Name = "new TripDetail",
                LinkName = "new LinkName",
                Date = _tripDetailDate,
                TripID = _tripId,
                TripDetailTypeID = 3
            };

            // Act & Assert
            await Assert.ThrowsAsync<TripDetailTypeBadRequestException>(async () =>
            {
                await _tripDetailService.Add(req);
            });
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Assign
            var req = new TripDetailAddReq
            {
                Name = "new TripDetail",
                LinkName = "new LinkName",
                Date = _tripDetailDate,
                TripID = _tripId,
                TripDetailTypeID = _tripDetailTypeId
            };

            // Act
            var result = await _tripDetailService.Add(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result!.ID);
            Assert.Equal(req.Name, result!.Name);
            Assert.Equal(req.LinkName, result!.LinkName);
            Assert.Equal(req.Date, result!.Date);
            Assert.Equal(req.TripID, result!.TripID);
            Assert.Equal(req.TripDetailTypeID, result!.TripDetailTypeID);
        }

        [Fact]
        public async Task Update_Returns_TripBadRequestException_Failed()
        {
            // Assign
            var req = new TripDetailAddReq
            {
                Name = "updated TripDetail",
                LinkName = "updated LinkName",
                Date = _tripDetailDate,
                TripID = 3,
                TripDetailTypeID = _tripDetailTypeId
            };

            // Act & Assert
            await Assert.ThrowsAsync<TripBadRequestException>(async () =>
            {
                await _tripDetailService.Update(_tripDetailId, req);
            });
        }

        [Fact]
        public async Task Update_Returns_TripDetailTypeBadRequestException_Failed()
        {
            // Assign
            var req = new TripDetailAddReq
            {
                Name = "updated TripDetail",
                LinkName = "updated LinkName",
                Date = _tripDetailDate,
                TripID = _tripId,
                TripDetailTypeID = 3
            };

            // Act & Assert
            await Assert.ThrowsAsync<TripDetailTypeBadRequestException>(async () =>
            {
                await _tripDetailService.Update(_tripDetailId, req);
            });
        }

        [Fact]
        public async Task Update_Returns_TripDetailNotFoundException_Failed()
        {
            // Assign
            var req = new TripDetailAddReq
            {
                Name = "updated TripDetail",
                LinkName = "updated LinkName",
                Date = _tripDetailDate,
                TripID = _tripId,
                TripDetailTypeID = _tripDetailTypeId
            };

            // Act & Assert
            await Assert.ThrowsAsync<TripDetailNotFoundException>(async () =>
            {
                await _tripDetailService.Update(3, req);
            });
        }

        [Fact]
        public async Task Update_Returns_Success()
        {
            // Assign
            var req = new TripDetailAddReq
            {
                Name = "updated TripDetail",
                LinkName = "updated LinkName",
                Date = _tripDetailDate,
                TripID = _tripId,
                TripDetailTypeID = _tripDetailTypeId
            };

            // Act
            var result = await _tripDetailService.Update(_tripDetailId, req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_tripDetailId, result!.ID);
            Assert.Equal(req.Name, result!.Name);
            Assert.Equal(req.LinkName, result!.LinkName);
            Assert.Equal(req.Date, result!.Date);
            Assert.Equal(req.TripID, result!.TripID);
            Assert.Equal(req.TripDetailTypeID, result!.TripDetailTypeID);
        }

        [Fact]
        public async Task Delete_Returns_Failed()
        {
            // Act & Assert
            await Assert.ThrowsAsync<TripDetailNotFoundException>(async () =>
            {
                await _tripDetailService.Delete(3);
            });
        }

        [Fact]
        public async Task Delete_Returns_Success()
        {
            // Act
            var result = await _tripDetailService.Delete(_tripDetailId);

            // Assert
            Assert.NotNull(result);

            // Act & Assert
            await Assert.ThrowsAsync<TripDetailNotFoundException>(async () =>
            {
                await _tripDetailService.Get(_tripDetailId);
            });
        }
    }
}
