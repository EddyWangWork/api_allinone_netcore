# Database-Based Idle Time Authentication Implementation

## Overview
The idle time authentication system has been migrated from **in-memory cache** to **database storage** for better persistence, scalability, and auditability.

---

## 🗄️ Database Schema

### UserSession Table
```sql
CREATE TABLE [UserSession] (
    [ID] int NOT NULL IDENTITY PRIMARY KEY,
    [MemberID] int NOT NULL,
    [LastActivityTime] datetime2 NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ExpiresAt] datetime2 NULL,
    [IpAddress] nvarchar(max) NULL,
    [UserAgent] nvarchar(max) NULL,
    [TokenIdentifier] nvarchar(100) NULL
)
```

### Fields Explanation
- **ID**: Primary key, auto-increment
- **MemberID**: Foreign key to Member table (user identifier)
- **LastActivityTime**: UTC timestamp of last user activity
- **CreatedAt**: UTC timestamp when session was created
- **ExpiresAt**: Optional expiration time for the session
- **IpAddress**: Optional field to track user's IP address
- **UserAgent**: Optional field to track user's browser/client
- **TokenIdentifier**: Optional unique identifier for the JWT token

---

## 📁 Files Created/Modified

### New Files
1. **Allinone.Domain/Sessions/UserSession.cs** - Entity model
2. **Allinone.DLL/Repositories/UserSessionRepository.cs** - Data access layer
3. **Allinone.API/Services/SessionCleanupService.cs** - Background service for cleanup

### Modified Files
1. **Allinone.API/Services/IdleTimeTrackingService.cs** - Changed from cache to database
2. **Allinone.API/Events/CustomBearerEvents.cs** - Updated to use async methods
3. **Allinone.API/Controllers/SessionController.cs** - Updated to use async methods
4. **Allinone.API/Middleware/ActivityTrackingMiddleware.cs** - Updated to use async methods
5. **Allinone.DLL/Data/DSContext.cs** - Added UserSession DbSet
6. **Allinone.API/Program.cs** - Registered repository and background service

---

## 🔄 How It Works

### 1. User Login
```
User logs in → Token generated → No session record yet
```

### 2. First Authenticated Request
```
Request arrives → CustomBearerEvents.TokenValidated() triggered
→ Check database for session (not found)
→ Create new UserSession record with LastActivityTime = NOW
→ Continue processing request
```

### 3. Subsequent Requests (Active User)
```
Request arrives → CustomBearerEvents.TokenValidated() triggered
→ Get session from database
→ Calculate: NOW - LastActivityTime
→ If < IdleTimeout: Update LastActivityTime = NOW and continue
→ If > IdleTimeout: Return 401 "Session-Expired-Idle"
```

### 4. Session Cleanup (Background)
```
Every 1 hour → SessionCleanupService runs
→ Deletes all sessions where ExpiresAt < NOW
→ Keeps database clean
```

---

## 🔍 Database Queries

### Check Active Sessions
```sql
SELECT * FROM UserSession 
WHERE LastActivityTime > DATEADD(MINUTE, -30, GETUTCDATE())
```

### Find Idle Sessions
```sql
SELECT MemberID, LastActivityTime, 
       DATEDIFF(MINUTE, LastActivityTime, GETUTCDATE()) AS MinutesIdle
FROM UserSession
WHERE DATEDIFF(MINUTE, LastActivityTime, GETUTCDATE()) > 30
```

### Session Count by User
```sql
SELECT MemberID, COUNT(*) AS SessionCount
FROM UserSession
GROUP BY MemberID
```

### Clean Up Old Sessions
```sql
DELETE FROM UserSession
WHERE LastActivityTime < DATEADD(HOUR, -24, GETUTCDATE())
```

---

## 📊 Performance Comparison

### In-Memory Cache (Old)
```
- Read: ~1ms
- Write: ~1ms
- Total overhead per request: ~2ms
```

### Database (New)
```
- Read: ~10-30ms (depends on DB)
- Write: ~10-30ms (depends on DB)
- Total overhead per request: ~20-60ms
```

### Performance Tips
1. **Add index** on MemberID for faster lookups:
   ```sql
   CREATE INDEX IX_UserSession_MemberID ON UserSession(MemberID)
   ```

2. **Add index** on LastActivityTime for cleanup queries:
   ```sql
   CREATE INDEX IX_UserSession_LastActivityTime ON UserSession(LastActivityTime)
   ```

3. **Connection pooling** - Already enabled by default in EF Core

---

## ✅ Benefits of Database Approach

### ✓ Persistence
- Sessions survive server restarts
- Users don't need to re-login after deployment

### ✓ Scalability
- Works with multiple servers behind load balancer
- Centralized session storage

### ✓ Auditability
- Track user login patterns
- Analyze session duration
- Identify inactive users

### ✓ Security
- Can force logout specific users
- Can view all active sessions
- Can implement "logout all devices" feature

### ✓ Flexibility
- Store additional metadata (IP, UserAgent)
- Implement session limits per user
- Add session history tracking

---

## 🎛️ Configuration

### appsettings.json
```json
{
  "Jwt": {
    "IdleTimeoutMinutes": 30
  }
}
```

### appsettings.Development.json
```json
{
  "Jwt": {
    "IdleTimeoutMinutes": 15
  }
}
```

---

## 🔧 API Endpoints

### Check Session Status
```http
GET /session/status
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "userId": "1",
    "lastActivity": "2026-01-10T03:45:00Z",
    "timeSinceLastActivity": "00:05:30",
    "idleTimeoutMinutes": 30,
    "isActive": true,
    "remainingTime": "00:24:30",
    "message": "Session is active"
  }
}
```

### Manually Refresh Session
```http
POST /session/refresh
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "userId": "1",
    "lastActivity": "2026-01-10T03:50:00Z",
    "message": "Activity refreshed successfully"
  }
}
```

---

## 🧪 Testing

### Test Scenario 1: Normal Flow
```bash
# 1. Login
POST /member/loginV2
Body: { "name": "john", "password": "pass123" }
# Save the token

# 2. Make request (creates session)
GET /todolist
Authorization: Bearer {token}
# Check database: SELECT * FROM UserSession

# 3. Wait 10 minutes, make another request
GET /todolist
Authorization: Bearer {token}
# Session updated, still active

# 4. Wait 35 minutes (> idle timeout)
GET /todolist
Authorization: Bearer {token}
# Expected: 401 "Session-Expired-Idle"
```

### Test Scenario 2: Server Restart
```bash
# 1. Login and create session
# 2. Restart server
# 3. Make request with same token
# Expected: Session still exists in database, request succeeds
```

---

## 📈 Database Monitoring

### Check Active Users
```sql
SELECT COUNT(DISTINCT MemberID) AS ActiveUsers
FROM UserSession
WHERE LastActivityTime > DATEADD(MINUTE, -30, GETUTCDATE())
```

### Average Session Duration
```sql
SELECT AVG(DATEDIFF(MINUTE, CreatedAt, LastActivityTime)) AS AvgDurationMinutes
FROM UserSession
WHERE CreatedAt >= DATEADD(DAY, -7, GETUTCDATE())
```

### Sessions by Hour
```sql
SELECT DATEPART(HOUR, LastActivityTime) AS Hour,
       COUNT(*) AS SessionCount
FROM UserSession
WHERE LastActivityTime >= DATEADD(DAY, -1, GETUTCDATE())
GROUP BY DATEPART(HOUR, LastActivityTime)
ORDER BY Hour
```

---

## 🚨 Troubleshooting

### Issue: Sessions not updating
**Check:**
1. CustomBearerEvents is being called
2. Repository is registered in DI
3. Database connection is working
4. Check logs for exceptions

### Issue: Performance degradation
**Solutions:**
1. Add database indexes (see Performance Tips)
2. Implement read-through cache (hybrid approach)
3. Optimize queries
4. Check database connection pooling

### Issue: Too many old sessions
**Solutions:**
1. SessionCleanupService is running (check logs)
2. Manually clean: `DELETE FROM UserSession WHERE LastActivityTime < DATEADD(DAY, -7, GETUTCDATE())`
3. Adjust cleanup interval if needed

---

## 🔮 Future Enhancements

### 1. Hybrid Approach (Cache + Database)
- Cache for reads (fast)
- Database for writes (persistent)
- Best of both worlds

### 2. Session History
- Keep audit trail of all sessions
- Track login/logout times
- Useful for security audits

### 3. Multiple Sessions per User
- Allow multiple devices
- Track device information
- Implement "logout all devices"

### 4. Redis Integration
- Replace database with Redis for speed
- Still persistent, but much faster
- Good for high-traffic scenarios

### 5. Advanced Features
- IP-based session validation
- Device fingerprinting
- Suspicious activity detection
- Session timeout warnings

---

## 📝 Migration Notes

### From Cache to Database
The migration has been completed automatically:
- EF Core migration created: `20260110034718_AddUserSessionTable`
- Database updated successfully
- All existing functionality preserved
- No breaking changes to API

### Rollback (if needed)
```bash
# Revert to cache-based approach
cd Allinone.DLL
dotnet ef database update {previous-migration-name} --startup-project ../Allinone.API
dotnet ef migrations remove --startup-project ../Allinone.API
# Then restore old IdleTimeTrackingService.cs from git
```

---

## ✨ Summary

The database-based approach provides:
- ✅ **Persistence** - Sessions survive restarts
- ✅ **Scalability** - Works with load balancers
- ✅ **Auditability** - Track session history
- ✅ **Security** - Better control over sessions
- ✅ **Flexibility** - Easy to extend with new features

Trade-off: Slightly higher latency (~20-60ms per request) vs in-memory cache, but acceptable for most applications and outweighed by the benefits.
