using Allinone.DLL.Data;
using Allinone.Domain.Trips;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface ITripRepository
    {
        Task<IEnumerable<TripResultDto>> GetAllDetailsV2Async();

        Task<bool> IsExist(int id);
        Task<Trip?> GetAsync(int id);
        Task<IEnumerable<Trip>> GetAllAsync();
        Task Add(Trip entity);
        Task Update(Trip entity);
        Task Delete(Trip entity);
    }

    public class TripRepository(DSContext context) : ITripRepository
    {
        public async Task<IEnumerable<TripResultDto>> GetAllDetailsV2Async()
        {
            var trips = await context.Trip
                .AsNoTracking()
                .Select(x => new { x.ID, x.Name, x.FromDate, x.ToDate })
                .ToListAsync();

            var types = await context.TripDetailType
                .AsNoTracking()
                .Select(x => new { x.ID, x.Name })
                .ToListAsync();

            var details = await context.TripDetail
                .AsNoTracking()
                .Select(x => new
                {
                    x.TripID,
                    x.TripDetailTypeID,
                    x.Date,
                    x.ID,
                    x.Name,
                    x.LinkName
                })
                .ToListAsync();

            var detailsByTrip = details.ToLookup(x => x.TripID);

            var results = new List<TripResultDto>(trips.Count);

            foreach (var trip in trips)
            {
                var tripDetails = detailsByTrip[trip.ID]
                    .ToLookup(x => (x.Date, x.TripDetailTypeID));

                var days = (trip.ToDate - trip.FromDate).Days + 1;
                var tripDtos = new List<TripDto>(Math.Max(days, 0));

                for (var i = 0; i < days; i++)
                {
                    var date = trip.FromDate.AddDays(i);

                    var typeDtos = types.Select(type =>
                    {
                        var values = tripDetails[(date, type.ID)]
                            .Select(d => new TripDetailTypeValueDto
                            {
                                TypeValueID = d.ID,
                                TypeValue = d.Name,
                                TypeVTypeLink = d.LinkName ?? string.Empty
                            })
                            .ToList();

                        if (values.Count == 0)
                        {
                            values.Add(new TripDetailTypeValueDto { TypeValue = "-", TypeVTypeLink = string.Empty });
                        }

                        return new TripDetailTypeDto
                        {
                            TypeID = type.ID,
                            TypeName = type.Name,
                            TypeValues = values
                        };
                    }).ToList();

                    tripDtos.Add(new TripDto
                    {
                        Date = date,
                        TripDetailDto = new TripDetailDto { TripDetailTypesInfo = typeDtos }
                    });
                }

                results.Add(new TripResultDto
                {
                    ID = trip.ID,
                    Name = trip.Name,
                    TripDtos = tripDtos
                });
            }

            return results;
        }

        public async Task<bool> IsExist(int id) =>
            await context.Trip.AnyAsync(x => x.ID == id);

        public async Task<Trip?> GetAsync(int id) =>
            await context.Trip.FindAsync(id);

        public async Task<IEnumerable<Trip>> GetAllAsync() =>
            await context.Trip.AsNoTracking().OrderByDescending(x => x.FromDate).ToListAsync();

        public async Task Add(Trip entity)
        {
            await context.Trip.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task Update(Trip entity)
        {
            context.Trip.Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task Delete(Trip entity)
        {
            context.Trip.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
