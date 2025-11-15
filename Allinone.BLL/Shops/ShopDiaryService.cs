using Allinone.BLL.Auditlogs;
using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Shops;
using Allinone.Domain.Shops.ShopDiarys;
using Allinone.Domain.Shops.ShopTypes;
using Allinone.Helper.Extension;
using Allinone.Helper.Mapper;
using Newtonsoft.Json;

namespace Allinone.BLL.Shops
{
    public interface IShopDiaryService
    {
        Task<IEnumerable<ShopDiaryDto>> GetShopDiaries();
        Task<IEnumerable<ShopDiaryDto>> GetShopDiaries(int shopId);
        Task<IEnumerable<ShopDiary>> GetAllByMemberAsync();

        Task<ShopDiary> Get(int id);
        Task<ShopDiary> Add(ShopDiaryAddReq req);
        Task<ShopDiary> Update(int id, ShopDiaryAddReq req);
        Task<ShopDiary> Delete(int id);
    }

    public class ShopDiaryService(
        IAuditlogService _auditlogService,
        IShopRepository _shopRepository,
        IShopTypeRepository _shopTypeRepository,
        IShopDiaryRepository _shopDiaryRepository,
        IMapModel mapper) : BaseBLL, IShopDiaryService
    {
        public async Task<IEnumerable<ShopDiaryDto>> GetShopDiaries()
        {
            var shopDiarys = await _shopDiaryRepository.GetAllByMemberAsync(MemberId);

            return shopDiarys.Select(x => ToShopDiaryDto(x));
        }

        public async Task<IEnumerable<ShopDiaryDto>> GetShopDiaries(int shopId)
        {
            var shopDiarys = await _shopDiaryRepository.GetAllByMemberAsync(MemberId, shopId);

            return shopDiarys.Select(x => ToShopDiaryDto(x));
        }

        public async Task<IEnumerable<ShopDiary>> GetAllByMemberAsync()
        {
            return await _shopDiaryRepository.GetAllByMemberAsync(MemberId);
        }

        public async Task<ShopDiary> Get(int id)
        {
            return await _shopDiaryRepository.GetByMemberAndDiaryIdAsync(MemberId, id) ?? throw new ShopDiaryNotFoundException();
        }

        public async Task<ShopDiary> Add(ShopDiaryAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();
            if (!await _shopRepository.IsExistByMemberAsync(MemberId, req.ShopID)) throw new ShopBadRequestException();
            var shopEntity = await _shopRepository.GetByMemberAsync(MemberId, req.ShopID) ?? throw new ShopDiaryNotFoundException();

            var entity = mapper.MapDto<ShopDiaryAddReq, ShopDiary>(req);

            await _shopDiaryRepository.Add(entity);
            await _auditlogService.LogShopDiaryNew(
                entity.Shop.Name, MemberId, JsonConvert.SerializeObject(entity, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));
            return entity;
        }

        public async Task<ShopDiary> Update(int id, ShopDiaryAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();
            if (!await _shopRepository.IsExistByMemberAsync(MemberId, req.ShopID)) throw new ShopBadRequestException();

            var entity = await _shopDiaryRepository.GetByMemberAndDiaryIdAsync(MemberId, id) ?? throw new ShopDiaryNotFoundException();
            var shopEntity = await _shopRepository.GetByMemberAsync(MemberId, req.ShopID) ?? throw new ShopDiaryNotFoundException();

            mapper.Map(req, entity);

            _shopDiaryRepository.Update(entity);
            await _auditlogService.LogShopDiaryUpdate(
               entity.Shop.Name, MemberId,
               JsonConvert.SerializeObject(entity, new JsonSerializerSettings
               {
                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
               }),
               JsonConvert.SerializeObject(req, new JsonSerializerSettings
               {
                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
               }));
            return entity;
        }

        public async Task<ShopDiary> Delete(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _shopDiaryRepository.GetByMemberAndDiaryIdAsync(MemberId, id) ?? throw new ShopDiaryNotFoundException();

            _shopDiaryRepository.Delete(entity);

            return entity;
        }

        //Private
        private ShopDto ToShopDto(Shop shop, IEnumerable<ShopType> shopTypes)
        {
            var dto = mapper.MapDto<ShopDto>(shop);
            var typeIntList = shop.Types.ToIntList();

            dto.TypeList = typeIntList.IsNotNullOrEmpty()
                ? []
                : shopTypes
                    .Where(x => typeIntList.Contains(x.ID))
                    .Select(x => x.Name)
                    .ToList();

            return dto;
        }

        private static ShopDiaryDto ToShopDiaryDto(ShopDiary shopDiary)
        {
            return new ShopDiaryDto
            {
                ID = shopDiary.ID,
                Comment = shopDiary.Comment,
                Date = shopDiary.Date,
                Remark = shopDiary.Remark,
                ShopID = shopDiary.Shop.ID,
                ShopName = shopDiary.Shop.Name
            };
        }
    }
}
