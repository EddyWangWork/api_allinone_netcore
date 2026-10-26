using Allinone.DLL.Data;
using Allinone.Domain.Kanbans;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IKanbanRepository
    {
        Task<List<Kanban>> GetKanbansAsync(int memberid);
        Task<Kanban?> GetByMemberAsync(int memberid, int id);
        Task Add(Kanban entity);
        Task Update(Kanban entity);
        Task Delete(Kanban entity);
    }

    public class KanbanRepository(DSContext context) : IKanbanRepository
    {
        public Task<List<Kanban>> GetKanbansAsync(int memberid) =>
            context.Kanban
                .AsNoTracking()
                .Where(x => x.MemberID == memberid)
                .OrderBy(x => x.Status)
                .ThenBy(x => x.Priority == 0 ? int.MaxValue : x.Priority)
                .ThenByDescending(x => x.UpdatedTime)
                .ToListAsync();

        public Task<Kanban?> GetByMemberAsync(int memberid, int id) =>
            context.Kanban.FirstOrDefaultAsync(x => x.MemberID == memberid && x.ID == id);

        public async Task Add(Kanban entity)
        {
            await context.Kanban.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task Update(Kanban entity)
        {
            context.Kanban.Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task Delete(Kanban entity)
        {
            context.Kanban.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
