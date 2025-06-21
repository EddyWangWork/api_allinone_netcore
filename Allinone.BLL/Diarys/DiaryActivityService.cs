using Allinone.DLL.Repositories;
using Allinone.Domain.Diarys.DiaryActivitys;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Diarys
{
    public interface IDiaryActivityService
    {
        Task<IEnumerable<DiaryActivity>> GetAllByMemberAsync();
        Task<DiaryActivity> GetAllByMemberAsync(int id);
        Task<DiaryActivity> AddAsync(DiaryActivityAddReq req);
        Task<DiaryActivity> UpdateAsync(int id, DiaryActivityAddReq req);
        Task<DiaryActivity> DeleteAsync(int id);
    }

    public class DiaryActivityService(
        IDiaryActivityRepository _diaryActivityRepository,
        MemoryCacheHelper _memoryCacheHelper,
        IMapModel _mapper) : BaseBLL, IDiaryActivityService
    {
        public async Task<IEnumerable<DiaryActivity>> GetAllByMemberAsync()
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            return await _diaryActivityRepository.GetAllByMemberAsync(MemberId);
        }

        public async Task<DiaryActivity> GetAllByMemberAsync(int id)
        {
            return await _diaryActivityRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryActivityNotFoundException();
        }

        public async Task<DiaryActivity> AddAsync(DiaryActivityAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = _mapper.MapDto<DiaryActivityAddReq, DiaryActivity>(req);
            entity.MemberID = MemberId;

            await _diaryActivityRepository.AddAsync(entity);

            return entity;
        }

        public async Task<DiaryActivity> UpdateAsync(int id, DiaryActivityAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _diaryActivityRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryActivityNotFoundException();

            _mapper.Map(req, entity);

            await _diaryActivityRepository.UpdateAsync(entity);

            return entity;
        }

        public async Task<DiaryActivity> DeleteAsync(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _diaryActivityRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryActivityNotFoundException();

            await _diaryActivityRepository.DeleteAsync(entity);

            return entity;
        }
    }
}
