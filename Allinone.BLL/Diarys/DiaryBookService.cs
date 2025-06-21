using Allinone.DLL.Repositories;
using Allinone.Domain.Diarys.DiaryBooks;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Diarys
{
    public interface IDiaryBookService
    {
        Task<IEnumerable<DiaryBook>> GetAllByMemberAsync();
        Task<DiaryBook> GetAllByMemberAsync(int id);
        Task<DiaryBook> AddAsync(DiaryBookAddReq req);
        Task<DiaryBook> UpdateAsync(int id, DiaryBookAddReq req);
        Task<DiaryBook> DeleteAsync(int id);
    }

    public class DiaryBookService(
        IDiaryBookRepository _diaryBookRepository,
        MemoryCacheHelper _memoryCacheHelper,
        IMapModel _mapper) : BaseBLL, IDiaryBookService
    {
        public async Task<IEnumerable<DiaryBook>> GetAllByMemberAsync()
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            return await _diaryBookRepository.GetAllByMemberAsync(MemberId);
        }

        public async Task<DiaryBook> GetAllByMemberAsync(int id)
        {
            return await _diaryBookRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryBookNotFoundException();
        }

        public async Task<DiaryBook> AddAsync(DiaryBookAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = _mapper.MapDto<DiaryBookAddReq, DiaryBook>(req);
            entity.MemberID = MemberId;

            await _diaryBookRepository.AddAsync(entity);

            return entity;
        }

        public async Task<DiaryBook> UpdateAsync(int id, DiaryBookAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _diaryBookRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryBookNotFoundException();

            _mapper.Map(req, entity);

            await _diaryBookRepository.UpdateAsync(entity);

            return entity;
        }

        public async Task<DiaryBook> DeleteAsync(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _diaryBookRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryBookNotFoundException();

            await _diaryBookRepository.DeleteAsync(entity);

            return entity;
        }
    }
}
