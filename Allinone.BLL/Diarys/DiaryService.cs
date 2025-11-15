using Allinone.DLL.Repositories;
using Allinone.Domain;
using Allinone.Domain.Diarys;
using Allinone.Domain.Diarys.DiaryDetails;
using Allinone.Domain.Enums;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Extension;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Diarys
{
    public interface IDiaryService
    {
        Task<DiaryInfoDto> GetAllDiaryInfoAsync();
        Task<IEnumerable<DiaryDto>> GetAllByMemberOrderByDateAsync();
        Task<DiaryDto> GetByMemberAsync(int id);
        Task<Diary> AddAsync(DiaryAddReq req);
        Task<Diary> UpdateAsync(int id, DiaryAddReq req);
        Task<Diary> DeleteAsync(int id);
    }

    public class DiaryService(
        IDiaryRepository _diaryRepository,
        IDiaryActivityRepository _diaryActivityRepository,
        IDiaryEmotionRepository _diaryEmotionRepository,
        IDiaryFoodRepository _diaryFoodRepository,
        IDiaryLocationRepository _diaryLocationRepository,
        IDiaryBookRepository _diaryBookRepository,
        IDiaryWeatherRepository _diaryWeatherRepository,
        MemoryCacheHelper _memoryCacheHelper,
        IMapModel _mapper) : BaseBLL, IDiaryService
    {
        public async Task<DiaryInfoDto> GetAllDiaryInfoAsync()
        {
            var diaryActivityEntities = await _diaryActivityRepository.GetAllByMemberAsync(MemberId);
            var diaryEmotionEntities = await _diaryEmotionRepository.GetAllByMemberAsync(MemberId);
            var diaryFoodEntities = await _diaryFoodRepository.GetAllByMemberAsync(MemberId);
            var diaryLocationEntities = await _diaryLocationRepository.GetAllByMemberAsync(MemberId);
            var diaryBookEntities = await _diaryBookRepository.GetAllByMemberAsync(MemberId);
            var diaryWeatherEntities = await _diaryWeatherRepository.GetAllByMemberAsync(MemberId);

            var diaryInfo = new DiaryInfoDto
            {
                Activitys = diaryActivityEntities.Select(x => new EnumModel { ID = x.ID, Name = x.Name }).ToList(),
                Emotions = diaryEmotionEntities.Select(x => new EnumModel { ID = x.ID, Name = x.Name }).ToList(),
                Foods = diaryFoodEntities.Select(x => new EnumModel { ID = x.ID, Name = x.Name }).ToList(),
                Locations = diaryLocationEntities.Select(x => new EnumModel { ID = x.ID, Name = x.Name }).ToList(),
                Books = diaryBookEntities.Select(x => new EnumModel { ID = x.ID, Name = x.Name }).ToList(),
                Weathers = diaryWeatherEntities.Select(x => new EnumModel { ID = x.ID, Name = x.Name }).ToList()
            };

            return diaryInfo;
        }

        public async Task<IEnumerable<DiaryDto>> GetAllByMemberOrderByDateAsync()
        {
            var diaryEntities = await _diaryRepository.GetAllByMemberOrderByDateAsync(MemberId);
            var diaryActivityEntities = await _diaryActivityRepository.GetAllByMemberAsync(MemberId);
            var diaryEmotionEntities = await _diaryEmotionRepository.GetAllByMemberAsync(MemberId);
            var diaryFoodEntities = await _diaryFoodRepository.GetAllByMemberAsync(MemberId);
            var diaryLocationEntities = await _diaryLocationRepository.GetAllByMemberAsync(MemberId);
            var diaryBookEntities = await _diaryBookRepository.GetAllByMemberAsync(MemberId);
            var diaryWeatherEntities = await _diaryWeatherRepository.GetAllByMemberAsync(MemberId);

            return diaryEntities.Select(item =>
            {
                var dto = _mapper.MapDto<Diary, DiaryDto>(item);

                List<DiaryDetailDto> dDetailDto = [];
                foreach (var diaryDetail in item.DiaryDetails)
                {
                    dDetailDto.Add(new DiaryDetailDto
                    {
                        ID = diaryDetail.ID,
                        Title = diaryDetail.Title,
                        Description = diaryDetail.Description,
                        DiaryType = diaryDetail.DiaryType.Name
                    });
                }
                dto.DiaryDetailDtos.AddRange(dDetailDto);

                dto.Activitys = GetItemContainIdsEnumModel(item.ActivityIDs, diaryActivityEntities.ToDictionary(x => x.ID, x => x.Name));
                dto.Emotions = GetItemContainIdsEnumModel(item.EmotionIDs, diaryEmotionEntities.ToDictionary(x => x.ID, x => x.Name));
                dto.Foods = GetItemContainIdsEnumModel(item.FoodIDs, diaryFoodEntities.ToDictionary(x => x.ID, x => x.Name));
                dto.Locations = GetItemContainIdsEnumModel(item.LocationIDs, diaryLocationEntities.ToDictionary(x => x.ID, x => x.Name));
                dto.Books = GetItemContainIdsEnumModel(item.BookIDs, diaryBookEntities.ToDictionary(x => x.ID, x => x.Name));
                dto.Weathers = GetItemContainIdsEnumModel(item.WeatherIDs, diaryWeatherEntities.ToDictionary(x => x.ID, x => x.Name));

                return dto;
            });
        }

        public async Task<DiaryDto> GetByMemberAsync(int id)
        {
            var diaryEntity = await _diaryRepository.GetByMemberWithDetailAsync(MemberId, id) ??
                throw new DiaryNotFoundException();

            var diaryActivityEntities = await _diaryActivityRepository.GetAllByMemberAsync(MemberId);
            var diaryEmotionEntities = await _diaryEmotionRepository.GetAllByMemberAsync(MemberId);
            var diaryFoodEntities = await _diaryFoodRepository.GetAllByMemberAsync(MemberId);
            var diaryLocationEntities = await _diaryLocationRepository.GetAllByMemberAsync(MemberId);
            var diaryBookEntities = await _diaryBookRepository.GetAllByMemberAsync(MemberId);
            var diaryWeatherEntities = await _diaryWeatherRepository.GetAllByMemberAsync(MemberId);

            var dto = _mapper.MapDto<Diary, DiaryDto>(diaryEntity);

            List<DiaryDetailDto> dDetailDto = [];
            foreach (var diaryDetail in diaryEntity.DiaryDetails)
            {
                dDetailDto.Add(new DiaryDetailDto
                {
                    Title = diaryDetail.Title,
                    Description = diaryDetail.Description,
                    DiaryType = diaryDetail.DiaryType.Name
                });
            }
            dto.DiaryDetailDtos.AddRange(dDetailDto);

            dto.Activitys = GetItemContainIdsEnumModel(diaryEntity.ActivityIDs, diaryActivityEntities.ToDictionary(x => x.ID, x => x.Name));
            dto.Emotions = GetItemContainIdsEnumModel(diaryEntity.EmotionIDs, diaryEmotionEntities.ToDictionary(x => x.ID, x => x.Name));
            dto.Foods = GetItemContainIdsEnumModel(diaryEntity.FoodIDs, diaryFoodEntities.ToDictionary(x => x.ID, x => x.Name));
            dto.Locations = GetItemContainIdsEnumModel(diaryEntity.LocationIDs, diaryLocationEntities.ToDictionary(x => x.ID, x => x.Name));
            dto.Books = GetItemContainIdsEnumModel(diaryEntity.BookIDs, diaryBookEntities.ToDictionary(x => x.ID, x => x.Name));
            dto.Weathers = GetItemContainIdsEnumModel(diaryEntity.WeatherIDs, diaryWeatherEntities.ToDictionary(x => x.ID, x => x.Name));

            return dto;
        }

        public async Task<Diary> AddAsync(DiaryAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();
            if (await _diaryRepository.IsDiaryDateExist(MemberId, req.Date.Value)) throw new DiaryDateDuplicatedException();

            var entity = _mapper.MapDto<DiaryAddReq, Diary>(req);
            entity = ServiceHelper.SetAddMemberFields(entity, MemberId);

            entity.ActivityIDs = await GetItemNameString(_diaryActivityRepository, req.Activitys);
            entity.EmotionIDs = await GetItemNameString(_diaryEmotionRepository, req.Emotions);
            entity.FoodIDs = await GetItemNameString(_diaryFoodRepository, req.Foods);
            entity.LocationIDs = await GetItemNameString(_diaryLocationRepository, req.Locations);
            entity.BookIDs = await GetItemNameString(_diaryBookRepository, req.Books);
            entity.WeatherIDs = await GetItemNameString(_diaryWeatherRepository, req.Weathers);

            await _diaryRepository.AddAsync(entity);

            return entity;
        }

        public async Task<Diary> UpdateAsync(int id, DiaryAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _diaryRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryNotFoundException();
            _mapper.Map(req, entity);

            entity.ActivityIDs = await GetItemNameString(_diaryActivityRepository, req.Activitys);
            entity.EmotionIDs = await GetItemNameString(_diaryEmotionRepository, req.Emotions);
            entity.FoodIDs = await GetItemNameString(_diaryFoodRepository, req.Foods);
            entity.LocationIDs = await GetItemNameString(_diaryLocationRepository, req.Locations);
            entity.BookIDs = await GetItemNameString(_diaryBookRepository, req.Books);
            entity.WeatherIDs = await GetItemNameString(_diaryWeatherRepository, req.Weathers);

            await _diaryRepository.UpdateAsync(entity);

            return entity;
        }

        public async Task<Diary> DeleteAsync(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _diaryRepository.GetAllByMemberAsync(MemberId, id) ?? throw new DiaryNotFoundException();
            await _diaryRepository.DeleteAsync(entity);

            return entity;
        }

        /*------------------------------------ Private */
        private static async Task<string> GetItemNameString<T>(IBaseRepository<T> repo, List<int> ids) where T : class, IMember
        {
            if (ids.IsNullOrEmpty())
                return "";

            var itemIds = await repo.GetIDsByMemberAsync(MemberId, ids);
            return itemIds.Count() == 0 ?
                "" : itemIds.Count() > 1 ?
                string.Join(",", itemIds.Select(x => x.ToString())) : itemIds.FirstOrDefault().ToString();
        }

        private static List<string> GetItemContainIds(
            string diaryEntityItemString,
            Dictionary<int, string> dicItems)
        {
            if (diaryEntityItemString.IsNullOrEmpty())
                return [];

            return diaryEntityItemString
                .Split(',')
                .Select(int.Parse)
                .Where(dicItems.ContainsKey)
                .Select(id => dicItems[id])
                .ToList();
        }

        private static List<EnumModel> GetItemContainIdsEnumModel(
            string diaryEntityItemString,
            Dictionary<int, string> dicItems)
        {
            if (diaryEntityItemString.IsNullOrEmpty())
                return [];

            return diaryEntityItemString
                .Split(',')
                .Select(int.Parse)
                .Where(dicItems.ContainsKey)
                .Select(id => new EnumModel
                {
                    ID = id,
                    Name = dicItems[id]
                })
                .ToList();
        }
    }
}
