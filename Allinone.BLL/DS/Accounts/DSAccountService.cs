using Allinone.BLL.DS.Transactions;
using Allinone.DLL.Repositories;
using Allinone.Domain.DS.Accounts;
using Allinone.Domain.Enums;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Todolists;
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
            var dsAccounts = new List<DSAccountDto>();
            var dsAccountsOrdered = new List<DSAccountDto>();

            var dsAccountList = await dsAccountRepository.GetAllByMemberAsync(MemberId);
            var dsTransList = await dsTransactionService.GetAllByMemberIdCacheAsync();

            foreach (var dsAccount in dsAccountList)
            {
                var dsAccountDto = new DSAccountDto
                {
                    ID = dsAccount.ID,
                    Name = dsAccount.Name,
                    IsActive = dsAccount.IsActive
                };

                var dsAccountSelected = dsTransList.Where(x => x.DSAccountID == dsAccount.ID);

                if (!dsAccountSelected.Any())
                {
                    dsAccounts.Add(dsAccountDto);
                    continue;
                }

                var incomes = dsAccountSelected.Where(x => incomeList.Contains(x.DSTypeID)).Sum(x => x.Amount);
                var expenses = dsAccountSelected.Where(x => expensesList.Contains(x.DSTypeID)).Sum(x => x.Amount);
                var LatestCreatedDateTime = dsAccountSelected.OrderByDescending(x => x.CreatedDateTime).FirstOrDefault().CreatedDateTime;

                dsAccountDto.Balance = incomes - expenses;
                dsAccountDto.CreatedDateTime = LatestCreatedDateTime;

                dsAccounts.Add(dsAccountDto);
            }

            dsAccountsOrdered.AddRange(dsAccounts.Where(x => x.IsActive == true).OrderByDescending(x => x.Balance));
            dsAccountsOrdered.AddRange(dsAccounts.Where(x => x.IsActive != true).OrderByDescending(x => x.Balance));

            return dsAccountsOrdered;
        }

        public async Task<IEnumerable<DSAccount>> Get()
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            return await dsAccountRepository.GetAllByMemberAsync(MemberId);
        }

        public async Task<DSAccount> Get(int id)
        {
            return await dsAccountRepository.GetAsync(id) ?? throw new DSAccountNotFoundException();
        }

        public async Task<DSAccount> Add(DSAccountAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = mapper.MapDto<DSAccountAddReq, DSAccount>(req);
            entity = ServiceHelper.SetAuditAddMemberFields(entity, MemberId);

            await dsAccountRepository.Add(entity);

            return entity;
        }

        public async Task<DSAccount> Update(int id, DSAccountAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await dsAccountRepository.GetAsync(id) ?? throw new DSAccountNotFoundException();

            mapper.Map(req, entity);

            dsAccountRepository.Update(entity);

            return entity;
        }

        public async Task<DSAccount> Delete(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await dsAccountRepository.GetAsync(id) ?? throw new DSAccountNotFoundException();

            dsAccountRepository.Delete(entity);

            return entity;
        }
    }
}
