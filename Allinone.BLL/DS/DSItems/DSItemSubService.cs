using Allinone.DLL.Repositories;
using Allinone.Domain.DS.DSItems;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.DS.DSItems
{
    public interface IDSItemSubService
    {
        Task<IEnumerable<DSItemSubDto>> GetAllByMemberAsync();
        Task<DSItemSubDto> Get(int id);
        Task<DSItemSubDto> Add(DSItemSubAddReq req);
        Task<DSItemSubDto> Update(int id, DSItemSubAddReq req);
        Task<DSItemSubDto> Delete(int id);
    }

    public class DSItemSubService(
        IDSItemRepository dsItemRepository,
        IDSItemSubRepository dsItemSubRepository,
        IMapModel mapper) : BaseBLL, IDSItemSubService
    {
        public async Task<IEnumerable<DSItemSubDto>> GetAllByMemberAsync() =>
            await dsItemSubRepository.GetAllByMemberAsync(MemberId);

        public async Task<DSItemSubDto> Get(int id)
        {
            var entity = await GetOwnedAsync(id);
            return ToDto(entity);
        }

        public async Task<DSItemSubDto> Add(DSItemSubAddReq req)
        {
            EnsureMember();
            await EnsureOwnedItemAsync(req.DSItemID);

            var entity = mapper.MapDto<DSItemSubAddReq, DSItemSub>(req);
            await dsItemSubRepository.Add(entity);

            return ToDto(entity);
        }

        public async Task<DSItemSubDto> Update(int id, DSItemSubAddReq req)
        {
            EnsureMember();
            await EnsureOwnedItemAsync(req.DSItemID);

            var entity = await GetOwnedAsync(id);

            mapper.Map(req, entity);
            await dsItemSubRepository.Update(entity);

            return ToDto(entity);
        }

        public async Task<DSItemSubDto> Delete(int id)
        {
            EnsureMember();

            var entity = await GetOwnedAsync(id);
            await dsItemSubRepository.Delete(entity);

            return ToDto(entity);
        }

        private void EnsureMember()
        {
            if (MemberId == 0) throw new MemberNotFoundException();
        }

        private async Task EnsureOwnedItemAsync(int dsItemId)
        {
            if (!await dsItemRepository.IsExistByMemberAndIdAsync(MemberId, dsItemId))
                throw new DSItemNotFoundException("Invalid DSItemID");
        }

        private async Task<DSItemSub> GetOwnedAsync(int id) =>
            await dsItemSubRepository.GetByMemberAsync(MemberId, id)
                ?? throw new DSItemSubNotFoundException("Invalid DSItemSubID");

        private static DSItemSubDto ToDto(DSItemSub entity) => new()
        {
            ID = entity.ID,
            Name = entity.Name,
            IsActive = entity.IsActive,
            DSItemID = entity.DSItemID,
            DSItemName = entity.DSItem?.Name
        };
    }
}
