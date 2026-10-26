using Allinone.DLL.Data;
using Allinone.Domain.Trips;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface ITripDetailTypeRepository
    {
        Task<bool> IsExist(int id, CancellationToken cancellationToken = default);
        Task<TripDetailType?> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TripDetailType>> GetAllAsync(CancellationToken cancellationToken = default);
        Task Add(TripDetailType entity, CancellationToken cancellationToken = default);
        Task Update(TripDetailType entity, CancellationToken cancellationToken = default);
        Task Delete(TripDetailType entity, CancellationToken cancellationToken = default);
    }

    public class TripDetailTypeRepository(DSContext context) : ITripDetailTypeRepository
    {
        public Task<bool> IsExist(int id, CancellationToken cancellationToken = default) =>
            context.TripDetailType.AnyAsync(x => x.ID == id, cancellationToken);

        public async Task<TripDetailType?> GetAsync(int id, CancellationToken cancellationToken = default) =>
            await context.TripDetailType.FindAsync(new object[] { id }, cancellationToken);

        public async Task<IEnumerable<TripDetailType>> GetAllAsync(CancellationToken cancellationToken = default) =>
            await context.TripDetailType
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.ID)
                .ToListAsync(cancellationToken);

        public async Task Add(TripDetailType entity, CancellationToken cancellationToken = default)
        {
            await context.TripDetailType.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(TripDetailType entity, CancellationToken cancellationToken = default)
        {
            context.TripDetailType.Update(entity);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(TripDetailType entity, CancellationToken cancellationToken = default)
        {
            context.TripDetailType.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
