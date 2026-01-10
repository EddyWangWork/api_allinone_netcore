using Allinone.DLL.Data;
using Allinone.Domain.TokenBlacklist;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface ITokenBlacklistRepository
    {
        Task<bool> IsTokenBlacklistedAsync(string tokenIdentifier);
        Task AddToBlacklistAsync(string tokenIdentifier, DateTime expiresAt, string reason, int? memberId = null);
        Task DeleteExpiredTokensAsync();
    }

    public class TokenBlacklistRepository : ITokenBlacklistRepository
    {
        private readonly DSContext _context;

        public TokenBlacklistRepository(DSContext context)
        {
            _context = context;
        }

        public async Task<bool> IsTokenBlacklistedAsync(string tokenIdentifier)
        {
            return await _context.TokenBlacklist
                .AnyAsync(t => t.TokenIdentifier == tokenIdentifier && t.ExpiresAt > DateTime.UtcNow);
        }

        public async Task AddToBlacklistAsync(string tokenIdentifier, DateTime expiresAt, string reason, int? memberId = null)
        {
            var blacklistEntry = new TokenBlacklist
            {
                TokenIdentifier = tokenIdentifier,
                BlacklistedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
                Reason = reason,
                MemberID = memberId
            };

            await _context.TokenBlacklist.AddAsync(blacklistEntry);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteExpiredTokensAsync()
        {
            var expiredTokens = await _context.TokenBlacklist
                .Where(t => t.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync();

            if (expiredTokens.Any())
            {
                _context.TokenBlacklist.RemoveRange(expiredTokens);
                await _context.SaveChangesAsync();
            }
        }
    }
}
