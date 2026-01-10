using Allinone.DLL.Repositories;
using Allinone.Domain.Sessions;

namespace Allinone.API.Services
{
    public interface IIdleTimeTrackingService
    {
        Task UpdateLastActivityAsync(string userId);
        Task<bool> IsUserActiveAsync(string userId, TimeSpan idleTimeout);
        Task RemoveUserAsync(string userId);
        Task<DateTime?> GetLastActivityAsync(string userId);
    }

    public class IdleTimeTrackingService : IIdleTimeTrackingService
    {
        private readonly IUserSessionRepository _sessionRepository;

        public IdleTimeTrackingService(IUserSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task UpdateLastActivityAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var memberId))
                return;

            var session = await _sessionRepository.GetByMemberIdAsync(memberId);

            if (session == null)
            {
                // Create new session
                session = new UserSession
                {
                    MemberID = memberId,
                    LastActivityTime = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };
                await _sessionRepository.CreateAsync(session);
            }
            else
            {
                // Update existing session
                session.LastActivityTime = DateTime.UtcNow;
                await _sessionRepository.UpdateAsync(session);
            }
        }

        public async Task<bool> IsUserActiveAsync(string userId, TimeSpan idleTimeout)
        {
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var memberId))
                return false;

            var lastActivity = await GetLastActivityAsync(userId);
            if (!lastActivity.HasValue) return false;

            var timeSinceLastActivity = DateTime.UtcNow - lastActivity.Value;
            return timeSinceLastActivity <= idleTimeout;
        }

        public async Task<DateTime?> GetLastActivityAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var memberId))
                return null;

            var session = await _sessionRepository.GetByMemberIdAsync(memberId);
            return session?.LastActivityTime;
        }

        public async Task RemoveUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var memberId))
                return;

            await _sessionRepository.DeleteAsync(memberId);
        }
    }
}