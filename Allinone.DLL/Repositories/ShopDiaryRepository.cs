using Allinone.DLL.Data;
using Allinone.Domain.Shops;
using Allinone.Domain.Shops.ShopDiarys;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IShopDiaryRepository
    {
        Task<IEnumerable<ShopDiary>> GetAllByMemberAsync(int memberId, int? shopId = null);
        Task<ShopDiary?> GetByMemberAndDiaryIdAsync(int memberId, int shopDiaryId);
        Task AddAsync(ShopDiary entity);
        Task UpdateAsync(ShopDiary entity);
        Task DeleteAsync(ShopDiary entity);
    }

    public class ShopDiaryRepository(DSContext context) : IShopDiaryRepository
    {
        public async Task<IEnumerable<ShopDiary>> GetAllByMemberAsync(int memberId, int? shopId = null) =>
            await MemberDiaries(memberId)
                .Where(x => shopId == null || x.ShopID == shopId)
                .OrderByDescending(x => x.Date)
                .AsNoTracking()
                .ToListAsync();

        public async Task<ShopDiary?> GetByMemberAndDiaryIdAsync(int memberId, int shopDiaryId) =>
            await MemberDiaries(memberId).FirstOrDefaultAsync(x => x.ID == shopDiaryId);

        public async Task AddAsync(ShopDiary entity)
        {
            await context.ShopDiary.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ShopDiary entity)
        {
            context.ShopDiary.Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ShopDiary entity)
        {
            context.ShopDiary.Remove(entity);
            await context.SaveChangesAsync();
        }

        // Diaries are owned through their shop, so ownership is enforced via Shop.MemberID.
        private IQueryable<ShopDiary> MemberDiaries(int memberId) =>
            context.ShopDiary
                .Include(x => x.Shop)
                .Where(x => x.Shop.MemberID == memberId);
    }
}
