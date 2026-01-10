using Allinone.DLL.Data;
using Allinone.Domain.Sessions;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface IUserSessionRepository
    {
        Task<UserSession?> GetByMemberIdAsync(int memberId);
        Task<UserSession?> GetByTokenIdentifierAsync(string tokenIdentifier);
        Task CreateAsync(UserSession session);
        Task UpdateAsync(UserSession session);
        Task DeleteAsync(int memberId);
        Task DeleteExpiredSessionsAsync();
        Task<bool> ExistsAsync(int memberId);
    }

    public class UserSessionRepository(DSContext context) : IUserSessionRepository
    {
        public async Task<UserSession?> GetByMemberIdAsync(int memberId) =>
            await context.UserSession
                .FirstOrDefaultAsync(x => x.MemberID == memberId);

        public async Task<UserSession?> GetByTokenIdentifierAsync(string tokenIdentifier) =>
            await context.UserSession
                .FirstOrDefaultAsync(x => x.TokenIdentifier == tokenIdentifier);

        public async Task CreateAsync(UserSession session)
        {
            await context.UserSession.AddAsync(session);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserSession session)
        {
            context.UserSession.Update(session);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int memberId)
        {
            var session = await GetByMemberIdAsync(memberId);
            if (session != null)
            {
                context.UserSession.Remove(session);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteExpiredSessionsAsync()
        {
            var expiredSessions = await context.UserSession
                .Where(x => x.ExpiresAt != null && x.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            if (expiredSessions.Any())
            {
                context.UserSession.RemoveRange(expiredSessions);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int memberId) =>
            await context.UserSession.AnyAsync(x => x.MemberID == memberId);
    }
}
