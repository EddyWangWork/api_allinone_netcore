using Allinone.DLL.Data;
using Allinone.Domain.Shops.ShopTypes;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IShopTypeRepository
    {
        Task<ShopType>? GetByMemberAsync(int memberid, int id);
        Task<ShopType>? GetAsync(int id);
        Task<IEnumerable<ShopType>> GetAllByMemberAsync(int memberid);
        Task<IEnumerable<ShopType>> GetAllByMemberAsync(int memberid, List<int> ids);
        Task<IEnumerable<ShopType>> GetAllAsync();
        Task Add(ShopType entity);
        void Update(ShopType entity);
        void Delete(ShopType entity);
    }

    public class ShopTypeRepository(DSContext context) : IShopTypeRepository
    {
        public async Task<ShopType>? GetByMemberAsync(int memberid, int id) =>
            await context.ShopType.FirstOrDefaultAsync(x => x.MemberID == memberid && x.ID == id);

        public async Task<ShopType>? GetAsync(int id) =>
            await context.ShopType.FindAsync(id);

        public async Task<IEnumerable<ShopType>> GetAllByMemberAsync(int memberid) =>
            await context.ShopType.Where(x => x.MemberID == memberid).ToListAsync();

        public async Task<IEnumerable<ShopType>> GetAllByMemberAsync(int memberid, List<int> ids) =>
            await context.ShopType
                .Where(x => x.MemberID == memberid && ids.Contains(x.ID)).ToListAsync();

        public async Task<IEnumerable<ShopType>> GetAllAsync() =>
            await context.ShopType.ToListAsync();

        public async Task Add(ShopType entity)
        {
            await context.ShopType.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public void Update(ShopType entity)
        {
            context.ShopType.Update(entity);
            context.SaveChanges();
        }

        public void Delete(ShopType entity)
        {
            context.ShopType.Remove(entity);
            context.SaveChanges();
        }
    }
}
