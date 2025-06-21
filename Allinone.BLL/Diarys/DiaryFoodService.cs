using Allinone.DLL.Repositories;
using Allinone.Domain.Diarys.DiaryFoods;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Diarys
{
    public interface IDiaryFoodService
    {
        Task<IEnumerable<DiaryFood>> GetAllByMemberAsync();
        Task<DiaryFood> GetAllByMemberAsync(int id);
        Task<DiaryFood> AddAsync(DiaryFoodAddReq req);
        Task<DiaryFood> UpdateAsync(int id, DiaryFoodAddReq req);
        Task<DiaryFood> DeleteAsync(int id);
    }

    public class DiaryFoodService(
        IDiaryFoodRepository _diaryFoodRepository,
        MemoryCacheHelper _memoryCacheHelper,
        IMapModel _mapper) : BaseBLL, IDiaryFoodService
    {
        public async Task<IEnumerable<DiaryFood>> GetAllByMemberAsync()
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            return await _diaryFoodRepository.GetAllByMemberAsync(MemberId);
        }

        public async Task<DiaryFood> GetAllByMemberAsync(int id)
        {
            return await _diaryFoodRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryFoodNotFoundException();
        }

        public async Task<DiaryFood> AddAsync(DiaryFoodAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = _mapper.MapDto<DiaryFoodAddReq, DiaryFood>(req);
            entity.MemberID = MemberId;

            await _diaryFoodRepository.AddAsync(entity);

            return entity;
        }

        public async Task<DiaryFood> UpdateAsync(int id, DiaryFoodAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _diaryFoodRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryFoodNotFoundException();

            _mapper.Map(req, entity);

            await _diaryFoodRepository.UpdateAsync(entity);

            return entity;
        }

        public async Task<DiaryFood> DeleteAsync(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _diaryFoodRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryFoodNotFoundException();

            await _diaryFoodRepository.DeleteAsync(entity);

            return entity;
        }
    }
}
