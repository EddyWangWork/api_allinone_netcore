using Allinone.Helper.Cache;

namespace Allinone.API.Services
{
    public interface IIdleTimeTrackingService
    {
        void UpdateLastActivity(string userId);
        bool IsUserActive(string userId, TimeSpan idleTimeout);
        void RemoveUser(string userId);
        DateTime? GetLastActivity(string userId);
    }

    public class IdleTimeTrackingService : IIdleTimeTrackingService
    {
        private readonly MemoryCacheHelper _cache;
        private readonly string _cacheKeyPrefix = "user_activity_";

        public IdleTimeTrackingService(MemoryCacheHelper cache)
        {
            _cache = cache;
        }

        public void UpdateLastActivity(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return;

            var cacheKey = _cacheKeyPrefix + userId;
            var lastActivity = DateTime.UtcNow;

            // Cache for 24 hours - this should be longer than any reasonable idle timeout
            _cache.Set(cacheKey, lastActivity, TimeSpan.FromHours(24));
        }

        public bool IsUserActive(string userId, TimeSpan idleTimeout)
        {
            if (string.IsNullOrEmpty(userId)) return false;

            var lastActivity = GetLastActivity(userId);
            if (!lastActivity.HasValue) return false;

            var timeSinceLastActivity = DateTime.UtcNow - lastActivity.Value;
            return timeSinceLastActivity <= idleTimeout;
        }

        public DateTime? GetLastActivity(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;

            var cacheKey = _cacheKeyPrefix + userId;
            if (_cache.TryGetValue<DateTime>(cacheKey, out var lastActivity))
            {
                return lastActivity;
            }
            return null;
        }

        public void RemoveUser(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return;

            var cacheKey = _cacheKeyPrefix + userId;
            _cache.Remove(cacheKey);
        }
    }
}