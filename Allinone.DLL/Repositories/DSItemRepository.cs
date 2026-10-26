using Allinone.DLL.Data;
using Allinone.Domain.DS.DSItems;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IDSItemRepository
    {
        Task<IEnumerable<DSItemWithSubDtoV2>> GetDSItemWithSubV2(int memberid);
        Task<IEnumerable<DSItemWithSubDtoV3>> GetDSItemWithSubV3(int memberid);
        Task<IEnumerable<DSItemDto>> GetDSItems(int memberid);

        Task<bool> IsExistByMemberAndIdAsync(int memberid, int id);
        Task<bool> IsExistByMemberAndSubIdAsync(int memberid, int dsitemsubid);
        Task<DSItem?> GetByMemberAsync(int memberid, int id);
        Task<IEnumerable<DSItem>> GetAllByMemberAsync(int memberid);
        Task Add(DSItem entity);
        Task Update(DSItem entity);
        Task Delete(DSItem entity);
    }

    public class DSItemRepository(DSContext context) : IDSItemRepository
    {
        private sealed record ItemSubRow(int ItemID, string ItemName, int SubID, string SubName);

        // Flat item/sub rows for a member; items without subs have SubID = 0.
        private async Task<List<ItemSubRow>> GetItemSubRowsAsync(int memberid)
        {
            return await (
                from a in context.DSItem.AsNoTracking()
                join b in context.DSItemSub.AsNoTracking() on a.ID equals b.DSItemID into bb
                from b2 in bb.DefaultIfEmpty()
                where a.MemberID == memberid
                select new ItemSubRow(
                    a.ID,
                    a.Name,
                    b2 != null ? b2.ID : 0,
                    b2 != null ? b2.Name : string.Empty)).ToListAsync();
        }

        public async Task<IEnumerable<DSItemWithSubDtoV2>> GetDSItemWithSubV2(int memberid)
        {
            var rows = await GetItemSubRowsAsync(memberid);

            return rows
                .GroupBy(x => new { x.ItemID, x.ItemName })
                .SelectMany(g => new[]
                    {
                        new DSItemWithSubDtoV2 { ID = g.Key.ItemID, Name = g.Key.ItemName }
                    }
                    .Concat(g.Where(x => x.SubID != 0)
                        .Select(sub => new DSItemWithSubDtoV2
                        {
                            ID = g.Key.ItemID,
                            Name = g.Key.ItemName,
                            SubName = sub.SubName,
                            SubID = sub.SubID,
                        })))
                .OrderBy(x => x.Name)
                .ThenBy(x => x.SubName)
                .ToList();
        }

        public async Task<IEnumerable<DSItemWithSubDtoV3>> GetDSItemWithSubV3(int memberid)
        {
            var rows = await GetItemSubRowsAsync(memberid);

            return rows
                .GroupBy(x => new { x.ItemID, x.ItemName })
                .SelectMany(g => new[]
                    {
                        new DSItemWithSubDtoV3 { Name = g.Key.ItemName, ItemID = g.Key.ItemID, ItemSubID = 0 }
                    }
                    .Concat(g.Where(x => x.SubID != 0)
                        .Select(sub => new DSItemWithSubDtoV3
                        {
                            Name = $"{g.Key.ItemName}|{sub.SubName}",
                            ItemID = 0,
                            ItemSubID = sub.SubID
                        })))
                .ToList();
        }

        public async Task<IEnumerable<DSItemDto>> GetDSItems(int memberid) =>
            await context.DSItem
                .AsNoTracking()
                .Where(x => x.MemberID == memberid)
                .Select(x => new DSItemDto { ID = x.ID, Name = x.Name })
                .ToListAsync();

        public async Task<bool> IsExistByMemberAndIdAsync(int memberid, int id) =>
            await context.DSItem.AnyAsync(x => x.MemberID == memberid && x.ID == id);

        public async Task<bool> IsExistByMemberAndSubIdAsync(int memberid, int dsitemsubid) =>
            await context.DSItem
                .Where(x => x.MemberID == memberid)
                .AnyAsync(x => x.DSItemSubs.Any(sub => sub.ID == dsitemsubid));

        public async Task<DSItem?> GetByMemberAsync(int memberid, int id) =>
            await context.DSItem.FirstOrDefaultAsync(x => x.MemberID == memberid && x.ID == id);

        public async Task<IEnumerable<DSItem>> GetAllByMemberAsync(int memberid) =>
            await context.DSItem
                .Where(x => x.MemberID == memberid)
                .Include(x => x.DSItemSubs)
                .ToListAsync();

        public async Task Add(DSItem entity)
        {
            await context.DSItem.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task Update(DSItem entity)
        {
            context.DSItem.Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task Delete(DSItem entity)
        {
            context.DSItem.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
