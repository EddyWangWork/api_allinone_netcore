using Allinone.DLL.Data;
using Allinone.Domain.Trips;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface ITripRepository
    {
        Task<IEnumerable<TripResultDto>> GetAllDetailsV2Async();

        Task<bool> IsExist(int id);
        Task<Trip>? GetAsync(int id);
        Task<IEnumerable<Trip>> GetAllAsync();
        Task Add(Trip entity);
        void Update(Trip entity);
        void Delete(Trip entity);
    }

    public class TripRepository(DSContext context) : ITripRepository
    {
        public async Task<IEnumerable<TripResultDto>> GetAllDetailsAsync()
        {
            var tripDetails = await (
             from a in context.TripDetail
             join b in context.TripDetailType on a.TripDetailTypeID equals b.ID
             join c in context.Trip on a.TripID equals c.ID
             select new
             {
                 TripName = c.Name,
                 a.Date,
                 TripDetailTypeName = b.Name,
                 TripDetailNameID = a.ID,
                 TripDetailName = a.Name,
                 TripDetailNameLink = a.LinkName
             }).ToListAsync();


            var tripResultsDto = new List<TripResultDto>();

            var tripsInfo = await context.Trip.ToListAsync();
            var tripDetailTypes = await context.TripDetailType.ToListAsync();

            var tripDistinctTypeInfos = tripDetailTypes.Select(x => new { x.ID, x.Name });
            var tripGDates = tripDetails.GroupBy(x => new { x.TripName, x.Date });

            foreach (var tripInfo in tripsInfo)
            {
                var tripsDto = new List<TripDto>();

                var diffDay = tripInfo.ToDate - tripInfo.FromDate;

                for (int i = 0; i < diffDay.Days + 1; i++)
                {
                    var tripGTypesDto = new List<TripDetailTypeDto>();

                    var tripDate = tripInfo.FromDate.AddDays(i);
                    var tripGTypes = tripGDates.FirstOrDefault(x => x.Key.TripName == tripInfo.Name && x.Key.Date == tripDate)?.GroupBy(y => y.TripDetailTypeName);

                    foreach (var tripDistinctTypeInfo in tripDistinctTypeInfos)
                    {
                        var typeValues = tripGTypes?.FirstOrDefault(x => x.Key == tripDistinctTypeInfo.Name)?.
                            Select(x => new TripDetailTypeValueDto
                            {
                                TypeValueID = x.TripDetailNameID,
                                TypeValue = x.TripDetailName,
                                TypeVTypeLink = (x.TripDetailNameLink == null || x.TripDetailNameLink == string.Empty) ? string.Empty : x.TripDetailNameLink
                            })?
                            .ToList();

                        if (tripGTypes == null || typeValues == null)
                        {
                            typeValues = new List<TripDetailTypeValueDto> { new TripDetailTypeValueDto { TypeValue = "-", TypeVTypeLink = string.Empty } };
                        }

                        tripGTypesDto.Add(new TripDetailTypeDto
                        {
                            TypeID = tripDistinctTypeInfo.ID,
                            TypeName = tripDistinctTypeInfo.Name,
                            TypeValues = typeValues,
                        });
                    }
                    tripsDto.Add(new TripDto
                    {
                        Date = tripDate,
                        TripDetailDto = new TripDetailDto { TripDetailTypesInfo = tripGTypesDto }
                    });
                }

                tripResultsDto.Add(new TripResultDto
                {
                    ID = tripInfo.ID,
                    Name = tripInfo.Name,
                    TripDtos = tripsDto
                });
            }

            return tripResultsDto;
        }

        public async Task<IEnumerable<TripResultDto>> GetAllDetailsV2Async()
        {
            // Step 1: Load data
            var tripDetails = await (
                from a in context.TripDetail
                join b in context.TripDetailType on a.TripDetailTypeID equals b.ID
                join c in context.Trip on a.TripID equals c.ID
                select new TripDetailFlatDto
                {
                    TripID = c.ID,
                    TripName = c.Name,
                    FromDate = c.FromDate,
                    ToDate = c.ToDate,
                    Date = a.Date,
                    TripDetailTypeID = b.ID,
                    TripDetailTypeName = b.Name,
                    TripDetailID = a.ID,
                    TripDetailName = a.Name,
                    TripDetailLink = a.LinkName
                }).ToListAsync();

            // Step 2: Organize trip details into dictionaries for fast access
            var tripDetailsByTrip = tripDetails
                .GroupBy(x => x.TripID)
                .ToDictionary(g => g.Key, g => g.ToList());

            var tripDetailTypes = await context.TripDetailType
                .Select(x => new { x.ID, x.Name })
                .ToListAsync();

            // Step 3: Load trip metadata
            var trips = await context.Trip
                .Select(x => new { x.ID, x.Name, x.FromDate, x.ToDate })
                .ToListAsync();

            // Step 4: Build result DTO
            var tripResults = new List<TripResultDto>();

            foreach (var trip in trips)
            {
                var tripDates = Enumerable.Range(0, (trip.ToDate - trip.FromDate).Days + 1)
                    .Select(offset => trip.FromDate.AddDays(offset));

                var tripDetailList = tripDetailsByTrip.TryGetValue(trip.ID, out var details)
                    ? details
                    : new List<TripDetailFlatDto>();

                var tripDtos = new List<TripDto>();

                foreach (var date in tripDates)
                {
                    var detailsForDate = tripDetailList
                        .Where(d => d.Date == date)
                        .GroupBy(d => d.TripDetailTypeName)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    var typeDtos = new List<TripDetailTypeDto>();

                    foreach (var type in tripDetailTypes)
                    {
                        var values = detailsForDate.TryGetValue(type.Name, out var typeDetails)
                            ? typeDetails.Select(d => new TripDetailTypeValueDto
                            {
                                TypeValueID = d.TripDetailID,
                                TypeValue = d.TripDetailName,
                                TypeVTypeLink = string.IsNullOrWhiteSpace(d.TripDetailLink) ? string.Empty : d.TripDetailLink
                            }).ToList()
                            : new List<TripDetailTypeValueDto> {
                      new TripDetailTypeValueDto { TypeValue = "-", TypeVTypeLink = string.Empty }
                              };

                        typeDtos.Add(new TripDetailTypeDto
                        {
                            TypeID = type.ID,
                            TypeName = type.Name,
                            TypeValues = values
                        });
                    }

                    tripDtos.Add(new TripDto
                    {
                        Date = date,
                        TripDetailDto = new TripDetailDto
                        {
                            TripDetailTypesInfo = typeDtos
                        }
                    });
                }

                tripResults.Add(new TripResultDto
                {
                    ID = trip.ID,
                    Name = trip.Name,
                    TripDtos = tripDtos
                });
            }

            return tripResults;
        }

        public async Task<bool> IsExist(int id) =>
          await context.Trip.AnyAsync(x => x.ID == id);
        public async Task<Trip>? GetAsync(int id) =>
            await context.Trip.FindAsync(id);

        public async Task<IEnumerable<Trip>> GetAllAsync() =>
            await context.Trip.OrderByDescending(x => x.FromDate).ToListAsync();

        public async Task Add(Trip entity)
        {
            await context.Trip.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public void Update(Trip entity)
        {
            context.Trip.Update(entity);
            context.SaveChanges();
        }

        public void Delete(Trip entity)
        {
            context.Trip.Remove(entity);
            context.SaveChanges();
        }
    }
}
