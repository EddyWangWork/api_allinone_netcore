using Allinone.DLL.Data;
using Allinone.Domain.Installments;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IInstallmentRepository
    {
        Task<IEnumerable<Installment>> GetAllByMemberAsync(int memberId, bool? isActive = null);
        Task<Installment?> GetByMemberAsync(int memberId, int id);
        Task Add(Installment entity);
        void Update(Installment entity);
        void Delete(Installment entity);
    }

    public class InstallmentRepository(DSContext context) : IInstallmentRepository
    {
        public async Task<IEnumerable<Installment>> GetAllByMemberAsync(int memberId, bool? isActive = null)
        {
            var query = context.Installment.Where(x => x.MemberID == memberId);

            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive.Value);

            return await query
                .OrderByDescending(x => x.UpdatedTime)
                .ToListAsync();
        }

        public async Task<Installment?> GetByMemberAsync(int memberId, int id) =>
            await context.Installment.FirstOrDefaultAsync(x => x.MemberID == memberId && x.ID == id);

        public async Task Add(Installment entity)
        {
            await context.Installment.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public void Update(Installment entity)
        {
            context.Installment.Update(entity);
            context.SaveChanges();
        }

        public void Delete(Installment entity)
        {
            context.Installment.Remove(entity);
            context.SaveChanges();
        }
    }
}
