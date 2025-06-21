using Allinone.DLL.Data;
using Allinone.Domain.Diarys.DiaryLocations;

namespace Allinone.DLL.Repositories
{
    public interface IDiaryLocationRepository : IBaseRepository<DiaryLocation>
    {

    }

    public class DiaryLocationRepository(DSContext _context) : BaseRepository<DiaryLocation>(_context), IDiaryLocationRepository
    {
    }
}
