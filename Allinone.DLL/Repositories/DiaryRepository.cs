using Allinone.DLL.Data;
using Allinone.Domain.Diarys;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IDiaryRepository : IBaseRepository<Diary>
    {
        Task<IEnumerable<Diary>> GetAllByMemberOrderByDateAsync(int memberid);
    }

    public class DiaryRepository(DSContext _context) : BaseRepository<Diary>(_context), IDiaryRepository
    {
        public async Task<IEnumerable<Diary>> GetAllByMemberOrderByDateAsync(int memberid)
                => await _context.Diary.Where(x => x.MemberID == memberid).OrderByDescending(x => x.Date).ToListAsync();
    }
}
