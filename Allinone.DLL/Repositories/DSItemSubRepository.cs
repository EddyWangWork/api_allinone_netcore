using Allinone.DLL.Data;
using Allinone.Domain.DS.DSItems;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IDSItemSubRepository
    {
        Task<bool> IsExist(string name);
        Task<DSItemSub?> GetAsync(int id);
        Task<DSItemSub?> GetByMemberAsync(int memberId, int id);
        Task<List<DSItemSubDto>> GetAllByMemberAsync(int memberId);
        Task Add(DSItemSub entity);
        Task Update(DSItemSub entity);
        Task Delete(DSItemSub entity);
    }

    public class DSItemSubRepository(DSContext context) : IDSItemSubRepository
    {
        public async Task<bool> IsExist(string name) =>
           await context.DSItemSub.AnyAsync(x => x.Name == name);

        public async Task<DSItemSub?> GetAsync(int id) =>
            await context.DSItemSub.FindAsync(id);

        // Scoped to the owning member through the parent DSItem
        public async Task<DSItemSub?> GetByMemberAsync(int memberId, int id) =>
            await context.DSItemSub
                .FirstOrDefaultAsync(x => x.ID == id && x.DSItem.MemberID == memberId);

        public async Task<List<DSItemSubDto>> GetAllByMemberAsync(int memberId) =>
            await context.DSItemSub
                .AsNoTracking()
                .Where(x => x.DSItem.MemberID == memberId)
                .OrderBy(x => x.DSItemID)
                .ThenBy(x => x.ID)
                .Select(x => new DSItemSubDto
                {
                    ID = x.ID,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    DSItemID = x.DSItemID,
                    DSItemName = x.DSItem.Name
                })
                .ToListAsync();

        public async Task Add(DSItemSub entity)
        {
            await context.DSItemSub.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task Update(DSItemSub entity)
        {
            context.DSItemSub.Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task Delete(DSItemSub entity)
        {
            context.DSItemSub.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
