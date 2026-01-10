# Admin Controller Documentation

## Overview
The AdminController provides administrative endpoints for managing users and sessions. All endpoints require the user to have the "Admin" role.

## Authentication
All admin endpoints require:
- Valid JWT token in Authorization header: `Bearer <token>`
- User must have `Role = "Admin"` claim in the token

## Endpoints

### 1. Get All Members
**GET** `/api/admin/members`

Returns a list of all members with their details and session status.

**Response:**
```json
[
  {
    "id": 1,
    "name": "john_doe",
    "role": "User",
    "isActive": true,
    "lastLoginDate": "2025-01-10T10:30:00Z",
    "hasActiveSession": true,
    "lastActivityTime": "2025-01-10T11:45:00Z"
  }
]
```

---

### 2. Get Member Details
**GET** `/api/admin/members/{id}`

Returns detailed information about a specific member.

**Parameters:**
- `id` (path): Member ID

**Response:**
```json
{
  "id": 1,
  "name": "john_doe",
  "role": "User",
  "isActive": true,
  "lastLoginDate": "2025-01-10T10:30:00Z",
  "hasActiveSession": true,
  "sessionDetails": {
    "createdAt": "2025-01-10T10:30:00Z",
    "lastActivityTime": "2025-01-10T11:45:00Z",
    "expiresAt": "2025-01-10T11:30:00Z",
    "ipAddress": "192.168.1.100",
    "userAgent": "Mozilla/5.0..."
  }
}
```

---

### 3. Delete Member
**DELETE** `/api/admin/members/{id}`

Deletes a member and their associated session.

**Parameters:**
- `id` (path): Member ID

**Response:**
```json
{
  "message": "Member deleted successfully",
  "deletedMemberId": 1
}
```

---

### 4. Get All Sessions
**GET** `/api/admin/sessions`

Returns all active user sessions.

**Response:**
```json
[
  {
    "id": 1,
    "memberId": 5,
    "memberName": "john_doe",
    "createdAt": "2025-01-10T10:30:00Z",
    "lastActivityTime": "2025-01-10T11:45:00Z",
    "expiresAt": "2025-01-10T11:30:00Z",
    "ipAddress": "192.168.1.100",
    "userAgent": "Mozilla/5.0...",
    "isExpired": false,
    "minutesSinceActivity": 5
  }
]
```

---

### 5. Get User Session
**GET** `/api/admin/sessions/{memberId}`

Returns session details for a specific user.

**Parameters:**
- `memberId` (path): Member ID

**Response:**
```json
{
  "id": 1,
  "memberId": 5,
  "memberName": "john_doe",
  "createdAt": "2025-01-10T10:30:00Z",
  "lastActivityTime": "2025-01-10T11:45:00Z",
  "expiresAt": "2025-01-10T11:30:00Z",
  "ipAddress": "192.168.1.100",
  "userAgent": "Mozilla/5.0...",
  "isExpired": false,
  "idleMinutes": 5
}
```

---

### 6. Force Logout User
**DELETE** `/api/admin/sessions/{memberId}`

Forces a user to logout by removing their session.

**Parameters:**
- `memberId` (path): Member ID

**Response:**
```json
{
  "message": "User logged out successfully",
  "memberId": 5
}
```

---

### 7. Get System Statistics
**GET** `/api/admin/stats`

Returns system-wide statistics about users and sessions.

**Response:**
```json
{
  "totalMembers": 150,
  "activeMembers": 125,
  "inactiveMembers": 25,
  "adminCount": 3,
  "userCount": 147,
  "totalSessions": 89,
  "activeSessions": 78,
  "expiredSessions": 11,
  "averageSessionDurationMinutes": 42.5,
  "recentLogins24h": 23
}
```

---

### 8. Cleanup Expired Sessions
**POST** `/api/admin/sessions/cleanup`

Manually triggers cleanup of expired sessions.

**Response:**
```json
{
  "message": "Expired sessions cleaned up",
  "deletedCount": 11
}
```

---

### 9. Update Member Password
**PUT** `/api/admin/members/{id}/password`

Updates a member's password. Optionally forces logout after change.

**Parameters:**
- `id` (path): Member ID

**Request Body:**
```json
{
  "newPassword": "newSecurePassword123",
  "forceLogout": true
}
```

**Response:**
```json
{
  "message": "Password updated successfully",
  "memberId": 5,
  "loggedOut": true
}
```

---

## Setting Up Admin Users

### 1. Create Initial Admin User
To create your first admin user, you need to manually update the database:

```sql
-- Update an existing user to admin
UPDATE Member 
SET Role = 'Admin' 
WHERE Name = 'your_admin_username';

-- Verify the change
SELECT ID, Name, Role, IsActive FROM Member WHERE Role = 'Admin';
```

### 2. Login with Admin Credentials
```http
POST /api/member/loginv2
Content-Type: application/json

{
  "name": "your_admin_username",
  "password": "your_password"
}
```

The response will include a JWT token with the "Admin" role claim:
```json
{
  "id": 1,
  "name": "your_admin_username",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "lastLoginDate": "2025-01-10T10:30:00Z"
}
```

### 3. Use Admin Token
Include the token in all admin requests:
```http
GET /api/admin/members
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## Error Responses

### 401 Unauthorized
Returned when no token is provided or token is invalid:
```json
{
  "message": "Unauthorized"
}
```

### 403 Forbidden
Returned when user is authenticated but doesn't have Admin role:
```json
{
  "message": "Forbidden: Admin role required"
}
```

### 404 Not Found
Returned when requested resource doesn't exist:
```json
{
  "message": "Member not found"
}
```

### 400 Bad Request
Returned when request data is invalid:
```json
{
  "message": "Invalid request data",
  "errors": ["Password must be at least 6 characters"]
}
```

---

## Security Considerations

1. **Role-Based Access**: Only users with `Role = "Admin"` can access these endpoints
2. **Session Validation**: Admin tokens are also subject to idle timeout validation
3. **Password Management**: Admins can update passwords but cannot retrieve existing passwords
4. **Audit Logging**: All admin actions should be logged (consider adding audit trail)

---

## Integration with Idle Time Tracking

Admin sessions are subject to the same idle timeout rules as regular users:
- **Idle Timeout**: 30 minutes (configurable via `Jwt:IdleTimeoutMinutes`)
- **Session Expiry**: 60 minutes (configurable via `Jwt:ExpireMinutes`)
- Admin sessions are automatically cleaned up by the background `SessionCleanupService`

---

## Example Usage Scenarios

### Scenario 1: Monitor Active Users
```bash
# Get all members with session status
curl -X GET "https://localhost:7096/api/admin/members" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN"
```

### Scenario 2: Force Logout Suspicious User
```bash
# Get user session details
curl -X GET "https://localhost:7096/api/admin/sessions/5" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN"

# Force logout
curl -X DELETE "https://localhost:7096/api/admin/sessions/5" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN"
```

### Scenario 3: Reset User Password
```bash
curl -X PUT "https://localhost:7096/api/admin/members/5/password" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "newPassword": "newSecurePassword123",
    "forceLogout": true
  }'
```

### Scenario 4: System Health Check
```bash
# Get system statistics
curl -X GET "https://localhost:7096/api/admin/stats" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN"

# Cleanup expired sessions
curl -X POST "https://localhost:7096/api/admin/sessions/cleanup" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN"
```

---

## Testing with Postman

1. **Import Environment Variables:**
   - `base_url`: https://localhost:7096
   - `admin_token`: (obtained from login)

2. **Create Collection:**
   - Add Authorization header: `Bearer {{admin_token}}`
   - Test each endpoint sequentially

3. **Test Flow:**
   1. Login as admin → Save token
   2. GET /api/admin/stats → Verify access
   3. GET /api/admin/members → List all users
   4. GET /api/admin/sessions → View active sessions
   5. Test error scenarios (non-admin token, invalid IDs)

---

## Future Enhancements

Potential improvements for the admin system:

1. **Audit Trail**: Log all admin actions with timestamps
2. **Bulk Operations**: Update multiple users at once
3. **Advanced Filtering**: Search/filter members by role, activity, etc.
4. **Session Analytics**: Detailed session duration charts
5. **Role Management**: API to promote/demote users
6. **Activity Logs**: View user action history
7. **IP Blocking**: Block specific IPs from accessing the system
8. **Two-Factor Authentication**: Require 2FA for admin users
