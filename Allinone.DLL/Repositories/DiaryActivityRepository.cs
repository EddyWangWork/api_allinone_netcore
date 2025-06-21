using Allinone.DLL.Data;
using Allinone.Domain.Diarys.DiaryActivitys;

namespace Allinone.DLL.Repositories
{
    public interface IDiaryActivityRepository : IBaseRepository<DiaryActivity>
    {
    }

    public class DiaryActivityRepository(DSContext _context) : BaseRepository<DiaryActivity>(_context), IDiaryActivityRepository
    {
    }
}
