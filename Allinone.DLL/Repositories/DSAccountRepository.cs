using Allinone.DLL.Data;
using Allinone.Domain.DS.Accounts;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IDSAccountRepository
    {
        Task<bool> IsExist(string name);
        Task<bool> IsExistByMember(int memberid, int dsAccount);
        Task<DSAccount>? GetAsync(int id);
        Task<IEnumerable<DSAccount>> GetAllByMemberAsync(int memberid);
        Task<IEnumerable<DSAccount>> GetAllAsync();
        Task Add(DSAccount entity);
        void Update(DSAccount entity);
        void Delete(DSAccount entity);
    }

    public class DSAccountRepository(DSContext context) : IDSAccountRepository
    {
        public async Task<bool> IsExist(string name) =>
           await context.DSAccount.AnyAsync(x => x.Name == name);

        public async Task<bool> IsExistByMember(int memberid, int id) =>
          await context.DSAccount.AnyAsync(x => x.MemberID == memberid && x.ID == id);

        public async Task<DSAccount> GetAsync(int id) =>
            await context.DSAccount.FindAsync(id);

        public async Task<IEnumerable<DSAccount>> GetAllByMemberAsync(int memberid) =>
            await context.DSAccount.Where(x => x.MemberID == memberid).ToListAsync();

        public async Task<IEnumerable<DSAccount>> GetAllAsync() =>
            await context.DSAccount.ToListAsync();

        public async Task Add(DSAccount entity)
        {
            await context.DSAccount.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public void Update(DSAccount entity)
        {
            context.DSAccount.Update(entity);
            context.SaveChanges();
        }

        public void Delete(DSAccount entity)
        {
            context.DSAccount.Remove(entity);
            context.SaveChanges();
        }
    }
}
