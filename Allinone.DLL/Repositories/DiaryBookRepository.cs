using Allinone.DLL.Data;
using Allinone.Domain.Diarys.DiaryBooks;

namespace Allinone.DLL.Repositories
{
    public interface IDiaryBookRepository : IBaseRepository<DiaryBook>
    {

    }

    public class DiaryBookRepository(DSContext _context) : BaseRepository<DiaryBook>(_context), IDiaryBookRepository
    {
    }
}
