using Allinone.DLL.Repositories;
using Allinone.Domain.Diarys.DiaryWeathers;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Diarys
{
    public interface IDiaryWeatherService
    {
        Task<IEnumerable<DiaryWeather>> GetAllByMemberAsync();
        Task<DiaryWeather> GetAllByMemberAsync(int id);
        Task<DiaryWeather> AddAsync(DiaryWeatherAddReq req);
        Task<DiaryWeather> UpdateAsync(int id, DiaryWeatherAddReq req);
        Task<DiaryWeather> DeleteAsync(int id);
    }

    public class DiaryWeatherService(
        IDiaryWeatherRepository _diaryWeatherRepository,
        MemoryCacheHelper _memoryCacheHelper,
        IMapModel _mapper) : BaseBLL, IDiaryWeatherService
    {
        public async Task<IEnumerable<DiaryWeather>> GetAllByMemberAsync()
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            return await _diaryWeatherRepository.GetAllByMemberAsync(MemberId);
        }

        public async Task<DiaryWeather> GetAllByMemberAsync(int id)
        {
            return await _diaryWeatherRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryWeatherNotFoundException();
        }

        public async Task<DiaryWeather> AddAsync(DiaryWeatherAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = _mapper.MapDto<DiaryWeatherAddReq, DiaryWeather>(req);
            entity.MemberID = MemberId;

            await _diaryWeatherRepository.AddAsync(entity);

            return entity;
        }

        public async Task<DiaryWeather> UpdateAsync(int id, DiaryWeatherAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _diaryWeatherRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryWeatherNotFoundException();

            _mapper.Map(req, entity);

            await _diaryWeatherRepository.UpdateAsync(entity);

            return entity;
        }

        public async Task<DiaryWeather> DeleteAsync(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _diaryWeatherRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryWeatherNotFoundException();

            await _diaryWeatherRepository.DeleteAsync(entity);

            return entity;
        }
    }
}
