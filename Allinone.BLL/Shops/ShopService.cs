using Allinone.BLL.Auditlogs;
using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Shops;
using Allinone.Helper.Mapper;
using Newtonsoft.Json;

namespace Allinone.BLL.Shops
{
    public interface IShopService
    {
        Task<IEnumerable<ShopDto>> GetAllByMemberAsync();
        Task<Shop> Get(int id);
        Task<Shop> Add(ShopAddReq req);
        Task<Shop> Update(int id, ShopAddReq req);
        Task<Shop> Delete(int id);
    }

    public class ShopService(
        IAuditlogService _auditlogService,
        IShopTypeRepository shopTypeRepository,
        IShopRepository shopRepository,
        IMapModel mapper) : BaseBLL, IShopService
    {
        public async Task<IEnumerable<ShopDto>> GetAllByMemberAsync()
        {
            var shops = await shopRepository.GetAllByMemberAsync(MemberId);
            var shopTypesEntities = await shopTypeRepository.GetAllByMemberAsync(MemberId);

            return shops.Select(item =>
            {
                var dto = mapper.MapDto<Shop, ShopDto>(item);

                var typesInt = item.Types.Split(',').Select(int.Parse).ToList();
                var shopTypes = shopTypesEntities.Where(x => typesInt.Contains(x.ID));

                dto.TypeList = [.. shopTypes.Select(x => x.Name)];

                return dto;
            });
        }

        public async Task<Shop> Get(int id)
        {
            return await shopRepository.GetByMemberAsync(MemberId, id) ?? throw new ShopNotFoundException();
        }

        public async Task<Shop> Add(ShopAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = mapper.MapDto<ShopAddReq, Shop>(req);
            entity = ServiceHelper.SetAuditAddMemberFields(entity, MemberId);

            await SetShopTypesField(entity, req.TypeList);

            await shopRepository.Add(entity);

            await _auditlogService.LogShopNew(req.Name, MemberId, JsonConvert.SerializeObject(entity));

            return entity;
        }

        public async Task<Shop> Update(int id, ShopAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await shopRepository.GetByMemberAsync(MemberId, id) ?? throw new ShopNotFoundException();

            mapper.Map(req, entity);

            await SetShopTypesField(entity, req.TypeList);

            shopRepository.Update(entity);
            await _auditlogService.LogShopUpdate(
                entity.Name, MemberId, JsonConvert.SerializeObject(entity), JsonConvert.SerializeObject(req));
            return entity;
        }

        public async Task<Shop> Delete(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await shopRepository.GetByMemberAsync(MemberId, id) ?? throw new ShopNotFoundException();

            shopRepository.Delete(entity);
            await _auditlogService.LogShopDelete(entity.Name, MemberId, JsonConvert.SerializeObject(entity));
            return entity;
        }

        private async Task SetShopTypesField(Shop entity, List<int> typeList)
        {
            if (typeList == null) throw new ShopTypeNotFoundException();

            var validShopTypes =
                ServiceHelper.EnsureNotNullOrEmpty(
                    await shopTypeRepository.GetAllByMemberAsync(MemberId, typeList),
                    new ShopTypeNotFoundException());

            entity.Types = validShopTypes.Count > 1
                ? string.Join(",", validShopTypes.Select(x => x.ID))
                : validShopTypes.FirstOrDefault().ID.ToString() ?? "";
        }
    }
}
