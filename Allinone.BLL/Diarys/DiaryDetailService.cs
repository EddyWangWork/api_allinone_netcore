using Allinone.DLL.Repositories;
using Allinone.Domain.Diarys.DiaryDetails;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Diarys
{
    public interface IDiaryDetailService
    {
        Task<IEnumerable<DiaryDetailDto>> GetAllDtoByDiaryIDAsync(int diaryId);
        Task<IEnumerable<DiaryDetailDto>> GetAllDtoByMemberAsync();
        Task<IEnumerable<DiaryDetail>> GetAllByMemberAsync();
        Task<DiaryDetail> GetByMemberAsync(int id);
        Task<DiaryDetail> AddAsync(DiaryDetailAddReq req);
        Task<DiaryDetail> UpdateAsync(int id, DiaryDetailAddReq req);
        Task<DiaryDetail> DeleteAsync(int id);
    }

    public class DiaryDetailService(
        IDiaryRepository _diaryRepository,
        IDiaryTypeRepository _diaryTypeRepository,
        IDiaryDetailRepository _diaryDetailRepository,
        MemoryCacheHelper _memoryCacheHelper,
        IMapModel _mapper) : BaseBLL, IDiaryDetailService
    {
        public async Task<IEnumerable<DiaryDetailDto>> GetAllDtoByDiaryIDAsync(int diaryId)
        {
            return await _diaryDetailRepository.GetAllDtoByDiaryIDAsync(diaryId, MemberId);
        }

        public async Task<IEnumerable<DiaryDetailDto>> GetAllDtoByMemberAsync()
        {
            return await _diaryDetailRepository.GetAllDtoByMemberAsync(MemberId);
        }

        public async Task<IEnumerable<DiaryDetail>> GetAllByMemberAsync()
        {
            return await _diaryDetailRepository.GetAllByMemberAsync(MemberId);
        }

        public async Task<DiaryDetail> GetByMemberAsync(int id)
        {
            return await _diaryDetailRepository.GetByMemberAsync(MemberId, id) ?? throw new DiaryDetailNotFoundException();
        }

        public async Task<DiaryDetail> AddAsync(DiaryDetailAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();
            if (!await _diaryRepository.IsExistByMemberAsync(MemberId, req.DiaryID)) throw new DiaryBadRequestException();
            if (!await _diaryTypeRepository.IsExistByMemberAsync(MemberId, req.DiaryTypeID)) throw new DiaryTypeBadRequestException();

            var entity = _mapper.MapDto<DiaryDetailAddReq, DiaryDetail>(req);

            await _diaryDetailRepository.AddAsync(entity);

            return entity;
        }

        public async Task<DiaryDetail> UpdateAsync(int id, DiaryDetailAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();
            if (!await _diaryRepository.IsExistByMemberAsync(MemberId, req.DiaryID)) throw new DiaryBadRequestException();
            if (!await _diaryTypeRepository.IsExistByMemberAsync(MemberId, req.DiaryTypeID)) throw new DiaryTypeBadRequestException();

            var entity = await _diaryDetailRepository.GetByMemberAsync(MemberId, id) ?? throw new DiaryDetailNotFoundException();
            entity = ServiceHelper.SetUpdateDateFields(entity);

            _mapper.Map(req, entity);

            await _diaryDetailRepository.UpdateAsync(entity);

            return entity;
        }

        public async Task<DiaryDetail> DeleteAsync(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _diaryDetailRepository.GetByMemberAsync(MemberId, id) ?? throw new DiaryDetailNotFoundException();

            await _diaryDetailRepository.DeleteAsync(entity);

            return entity;
        }
    }
}