using Allinone.DLL.Data;
using Allinone.Domain.Diarys.DiaryDetails;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IDiaryDetailRepository : IBaseIDRepository<DiaryDetail>
    {
        Task<IEnumerable<DiaryDetailDto>> GetAllDtoByDiaryIDAsync(int diaryId, int memberid);
        Task<IEnumerable<DiaryDetailDto>> GetAllDtoByMemberAsync(int memberid);
        Task<IEnumerable<DiaryDetail>> GetAllByMemberAsync(int memberid);
        Task<DiaryDetail> GetByMemberAsync(int memberid, int diaryDetailId);
    }

    public class DiaryDetailRepository(DSContext _context) : BaseIDRepository<DiaryDetail>(_context), IDiaryDetailRepository
    {
        public async Task<IEnumerable<DiaryDetailDto>> GetAllDtoByDiaryIDAsync(int diaryId, int memberid) =>
            await _context.Diary
                .Where(x => x.ID == diaryId && x.MemberID == memberid)
                .SelectMany(x => x.DiaryDetails)
                .Select(d => new DiaryDetailDto
                {
                    ID = d.ID,
                    Title = d.Title,
                    Description = d.Description,
                    UpdateDate = d.UpdateDate,
                    DiaryID = d.DiaryID,
                    DiaryTypeID = d.DiaryTypeID,
                    DiaryType = d.DiaryType.Name,
                    DiaryDate = d.Diary.Date
                })
                .ToListAsync();

        public async Task<IEnumerable<DiaryDetailDto>> GetAllDtoByMemberAsync(int memberid) =>
            await _context.Diary
                .Where(x => x.MemberID == memberid)
                .SelectMany(x => x.DiaryDetails)
                .Select(d => new DiaryDetailDto
                {
                    ID = d.ID,
                    Title = d.Title,
                    Description = d.Description,
                    UpdateDate = d.UpdateDate,
                    DiaryID = d.DiaryID,
                    DiaryTypeID = d.DiaryTypeID,
                    DiaryType = d.DiaryType.Name,
                    DiaryDate = d.Diary.Date
                })
                .ToListAsync();

        public async Task<IEnumerable<DiaryDetail>> GetAllByMemberAsync(int memberid) =>
            await _context.Diary
                .Where(x => x.MemberID == memberid)
                .SelectMany(x => x.DiaryDetails)
                //.Include(x => x.Diary)
                //.Include(x => x.DiaryType)
                .ToListAsync();

        public async Task<DiaryDetail> GetByMemberAsync(int memberid, int diaryDetailId) =>
            await _context.Diary
                .Where(x => x.MemberID == memberid)
                .SelectMany(x => x.DiaryDetails)
                .FirstOrDefaultAsync(diaryDetail => diaryDetail.ID == diaryDetailId);
    }
}
