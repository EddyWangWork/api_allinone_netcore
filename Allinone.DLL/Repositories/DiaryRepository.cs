using Allinone.DLL.Data;
using Allinone.Domain.Diarys;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IDiaryRepository : IBaseRepository<Diary>
    {
        Task<bool> IsDiaryDateExist(int memberid, DateTime diaryDate);
        Task<IEnumerable<Diary>> GetAllByMemberOrderByDateAsync(int memberid);
        Task<Diary> GetByMemberWithDetailAsync(int memberid, int id);
    }

    public class DiaryRepository(DSContext _context) : BaseRepository<Diary>(_context), IDiaryRepository
    {
        public async Task<bool> IsDiaryDateExist(int memberid, DateTime diaryDate) =>
            await _context.Diary
                .AnyAsync(x => x.MemberID == memberid && x.Date == diaryDate);

        public async Task<IEnumerable<Diary>> GetAllByMemberOrderByDateAsync(int memberid) =>
            await _context.Diary
                .Where(x => x.MemberID == memberid)
                .OrderByDescending(x => x.Date)
                .Include(x => x.DiaryDetails)
                    .ThenInclude(detail => detail.DiaryType)
                .ToListAsync();

        public async Task<Diary> GetByMemberWithDetailAsync(int memberid, int id) =>
            await _context.Diary
                .Where(x => x.MemberID == memberid && x.ID == id)
                .Include(x => x.DiaryDetails)
                    .ThenInclude(detail => detail.DiaryType)
                .FirstOrDefaultAsync();
    }
}
