using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Shops.ShopTypes;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Shops
{
    public interface IShopTypeService
    {
        Task<IEnumerable<ShopType>> GetAllByMemberAsync();
        Task<IEnumerable<ShopType>> GetAllByMemberAsync(List<int> ids);
        Task<ShopType> Get(int id);
        Task<ShopType> Add(ShopTypeAddReq req);
        Task<ShopType> Update(int id, ShopTypeAddReq req);
        Task<ShopType> Delete(int id);
    }

    public class ShopTypeService(
        IShopTypeRepository shopTypeRepository,
        IMapModel mapper) : BaseBLL, IShopTypeService
    {
        public async Task<IEnumerable<ShopType>> GetAllByMemberAsync()
        {
            return await shopTypeRepository.GetAllByMemberAsync(MemberId);
        }

        public async Task<IEnumerable<ShopType>> GetAllByMemberAsync(List<int> ids)
        {
            return await shopTypeRepository.GetAllByMemberAsync(MemberId, ids);
        }

        public async Task<ShopType> Get(int id)
        {
            return await shopTypeRepository.GetByMemberAsync(MemberId, id) ?? throw new ShopTypeNotFoundException();
        }

        public async Task<ShopType> Add(ShopTypeAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = mapper.MapDto<ShopTypeAddReq, ShopType>(req);
            entity = ServiceHelper.SetAuditAddMemberFields(entity, MemberId);

            await shopTypeRepository.Add(entity);

            return entity;
        }

        public async Task<ShopType> Update(int id, ShopTypeAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await shopTypeRepository.GetByMemberAsync(MemberId, id) ?? throw new ShopTypeNotFoundException();

            mapper.Map(req, entity);

            shopTypeRepository.Update(entity);

            return entity;
        }

        public async Task<ShopType> Delete(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await shopTypeRepository.GetByMemberAsync(MemberId, id) ?? throw new ShopTypeNotFoundException();

            shopTypeRepository.Delete(entity);

            return entity;
        }
    }
}
