using Allinone.BLL.DS.Transactions;
using Allinone.DLL.Repositories;
using Allinone.Domain.DS.Accounts;
using Allinone.Domain.Enums;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.DS.Accounts
{
    public interface IDSAccountService
    {
        Task<List<DSAccountDto>> GetDSAccountsWithBalance();

        Task<IEnumerable<DSAccount>> Get();
        Task<DSAccount> Get(int id);
        Task<DSAccount> Add(DSAccountAddReq req);
        Task<DSAccount> Update(int id, DSAccountAddReq req);
        Task<DSAccount> Delete(int id);
    }

    public class DSAccountService(
        IDSTransactionService dsTransactionService,
        IDSAccountRepository dsAccountRepository,
        MemoryCacheHelper memoryCacheHelper,
        IMapModel mapper) : BaseBLL, IDSAccountService
    {
        private readonly List<int> expensesList =
        [
            (int)EnumDSTranType.Expense,
            (int)EnumDSTranType.TransferOut,
            (int)EnumDSTranType.DebitTransferOut
        ];
        private readonly List<int> incomeList =
        [
            (int)EnumDSTranType.Income,
            (int)EnumDSTranType.TransferIn,
            (int)EnumDSTranType.CreditTransferIn
        ];

        public async Task<List<DSAccountDto>> GetDSAccountsWithBalance()
        {
            var dsAccountList = await dsAccountRepository.GetAllByMemberAsync(MemberId);
            var dsTransList = await dsTransactionService.GetAllByMemberIdCacheAsync();

            var transByAccount = dsTransList.ToLookup(x => x.DSAccountID);

            return dsAccountList
                .Select(dsAccount =>
                {
                    var dto = new DSAccountDto
                    {
                        ID = dsAccount.ID,
                        Name = dsAccount.Name,
                        IsActive = dsAccount.IsActive
                    };

                    var trans = transByAccount[dsAccount.ID].ToList();
                    if (trans.Count == 0) return dto;

                    dto.Balance = trans.Where(x => incomeList.Contains(x.DSTypeID)).Sum(x => x.Amount)
                        - trans.Where(x => expensesList.Contains(x.DSTypeID)).Sum(x => x.Amount);
                    dto.CreatedDateTime = trans.Max(x => x.CreatedDateTime);

                    return dto;
                })
                .OrderByDescending(x => x.IsActive)
                .ThenByDescending(x => x.Balance)
                .ToList();
        }

        public async Task<IEnumerable<DSAccount>> Get()
        {
            EnsureMember();

            return await dsAccountRepository.GetAllByMemberAsync(MemberId);
        }

        public async Task<DSAccount> Get(int id)
        {
            EnsureMember();

            return await dsAccountRepository.GetByMemberAsync(MemberId, id) ?? throw new DSAccountNotFoundException();
        }

        public async Task<DSAccount> Add(DSAccountAddReq req)
        {
            EnsureMember();

            var entity = mapper.MapDto<DSAccountAddReq, DSAccount>(req);
            entity = ServiceHelper.SetAuditAddMemberFields(entity, MemberId);

            await dsAccountRepository.Add(entity);

            return entity;
        }

        public async Task<DSAccount> Update(int id, DSAccountAddReq req)
        {
            var entity = await Get(id);

            mapper.Map(req, entity);

            dsAccountRepository.Update(entity);

            return entity;
        }

        public async Task<DSAccount> Delete(int id)
        {
            var entity = await Get(id);

            dsAccountRepository.Delete(entity);

            return entity;
        }

        private static void EnsureMember()
        {
            if (MemberId == 0) throw new MemberNotFoundException();
        }
    }
}
