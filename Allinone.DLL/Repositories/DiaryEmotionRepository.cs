using Allinone.DLL.Data;
using Allinone.Domain.Diarys.DiaryEmotions;

namespace Allinone.DLL.Repositories
{
    public interface IDiaryEmotionRepository : IBaseRepository<DiaryEmotion>
    {

    }

    public class DiaryEmotionRepository(DSContext _context) : BaseRepository<DiaryEmotion>(_context), IDiaryEmotionRepository
    {
    }
}
