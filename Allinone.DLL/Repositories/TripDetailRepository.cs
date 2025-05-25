using Allinone.DLL.Data;
using Allinone.Domain.Trips;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface ITripDetailRepository
    {
        Task<bool> IsExist(int id);
        Task<TripDetail>? GetAsync(int id);
        Task<IEnumerable<TripDetail>> GetAllAsync();
        Task Add(TripDetail entity);
        void Update(TripDetail entity);
        void Delete(TripDetail entity);
    }


    public class TripDetailRepository(DSContext context) : ITripDetailRepository
    {
        public async Task<bool> IsExist(int id) =>
           await context.TripDetail.AnyAsync(x => x.ID == id);

        public async Task<TripDetail>? GetAsync(int id) =>
            await context.TripDetail.FindAsync(id);

        public async Task<IEnumerable<TripDetail>> GetAllAsync() =>
            await context.TripDetail.ToListAsync();

        public async Task Add(TripDetail entity)
        {
            await context.TripDetail.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public void Update(TripDetail entity)
        {
            context.TripDetail.Update(entity);
            context.SaveChanges();
        }

        public void Delete(TripDetail entity)
        {
            context.TripDetail.Remove(entity);
            context.SaveChanges();
        }
    }
}
