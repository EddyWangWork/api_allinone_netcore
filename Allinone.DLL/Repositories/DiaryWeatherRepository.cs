using Allinone.DLL.Data;
using Allinone.Domain.Diarys.DiaryWeathers;

namespace Allinone.DLL.Repositories
{
    public interface IDiaryWeatherRepository : IBaseRepository<DiaryWeather>
    {

    }

    public class DiaryWeatherRepository(DSContext _context) : BaseRepository<DiaryWeather>(_context), IDiaryWeatherRepository
    {
    }
}
