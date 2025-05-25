using Allinone.DLL.Repositories;
using Allinone.Domain.DS.DSItems;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.DS.DSItems
{
    public interface IDSItemSubService
    {
        Task<IEnumerable<DSItemSubDto>> GetAllByMemberAsync();

        Task<DSItemSub> Get(int id);
        Task<DSItemSubDto> Add(DSItemSubAddReq req);
        Task<DSItemSubDto> Update(int id, DSItemSubAddReq req);
        Task<DSItemSubDto> Delete(int id);
    }

    public class DSItemSubService(
        IDSItemRepository dsItemRepository,
        IDSItemSubRepository dsItemSubRepository,
        IMapModel mapper) : BaseBLL, IDSItemSubService
    {
        public async Task<IEnumerable<DSItemSubDto>> GetAllByMemberAsync()
        {
            var dsitems = await dsItemRepository.GetAllByMemberAsync(MemberId);
            var dsSubItems = dsitems.Where(x => x.DSItemSubs.Any());

            return dsSubItems.SelectMany(item => item.DSItemSubs.Select(itemSub => new DSItemSubDto
            {
                ID = itemSub.ID,
                Name = itemSub.Name,
                IsActive = itemSub.IsActive,
                DSItemID = item.ID,
                DSItemName = item.Name
            }));
        }

        public async Task<DSItemSub> Get(int id)
        {
            return await dsItemSubRepository.GetAsync(id) ?? throw new DSItemSubNotFoundException();
        }

        public async Task<DSItemSubDto> Add(DSItemSubAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            _ = await dsItemRepository.IsExistByMemberAndIdAsync(MemberId, req.DSItemID) ?
                true : throw new DSItemNotFoundException("Invalid DSItemID");

            var entity = mapper.MapDto<DSItemSubAddReq, DSItemSub>(req);

            await dsItemSubRepository.Add(entity);

            return new DSItemSubDto
            {
                ID = entity.ID,
                DSItemID = entity.DSItemID,
                IsActive = entity.IsActive,
                Name = entity.Name,
            };
        }

        public async Task<DSItemSubDto> Update(int id, DSItemSubAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            _ = await dsItemRepository.IsExistByMemberAndIdAsync(MemberId, req.DSItemID) ?
                true : throw new DSItemNotFoundException("Invalid DSItemID");

            _ = await dsItemRepository.IsExistByMemberAndSubIdAsync(MemberId, id) ?
                true : throw new DSItemSubNotFoundException("Invalid DSItemSubID");

            var entity = await dsItemSubRepository.GetAsync(id) ?? throw new DSItemSubNotFoundException();

            mapper.Map(req, entity);

            dsItemSubRepository.Update(entity);

            return new DSItemSubDto
            {
                ID = entity.ID,
                DSItemID = entity.DSItemID,
                IsActive = entity.IsActive,
                Name = entity.Name,
            };
        }

        public async Task<DSItemSubDto> Delete(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            _ = await dsItemRepository.IsExistByMemberAndSubIdAsync(MemberId, id) ?
                true : throw new DSItemSubNotFoundException("Invalid DSItemSubID");

            var entity = await dsItemSubRepository.GetAsync(id) ?? throw new DSItemSubNotFoundException();

            dsItemSubRepository.Delete(entity);

            return new DSItemSubDto
            {
                ID = entity.ID,
                DSItemID = entity.DSItemID,
                IsActive = entity.IsActive,
                Name = entity.Name,
            };
        }
    }
}
