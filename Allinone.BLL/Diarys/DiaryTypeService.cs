using Allinone.DLL.Repositories;
using Allinone.Domain.Diarys.DiaryTypes;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Diarys
{
    public interface IDiaryTypeService
    {
        Task<IEnumerable<DiaryType>> GetAllByMemberAsync();
        Task<DiaryType> GetAllByMemberAsync(int id);
        Task<DiaryType> AddAsync(DiaryTypeAddReq req);
        Task<DiaryType> UpdateAsync(int id, DiaryTypeAddReq req);
        Task<DiaryType> DeleteAsync(int id);
    }

    public class DiaryTypeService(
        IDiaryTypeRepository _diaryTypeRepository,
        MemoryCacheHelper _memoryCacheHelper,
        IMapModel _mapper) : BaseBLL, IDiaryTypeService
    {
        public async Task<IEnumerable<DiaryType>> GetAllByMemberAsync()
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            return await _diaryTypeRepository.GetAllByMemberAsync(MemberId);
        }

        public async Task<DiaryType> GetAllByMemberAsync(int id)
        {
            return await _diaryTypeRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryTypeNotFoundException();
        }

        public async Task<DiaryType> AddAsync(DiaryTypeAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = _mapper.MapDto<DiaryTypeAddReq, DiaryType>(req);
            entity.MemberID = MemberId;

            await _diaryTypeRepository.AddAsync(entity);

            return entity;
        }

        public async Task<DiaryType> UpdateAsync(int id, DiaryTypeAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _diaryTypeRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryTypeNotFoundException();

            _mapper.Map(req, entity);

            await _diaryTypeRepository.UpdateAsync(entity);

            return entity;
        }

        public async Task<DiaryType> DeleteAsync(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _diaryTypeRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryTypeNotFoundException();

            await _diaryTypeRepository.DeleteAsync(entity);

            return entity;
        }
    }
}
