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

        Task<ShopDiaryDto> Get(int id);
        Task<ShopDiaryDto> Add(ShopDiaryAddReq req);
        Task<ShopDiaryDto> Update(int id, ShopDiaryAddReq req);
        Task<ShopDiaryDto> Delete(int id);
    }

    public class ShopDiaryService(
        IAuditlogService _auditlogService,
        IShopRepository _shopRepository,
        IShopDiaryRepository _shopDiaryRepository,
        IMapModel mapper) : BaseBLL, IShopDiaryService
    {
        private static readonly JsonSerializerSettings AuditJsonSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public async Task<IEnumerable<ShopDiaryDto>> GetShopDiaries() =>
            (await _shopDiaryRepository.GetAllByMemberAsync(MemberId)).Select(ToShopDiaryDto);

        public async Task<IEnumerable<ShopDiaryDto>> GetShopDiaries(int shopId) =>
            (await _shopDiaryRepository.GetAllByMemberAsync(MemberId, shopId)).Select(ToShopDiaryDto);

        public async Task<ShopDiaryDto> Get(int id) =>
            ToShopDiaryDto(await GetOwnedDiary(id));

        public async Task<ShopDiaryDto> Add(ShopDiaryAddReq req)
        {
            EnsureMember();
            var shop = await GetOwnedShop(req.ShopID);

            var entity = mapper.MapDto<ShopDiaryAddReq, ShopDiary>(req);
            entity.Shop = shop;

            await _shopDiaryRepository.AddAsync(entity);
            await _auditlogService.LogShopDiaryNew(shop.Name, MemberId, Serialize(entity));
            return ToShopDiaryDto(entity);
        }

        public async Task<ShopDiaryDto> Update(int id, ShopDiaryAddReq req)
        {
            EnsureMember();
            var shop = await GetOwnedShop(req.ShopID);
            var entity = await GetOwnedDiary(id);

            var oldValue = Serialize(entity);
            mapper.Map(req, entity);
            entity.Shop = shop;

            await _shopDiaryRepository.UpdateAsync(entity);
            await _auditlogService.LogShopDiaryUpdate(shop.Name, MemberId, Serialize(entity), oldValue);
            return ToShopDiaryDto(entity);
        }

        public async Task<ShopDiaryDto> Delete(int id)
        {
            EnsureMember();
            var entity = await GetOwnedDiary(id);

            await _shopDiaryRepository.DeleteAsync(entity);
            return ToShopDiaryDto(entity);
        }

        //Private
        private void EnsureMember()
        {
            if (MemberId == 0) throw new MemberNotFoundException();
        }

        private async Task<ShopDiary> GetOwnedDiary(int id) =>
            await _shopDiaryRepository.GetByMemberAndDiaryIdAsync(MemberId, id) ?? throw new ShopDiaryNotFoundException();

        private async Task<Shop> GetOwnedShop(int shopId)
        {
            if (!await _shopRepository.IsExistByMemberAsync(MemberId, shopId)) throw new ShopBadRequestException();
            return await _shopRepository.GetByMemberAsync(MemberId, shopId) ?? throw new ShopBadRequestException();
        }

        private static string Serialize(object value) => JsonConvert.SerializeObject(value, AuditJsonSettings);

        private static ShopDiaryDto ToShopDiaryDto(ShopDiary shopDiary) => new()
        {
            ID = shopDiary.ID,
            Comment = shopDiary.Comment,
            Date = shopDiary.Date,
            Remark = shopDiary.Remark,
            ShopID = shopDiary.ShopID,
            ShopName = shopDiary.Shop?.Name ?? string.Empty
        };
    }
}
