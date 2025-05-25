using Allinone.DLL.Data;
using Allinone.Domain.DS.DSItems;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Allinone.DLL.Repositories
{
    public interface IDSItemRepository
    {
        Task<IEnumerable<DSItemWithSubDtoV2>> GetDSItemWithSubV2(int memberid);
        Task<IEnumerable<DSItemWithSubDtoV3>>? GetDSItemWithSubV3(int memberid);
        Task<IEnumerable<DSItemDto>> GetDSItems(int memberid);

        Task<bool> IsExist(string name);
        Task<bool> IsExistByMemberAndIdAsync(int memberid, int id);
        Task<bool> IsExistByMemberAndSubIdAsync(int memberid, int dsitemsubid);
        Task<DSItem>? GetAsync(int id);
        Task<IEnumerable<DSItem>> GetAllByMemberAsync(int memberid);
        Task<IEnumerable<DSItem>> GetAllAsync();
        Task Add(DSItem entity);
        void Update(DSItem entity);
        void Delete(DSItem entity);
    }


    public class DSItemRepository(DSContext context) : IDSItemRepository
    {
        public async Task<IEnumerable<DSItemWithSubDtoV2>> GetDSItemWithSubV2(int memberid)
        {
            var responses = (
                from a in context.DSItem
                join b in context.DSItemSub on a.ID equals b.DSItemID into bb
                from b2 in bb.DefaultIfEmpty()
                where a.MemberID == memberid
                select new
                {
                    ID = a.ID,
                    ItemName = a.Name,
                    DSItemID = b2.DSItemID != null ? b2.DSItemID : 0,
                    SubID = b2.ID != null ? b2.ID : 0,
                    SubName = b2.Name != null ? b2.Name : string.Empty,
                    SubActive = b2.IsActive != null ? b2.IsActive : false,
                }).ToListAsync();

            var dsItems = await responses;

            var resss = new List<DSItemWithSubDtoV2>();

            var dsItemsG = dsItems.GroupBy(x => new { x.ID, x.ItemName });

            var dsItemsRes =
                    dsItemsG.SelectMany(group => new[]
                        {
                            new DSItemWithSubDtoV2
                            {
                                ID = group.Key.ID,
                                Name = group.Key.ItemName
                            }
                        }.Concat
                        (
                            group.Where(x => x.SubID != 0)
                                .Select(sub => new DSItemWithSubDtoV2
                                {
                                    ID = group.Key.ID,
                                    Name = group.Key.ItemName,
                                    SubName = sub.SubName,
                                    SubID = sub.SubID,
                                })
                        )
                    ).ToList();

            return dsItemsRes.OrderBy(x => x.Name).ThenBy(x => x.SubName);
        }

        public async Task<IEnumerable<DSItemWithSubDtoV3>>? GetDSItemWithSubV3(int memberid)
        {
            var responses = await (
                from a in context.DSItem
                join b in context.DSItemSub on a.ID equals b.DSItemID into bb
                from b2 in bb.DefaultIfEmpty()
                where a.MemberID == memberid
                select new
                {
                    ID = a.ID,
                    ItemName = a.Name,
                    DSItemID = b2.DSItemID != null ? b2.DSItemID : 0,
                    SubID = b2.ID != null ? b2.ID : 0,
                    SubName = b2.Name != null ? b2.Name : string.Empty,
                    SubActive = b2.IsActive != null ? b2.IsActive : false,
                }).ToListAsync();

            var dsItemsG = responses.GroupBy(x => new { x.ID, x.ItemName });

            var dsItemsRes =
                    dsItemsG.SelectMany(group => new[]
                        {
                            new DSItemWithSubDtoV3
                            {
                                Name = group.Key.ItemName,
                                ItemID = group.Key.ID,
                                ItemSubID = 0
                            }
                        }.Concat
                        (
                            group.Where(x => x.SubID != 0)
                                .Select(sub => new DSItemWithSubDtoV3
                                {
                                    Name = $"{group.Key.ItemName}|{sub.SubName}",
                                    ItemID = 0,
                                    ItemSubID = sub.SubID
                                })
                        )
                    ).ToList();

            return dsItemsRes;
        }

        public async Task<IEnumerable<DSItemDto>> GetDSItems(int memberid)
        {
            return await context.DSItem
                .Where(x => x.MemberID == memberid)
                .Select(x => new DSItemDto
                {
                    ID = x.ID,
                    Name = x.Name,
                }).ToListAsync();
        }


        public async Task<bool> IsExist(string name) =>
           await context.DSItem.AnyAsync(x => x.Name == name);

        public async Task<bool> IsExistByMemberAndIdAsync(int memberid, int id) =>
            await context.DSItem.AnyAsync(x => x.MemberID == memberid && x.ID == id);

        public async Task<bool> IsExistByMemberAndSubIdAsync(int memberid, int dsitemsubid) =>
            await context.DSItem
                .Where(x => x.MemberID == memberid)
                .AnyAsync(x => x.DSItemSubs.Any(sub => sub.ID == dsitemsubid));

        public async Task<DSItem>? GetAsync(int id) =>
            await context.DSItem.FindAsync(id);

        public async Task<IEnumerable<DSItem>> GetAllByMemberAsync(int memberid) =>
            await context.DSItem
                .Where(x => x.MemberID == memberid)
                .Include(x => x.DSItemSubs)
                .ToListAsync();

        public async Task<IEnumerable<DSItem>> GetAllAsync() =>
            await context.DSItem.ToListAsync();

        public async Task Add(DSItem entity)
        {
            await context.DSItem.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public void Update(DSItem entity)
        {
            context.DSItem.Update(entity);
            context.SaveChanges();
        }

        public void Delete(DSItem entity)
        {
            context.DSItem.Remove(entity);
            context.SaveChanges();
        }
    }
}
