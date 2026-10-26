using Allinone.DLL.Data;
using Allinone.Domain.Shops.ShopTypes;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IShopTypeRepository
    {
        Task<ShopType?> GetByMemberAsync(int memberid, int id);
        Task<IEnumerable<ShopType>> GetAllByMemberAsync(int memberid);
        Task<IEnumerable<ShopType>> GetAllByMemberAsync(int memberid, List<int> ids);
        Task Add(ShopType entity);
        Task Update(ShopType entity);
        Task Delete(ShopType entity);
    }

    public class ShopTypeRepository(DSContext context) : IShopTypeRepository
    {
        public async Task<ShopType?> GetByMemberAsync(int memberid, int id) =>
            await context.ShopType.FirstOrDefaultAsync(x => x.MemberID == memberid && x.ID == id);

        public async Task<IEnumerable<ShopType>> GetAllByMemberAsync(int memberid) =>
            await context.ShopType
                .AsNoTracking()
                .Where(x => x.MemberID == memberid)
                .OrderBy(x => x.Name)
                .ToListAsync();

        public async Task<IEnumerable<ShopType>> GetAllByMemberAsync(int memberid, List<int> ids) =>
            await context.ShopType
                .AsNoTracking()
                .Where(x => x.MemberID == memberid && ids.Contains(x.ID))
                .OrderBy(x => x.Name)
                .ToListAsync();

        public async Task Add(ShopType entity)
        {
            await context.ShopType.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task Update(ShopType entity)
        {
            context.ShopType.Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task Delete(ShopType entity)
        {
            context.ShopType.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
