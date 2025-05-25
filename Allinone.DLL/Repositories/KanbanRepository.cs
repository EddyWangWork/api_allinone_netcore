using Allinone.DLL.Data;
using Allinone.Domain.Kanbans;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IKanbanRepository
    {
        Task<IEnumerable<Kanban>>? GetKanbansAsync(int memberid);
        Task<Kanban>? GetByMemberAsync(int memberid, int id);
        Task<Kanban>? GetAsync(int id);
        Task<IEnumerable<Kanban>> GetAllByMemberAsync(int memberid);
        Task<IEnumerable<Kanban>> GetAllAsync();
        Task Add(Kanban entity);
        void Update(Kanban entity);
        void Delete(Kanban entity);
    }

    public class KanbanRepository(DSContext context) : IKanbanRepository
    {
        public async Task<IEnumerable<Kanban>>? GetKanbansAsync(int memberid) =>
            await context.Kanban
                .Where(x => x.MemberID == memberid)
                .OrderBy(x => x.Status)
                .ThenBy(x => x.Priority == 0 ? int.MaxValue : x.Priority)
                .ThenByDescending(x => x.UpdatedTime)
                .ToListAsync();

        public async Task<Kanban>? GetByMemberAsync(int memberid, int id) =>
            await context.Kanban.FirstOrDefaultAsync(x => x.MemberID == memberid && x.ID == id);

        public async Task<Kanban>? GetAsync(int id) =>
            await context.Kanban.FindAsync(id);

        public async Task<IEnumerable<Kanban>> GetAllByMemberAsync(int memberid) =>
            await context.Kanban.Where(x => x.MemberID == memberid).ToListAsync();

        public async Task<IEnumerable<Kanban>> GetAllAsync() =>
            await context.Kanban.ToListAsync();

        public async Task Add(Kanban entity)
        {
            await context.Kanban.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public void Update(Kanban entity)
        {
            context.Kanban.Update(entity);
            context.SaveChanges();
        }

        public void Delete(Kanban entity)
        {
            context.Kanban.Remove(entity);
            context.SaveChanges();
        }
    }
}
