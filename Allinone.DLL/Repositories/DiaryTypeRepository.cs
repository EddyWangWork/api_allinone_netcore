using Allinone.DLL.Data;
using Allinone.Domain.Diarys.DiaryTypes;

namespace Allinone.DLL.Repositories
{
    public interface IDiaryTypeRepository : IBaseRepository<DiaryType>
    {
    }

    public class DiaryTypeRepository(DSContext _context) : BaseRepository<DiaryType>(_context), IDiaryTypeRepository
    {
    }
}
