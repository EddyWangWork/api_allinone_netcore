using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Shops.ShopTypes;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Shops
{
    public interface IShopTypeService
    {
        Task<IEnumerable<ShopTypeDto>> GetAllByMemberAsync();
        Task<IEnumerable<ShopTypeDto>> GetAllByMemberAsync(List<int> ids);
        Task<ShopTypeDto> Get(int id);
        Task<ShopTypeDto> Add(ShopTypeAddReq req);
        Task<ShopTypeDto> Update(int id, ShopTypeAddReq req);
        Task<ShopTypeDto> Delete(int id);
    }

    public class ShopTypeService(
        IShopTypeRepository shopTypeRepository,
        IMapModel mapper) : BaseBLL, IShopTypeService
    {
        public async Task<IEnumerable<ShopTypeDto>> GetAllByMemberAsync() =>
            (await shopTypeRepository.GetAllByMemberAsync(MemberId)).Select(ToDto).ToList();

        public async Task<IEnumerable<ShopTypeDto>> GetAllByMemberAsync(List<int> ids) =>
            (await shopTypeRepository.GetAllByMemberAsync(MemberId, ids)).Select(ToDto).ToList();

        public async Task<ShopTypeDto> Get(int id) =>
            ToDto(await GetEntityAsync(id));

        public async Task<ShopTypeDto> Add(ShopTypeAddReq req)
        {
            EnsureMember();

            var entity = mapper.MapDto<ShopTypeAddReq, ShopType>(req);
            entity = ServiceHelper.SetAuditAddMemberFields(entity, MemberId);

            await shopTypeRepository.Add(entity);

            return ToDto(entity);
        }

        public async Task<ShopTypeDto> Update(int id, ShopTypeAddReq req)
        {
            EnsureMember();

            var entity = await GetEntityAsync(id);

            mapper.Map(req, entity);

            await shopTypeRepository.Update(entity);

            return ToDto(entity);
        }

        public async Task<ShopTypeDto> Delete(int id)
        {
            EnsureMember();

            var entity = await GetEntityAsync(id);

            await shopTypeRepository.Delete(entity);

            return ToDto(entity);
        }

        private async Task<ShopType> GetEntityAsync(int id) =>
            await shopTypeRepository.GetByMemberAsync(MemberId, id) ?? throw new ShopTypeNotFoundException();

        private static void EnsureMember()
        {
            if (MemberId == 0) throw new MemberNotFoundException();
        }

        private static ShopTypeDto ToDto(ShopType x) => new() { ID = x.ID, Name = x.Name };
    }
}
