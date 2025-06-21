using Allinone.DLL.Repositories;
using Allinone.Domain;
using Allinone.Domain.Diarys;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Extension;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Diarys
{
    public interface IDiaryService
    {
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

                dto.Activitys = GetItemContainIds(item.ActivityIDs, diaryActivityEntities.ToDictionary(x => x.ID, x => x.Name));
                dto.Emotions = GetItemContainIds(item.EmotionIDs, diaryEmotionEntities.ToDictionary(x => x.ID, x => x.Name));
                dto.Foods = GetItemContainIds(item.FoodIDs, diaryFoodEntities.ToDictionary(x => x.ID, x => x.Name));
                dto.Locations = GetItemContainIds(item.LocationIDs, diaryLocationEntities.ToDictionary(x => x.ID, x => x.Name));
                dto.Books = GetItemContainIds(item.BookIDs, diaryBookEntities.ToDictionary(x => x.ID, x => x.Name));
                dto.Weathers = GetItemContainIds(item.WeatherIDs, diaryWeatherEntities.ToDictionary(x => x.ID, x => x.Name));

                return dto;
            });
        }

        public async Task<DiaryDto> GetByMemberAsync(int id)
        {
            var diaryEntity = await _diaryRepository.GetAllByMemberAsync(MemberId, id) ??
                throw new DiaryNotFoundException();

            var diaryActivityEntities = await _diaryActivityRepository.GetAllByMemberAsync(MemberId);
            var diaryEmotionEntities = await _diaryEmotionRepository.GetAllByMemberAsync(MemberId);
            var diaryFoodEntities = await _diaryFoodRepository.GetAllByMemberAsync(MemberId);
            var diaryLocationEntities = await _diaryLocationRepository.GetAllByMemberAsync(MemberId);
            var diaryBookEntities = await _diaryBookRepository.GetAllByMemberAsync(MemberId);
            var diaryWeatherEntities = await _diaryWeatherRepository.GetAllByMemberAsync(MemberId);

            var dto = _mapper.MapDto<Diary, DiaryDto>(diaryEntity);

            dto.Activitys = GetItemContainIds(diaryEntity.ActivityIDs, diaryActivityEntities.ToDictionary(x => x.ID, x => x.Name));
            dto.Emotions = GetItemContainIds(diaryEntity.EmotionIDs, diaryEmotionEntities.ToDictionary(x => x.ID, x => x.Name));
            dto.Foods = GetItemContainIds(diaryEntity.FoodIDs, diaryFoodEntities.ToDictionary(x => x.ID, x => x.Name));
            dto.Locations = GetItemContainIds(diaryEntity.LocationIDs, diaryLocationEntities.ToDictionary(x => x.ID, x => x.Name));
            dto.Books = GetItemContainIds(diaryEntity.BookIDs, diaryBookEntities.ToDictionary(x => x.ID, x => x.Name));
            dto.Weathers = GetItemContainIds(diaryEntity.WeatherIDs, diaryWeatherEntities.ToDictionary(x => x.ID, x => x.Name));

            return dto;
        }

        public async Task<Diary> AddAsync(DiaryAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = _mapper.MapDto<DiaryAddReq, Diary>(req);
            entity = ServiceHelper.SetAddMemberFields(entity, MemberId);

            entity.ActivityIDs = await GetItemNameString(_diaryActivityRepository, req.Activitys);
            entity.EmotionIDs = await GetItemNameString(_diaryEmotionRepository, req.Emotions);
            entity.FoodIDs = await GetItemNameString(_diaryEmotionRepository, req.Foods);
            entity.LocationIDs = await GetItemNameString(_diaryEmotionRepository, req.Locations);
            entity.BookIDs = await GetItemNameString(_diaryEmotionRepository, req.Books);
            entity.WeatherIDs = await GetItemNameString(_diaryEmotionRepository, req.Weathers);

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
            entity.FoodIDs = await GetItemNameString(_diaryEmotionRepository, req.Foods);
            entity.LocationIDs = await GetItemNameString(_diaryEmotionRepository, req.Locations);
            entity.BookIDs = await GetItemNameString(_diaryEmotionRepository, req.Books);
            entity.WeatherIDs = await GetItemNameString(_diaryEmotionRepository, req.Weathers);

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
    }
}
