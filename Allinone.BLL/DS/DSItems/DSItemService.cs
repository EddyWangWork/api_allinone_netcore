using Allinone.DLL.Repositories;
using Allinone.DLL.UnitOfWork;
using Allinone.Domain.DS.DSItems;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Extension;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.DS.DSItems
{
    public interface IDSItemService
    {
        Task<IEnumerable<DSItemWithSubDtoV2>> GetDSItemWithSubV2();
        Task<IEnumerable<DSItemWithSubDtoV3>> GetDSItemWithSubV3();
        Task<IEnumerable<DSItemDto>> GetDSItems();

        Task<bool> AddWithSub(DSItemAddWithSubItemReq req);
        Task<bool> IsExistByMemberAndSubIdAsync(int dsitemsubid);
        Task<IEnumerable<DSItem>> GetAllByMemberAsync();
        Task<DSItem> Get(int id);
        Task<DSItem> Add(DSItemAddReq req);
        Task<DSItem> Update(int id, DSItemAddReq req);
        Task<DSItem> Delete(int id);
    }

    public class DSItemService(
        IUnitOfWork unitOfWork,
        IDSItemRepository dsItemRepository,
        MemoryCacheHelper memoryCacheHelper,
        IMapModel mapper) : BaseBLL, IDSItemService
    {
        public async Task<IEnumerable<DSItemWithSubDtoV2>> GetDSItemWithSubV2()
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            return await dsItemRepository.GetDSItemWithSubV2(MemberId);
        }

        public async Task<IEnumerable<DSItemWithSubDtoV3>> GetDSItemWithSubV3()
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            return await dsItemRepository.GetDSItemWithSubV3(MemberId);
        }

        public async Task<IEnumerable<DSItemDto>> GetDSItems()
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            return await dsItemRepository.GetDSItems(MemberId);
        }

        public async Task<bool> AddWithSub(DSItemAddWithSubItemReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            try
            {
                await unitOfWork.BeginTransactionAsync();

                var dsItemE = new DSItem
                {
                    Name = req.Name,
                    MemberID = MemberId
                };

                await unitOfWork.DSItem.AddAsync(dsItemE);
                await unitOfWork.SaveAsync();

                if (req.SubName.IsNotNullOrEmpty())
                {
                    //add sub item
                    var dsItemSubE = new DSItemSub
                    {
                        DSItemID = dsItemE.ID,
                        Name = req.SubName
                    };

                    await unitOfWork.DSItemSub.AddAsync(dsItemSubE);
                    await unitOfWork.SaveAsync();
                }

                await unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<bool> IsExistByMemberAndSubIdAsync(int dsitemsubid)
        {
            return await dsItemRepository.IsExistByMemberAndSubIdAsync(MemberId, dsitemsubid);
        }

        public async Task<IEnumerable<DSItem>> GetAllByMemberAsync()
        {
            return await dsItemRepository.GetAllByMemberAsync(MemberId);
        }

        public async Task<DSItem> Get(int id)
        {
            return await dsItemRepository.GetAsync(id) ?? throw new DSItemNotFoundException();
        }

        public async Task<DSItem> Add(DSItemAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = mapper.MapDto<DSItemAddReq, DSItem>(req);
            entity.MemberID = MemberId;

            await dsItemRepository.Add(entity);

            return entity;
        }

        public async Task<DSItem> Update(int id, DSItemAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await dsItemRepository.GetAsync(id) ?? throw new DSItemNotFoundException();

            mapper.Map(req, entity);

            dsItemRepository.Update(entity);

            return entity;
        }

        public async Task<DSItem> Delete(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await dsItemRepository.GetAsync(id) ?? throw new DSItemNotFoundException();

            dsItemRepository.Delete(entity);

            return entity;
        }
    }
}
