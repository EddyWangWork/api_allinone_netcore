using Allinone.DLL.Data;
using Allinone.Domain.DS.DSItems;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IDSItemSubRepository
    {
        Task<bool> IsExist(string name);
        Task<DSItemSub>? GetAsync(int id);
        Task<IEnumerable<DSItemSub>> GetAllAsync();
        Task Add(DSItemSub entity);
        void Update(DSItemSub entity);
        void Delete(DSItemSub entity);
    }

    public class DSItemSubRepository(DSContext context) : IDSItemSubRepository
    {
        public async Task<bool> IsExist(string name) =>
           await context.DSItemSub.AnyAsync(x => x.Name == name);

        public async Task<DSItemSub>? GetAsync(int id) =>
            await context.DSItemSub.FindAsync(id);

        public async Task<IEnumerable<DSItemSub>> GetAllAsync() =>
            await context.DSItemSub.ToListAsync();

        public async Task Add(DSItemSub entity)
        {
            await context.DSItemSub.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public void Update(DSItemSub entity)
        {
            context.DSItemSub.Update(entity);
            context.SaveChanges();
        }

        public void Delete(DSItemSub entity)
        {
            context.DSItemSub.Remove(entity);
            context.SaveChanges();
        }
    }
}
