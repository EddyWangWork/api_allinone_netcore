using Allinone.DLL.Data;
using Allinone.Domain;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllByMemberAsync(int memberid);
        Task<T> GetAllByMemberAsync(int memberid, int id);
        Task<IEnumerable<T>> GetAllByMemberAsync(int memberid, List<int> ids);
        Task<IEnumerable<int>> GetIDsByMemberAsync(int memberid, List<int> ids);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task DeleteAsync(T entity);
    }

    public class BaseRepository<T>(DSContext context) : IBaseRepository<T> where T : class, IMember
    {
        private readonly DbSet<T> _dbSet = context.Set<T>();

        public async Task<IEnumerable<T>> GetAllByMemberAsync(int memberid) =>
            await _dbSet.Where(x => x.MemberID == memberid).ToListAsync();

        public async Task<T> GetAllByMemberAsync(int memberid, int id) =>
            await _dbSet.Where(x => x.MemberID == memberid && x.ID == id).FirstOrDefaultAsync();

        public async Task<IEnumerable<T>> GetAllByMemberAsync(int memberid, List<int> ids) =>
            await _dbSet.Where(x => x.MemberID == memberid && ids.Contains(x.ID)).ToListAsync();

        public async Task<IEnumerable<int>> GetIDsByMemberAsync(int memberid, List<int> ids) =>
             await _dbSet.
                Where(x => x.MemberID == memberid && ids.Contains(x.ID)).
                Select(x => x.ID).ToListAsync();

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);


        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
