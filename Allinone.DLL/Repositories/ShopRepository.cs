using Allinone.DLL.Data;
using Allinone.Domain.Shops;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IShopRepository
    {
        Task<Shop> GetByMemberAndDiaryIdAsync(int memberid, int shopDiaryId);
        Task<bool> IsExistByMemberAndDiaryIdAsync(int memberid, int shopDiaryId);
        Task<bool> IsExistByMemberAsync(int memberid, int id);
        Task<Shop>? GetByMemberAsync(int memberid, int id);
        Task<Shop>? GetAsync(int id);
        Task<IEnumerable<Shop>> GetAllByMemberAsync(int memberid);
        Task<IEnumerable<Shop>> GetAllAsync();
        Task Add(Shop entity);
        void Update(Shop entity);
        void Delete(Shop entity);
    }

    public class ShopRepository(DSContext context) : IShopRepository
    {
        public async Task<Shop> GetByMemberAndDiaryIdAsync(int memberid, int shopDiaryId) =>
            await context.Shop
                .FirstOrDefaultAsync(x =>
                    x.MemberID == memberid &&
                    x.ShopDiarys.Any(sub => sub.ID == shopDiaryId));

        public async Task<bool> IsExistByMemberAndDiaryIdAsync(int memberid, int shopDiaryId) =>
            await context.Shop
                .Where(x => x.MemberID == memberid)
                .AnyAsync(x => x.ShopDiarys.Any(sub => sub.ID == shopDiaryId));

        public async Task<bool> IsExistByMemberAsync(int memberid, int id) =>
            await context.Shop.AnyAsync(x => x.MemberID == memberid && x.ID == id);

        public async Task<Shop>? GetByMemberAsync(int memberid, int id) =>
            await context.Shop.FirstOrDefaultAsync(x => x.MemberID == memberid && x.ID == id);

        public async Task<Shop>? GetAsync(int id) =>
            await context.Shop.FindAsync(id);

        public async Task<IEnumerable<Shop>> GetAllByMemberAsync(int memberid) =>
            await context.Shop
                .Where(x => x.MemberID == memberid)
                .Include(x => x.ShopDiarys)
                .ToListAsync();

        public async Task<IEnumerable<Shop>> GetAllAsync() =>
            await context.Shop
                .Include(x => x.ShopDiarys)
                .ToListAsync();

        public async Task Add(Shop entity)
        {
            await context.Shop.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public void Update(Shop entity)
        {
            context.Shop.Update(entity);
            context.SaveChanges();
        }

        public void Delete(Shop entity)
        {
            context.Shop.Remove(entity);
            context.SaveChanges();
        }
    }
}
