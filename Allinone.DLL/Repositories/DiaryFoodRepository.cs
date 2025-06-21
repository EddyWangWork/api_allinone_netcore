using Allinone.DLL.Data;
using Allinone.Domain.Diarys.DiaryFoods;

namespace Allinone.DLL.Repositories
{
    public interface IDiaryFoodRepository : IBaseRepository<DiaryFood>
    {

    }

    public class DiaryFoodRepository(DSContext _context) : BaseRepository<DiaryFood>(_context), IDiaryFoodRepository
    {
    }
}
