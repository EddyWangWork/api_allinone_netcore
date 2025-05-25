using Allinone.DLL.Data;
using Allinone.Domain.Members;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IMemberRepository
    {
        Task<bool> IsExist(string name);
        Task<Member>? GetAsync(string name, string password);
        Task<IEnumerable<Member>> GetAllAsync();
        Task Add(Member member);
        void Update(Member member);
    }

    public class MemberRepository(DSContext context) : IMemberRepository
    {
        public async Task<bool> IsExist(string name) =>
           await context.Member.AnyAsync(x => x.Name == name);

        public async Task<Member>? GetAsync(string name, string password) =>
            await context.Member.FirstOrDefaultAsync(x => x.Name == name && x.Password == password);

        public async Task<IEnumerable<Member>> GetAllAsync() =>
            await context.Member.ToListAsync();

        public async Task Add(Member member)
        {
            await context.Member.AddAsync(member);
            await context.SaveChangesAsync();
        }

        public void Update(Member member)
        {
            context.Member.Update(member);
            context.SaveChanges();
        }
    }
}
