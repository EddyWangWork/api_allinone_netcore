using Allinone.DLL.Data;
using Allinone.Domain.Trips;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface ITripDetailTypeRepository
    {
        Task<bool> IsExist(int id);
        Task<TripDetailType>? GetAsync(int id);
        Task<IEnumerable<TripDetailType>> GetAllAsync();
        Task Add(TripDetailType entity);
        void Update(TripDetailType entity);
        void Delete(TripDetailType entity);
    }

    public class TripDetailTypeRepository(DSContext context) : ITripDetailTypeRepository
    {
        public async Task<bool> IsExist(int id) =>
          await context.TripDetailType.AnyAsync(x => x.ID == id);

        public async Task<TripDetailType>? GetAsync(int id) =>
            await context.TripDetailType.FindAsync(id);

        public async Task<IEnumerable<TripDetailType>> GetAllAsync() =>
            await context.TripDetailType.ToListAsync();

        public async Task Add(TripDetailType entity)
        {
            await context.TripDetailType.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public void Update(TripDetailType entity)
        {
            context.TripDetailType.Update(entity);
            context.SaveChanges();
        }

        public void Delete(TripDetailType entity)
        {
            context.TripDetailType.Remove(entity);
            context.SaveChanges();
        }
    }
}
