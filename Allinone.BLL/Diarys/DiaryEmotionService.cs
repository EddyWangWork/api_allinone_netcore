using Allinone.DLL.Repositories;
using Allinone.Domain.Diarys.DiaryEmotions;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Diarys
{
    public interface IDiaryEmotionService
    {
        Task<IEnumerable<DiaryEmotion>> GetAllByMemberAsync();
        Task<DiaryEmotion> GetAllByMemberAsync(int id);
        Task<DiaryEmotion> AddAsync(DiaryEmotionAddReq req);
        Task<DiaryEmotion> UpdateAsync(int id, DiaryEmotionAddReq req);
        Task<DiaryEmotion> DeleteAsync(int id);
    }

    public class DiaryEmotionService(
        IDiaryEmotionRepository _diaryEmotionRepository,
        MemoryCacheHelper _memoryCacheHelper,
        IMapModel _mapper) : BaseBLL, IDiaryEmotionService
    {
        public async Task<IEnumerable<DiaryEmotion>> GetAllByMemberAsync()
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            return await _diaryEmotionRepository.GetAllByMemberAsync(MemberId);
        }

        public async Task<DiaryEmotion> GetAllByMemberAsync(int id)
        {
            return await _diaryEmotionRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryEmotionNotFoundException();
        }

        public async Task<DiaryEmotion> AddAsync(DiaryEmotionAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = _mapper.MapDto<DiaryEmotionAddReq, DiaryEmotion>(req);
            entity.MemberID = MemberId;

            await _diaryEmotionRepository.AddAsync(entity);

            return entity;
        }

        public async Task<DiaryEmotion> UpdateAsync(int id, DiaryEmotionAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _diaryEmotionRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryEmotionNotFoundException();

            _mapper.Map(req, entity);

            await _diaryEmotionRepository.UpdateAsync(entity);

            return entity;
        }

        public async Task<DiaryEmotion> DeleteAsync(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _diaryEmotionRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryEmotionNotFoundException();

            await _diaryEmotionRepository.DeleteAsync(entity);

            return entity;
        }
    }
}
