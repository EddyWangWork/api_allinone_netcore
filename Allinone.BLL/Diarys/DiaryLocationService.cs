using Allinone.DLL.Repositories;
using Allinone.Domain.Diarys.DiaryLocations;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Diarys
{
    public interface IDiaryLocationService
    {
        Task<IEnumerable<DiaryLocation>> GetAllByMemberAsync();
        Task<DiaryLocation> GetAllByMemberAsync(int id);
        Task<DiaryLocation> AddAsync(DiaryLocationAddReq req);
        Task<DiaryLocation> UpdateAsync(int id, DiaryLocationAddReq req);
        Task<DiaryLocation> DeleteAsync(int id);
    }

    public class DiaryLocationService(
        IDiaryLocationRepository _diaryLocationRepository,
        MemoryCacheHelper _memoryCacheHelper,
        IMapModel _mapper) : BaseBLL, IDiaryLocationService
    {
        public async Task<IEnumerable<DiaryLocation>> GetAllByMemberAsync()
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            return await _diaryLocationRepository.GetAllByMemberAsync(MemberId);
        }

        public async Task<DiaryLocation> GetAllByMemberAsync(int id)
        {
            return await _diaryLocationRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryLocationNotFoundException();
        }

        public async Task<DiaryLocation> AddAsync(DiaryLocationAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = _mapper.MapDto<DiaryLocationAddReq, DiaryLocation>(req);
            entity.MemberID = MemberId;

            await _diaryLocationRepository.AddAsync(entity);

            return entity;
        }

        public async Task<DiaryLocation> UpdateAsync(int id, DiaryLocationAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _diaryLocationRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryLocationNotFoundException();

            _mapper.Map(req, entity);

            await _diaryLocationRepository.UpdateAsync(entity);

            return entity;
        }

        public async Task<DiaryLocation> DeleteAsync(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _diaryLocationRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryLocationNotFoundException();

            await _diaryLocationRepository.DeleteAsync(entity);

            return entity;
        }
    }
}
