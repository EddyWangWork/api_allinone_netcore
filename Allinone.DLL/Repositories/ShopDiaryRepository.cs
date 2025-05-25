using Allinone.DLL.Data;
using Allinone.Domain.Shops;
using Allinone.Domain.Shops.ShopDiarys;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IShopDiaryRepository
    {
        Task<IEnumerable<ShopDiary>> GetAllByMemberAsync(int memberid);
        Task<IEnumerable<ShopDiary>> GetAllByMemberAsync(int memberid, int shopId);
        Task<ShopDiary> GetByMemberAndDiaryIdAsync(int memberid, int shopDiaryId);
        Task<ShopDiary>? GetAsync(int id);
        Task<IEnumerable<ShopDiary>> GetAllAsync();
        Task Add(ShopDiary entity);
        void Update(ShopDiary entity);
        void Delete(ShopDiary entity);
    }

    public class ShopDiaryRepository(DSContext context) : IShopDiaryRepository
    {
        public async Task<IEnumerable<ShopDiary>> GetAllByMemberAsync(int memberid) =>
            await context.Shop
                .Where(x => x.MemberID == memberid)
                .SelectMany(x => x.ShopDiarys)
                .Include(x => x.Shop)
                .ToListAsync();

        public async Task<IEnumerable<ShopDiary>> GetAllByMemberAsync(int memberid, int shopId) =>
            await context.Shop
                .Where(x => x.MemberID == memberid && x.ID == shopId)
                .SelectMany(x => x.ShopDiarys)
                .Include(x => x.Shop)
                .ToListAsync();

        public async Task<ShopDiary> GetByMemberAndDiaryIdAsync(int memberid, int shopDiaryId) =>
            await context.Shop
                .Where(x => x.MemberID == memberid)
                .SelectMany(x => x.ShopDiarys)
                .FirstOrDefaultAsync(diary => diary.ID == shopDiaryId);

        public async Task<ShopDiary>? GetAsync(int id) =>
            await context.ShopDiary.FindAsync(id);

        public async Task<IEnumerable<ShopDiary>> GetAllAsync() =>
            await context.ShopDiary.ToListAsync();

        public async Task Add(ShopDiary entity)
        {
            await context.ShopDiary.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public void Update(ShopDiary entity)
        {
            context.ShopDiary.Update(entity);
            context.SaveChanges();
        }

        public void Delete(ShopDiary entity)
        {
            context.ShopDiary.Remove(entity);
            context.SaveChanges();
        }
    }
}
