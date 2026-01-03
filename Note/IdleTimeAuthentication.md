# Idle Time-Based Authentication

This project has been enhanced with idle time-based authentication that automatically logs out users after a period of inactivity, instead of relying solely on fixed JWT token expiration.

## Features

### 1. Idle Time Tracking
- Tracks user activity in real-time using an in-memory cache
- Updates last activity timestamp on every authenticated request
- Configurable idle timeout period

### 2. Enhanced JWT Events
- Custom `CustomBearerEvents` that validates idle time during token validation
- Returns specific error messages for idle session expiration vs token expiration
- Seamlessly integrates with existing JWT authentication

### 3. Session Management
- New `/api/session/status` endpoint to check current session status
- `/api/session/refresh` endpoint to manually refresh activity
- Detailed session information including remaining time

## Configuration

### appsettings.json
```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyHere123!",
    "Issuer": "ProductServiceApp",
    "Audience": "ProductServiceUsers",
    "ExpireMinutes": 60,
    "IdleTimeoutMinutes": 30
  }
}
```

### Development Settings
In `appsettings.Development.json`, you can override the idle timeout for development:
```json
{
  "Jwt": {
    "IdleTimeoutMinutes": 15
  }
}
```

## How It Works

1. **User Authentication**: User logs in and receives JWT token as usual
2. **Activity Tracking**: Every authenticated request updates the user's last activity timestamp
3. **Idle Validation**: Before processing each request, the system checks if the time since last activity exceeds the configured idle timeout
4. **Session Expiration**: If user has been idle too long, they receive a `Session-Expired-Idle` response
5. **Automatic Refresh**: Active users automatically refresh their activity timestamp

## Response Messages

- `Token-Expired`: JWT token has expired (traditional expiration)
- `Session-Expired-Idle`: User session expired due to inactivity

## Components Added

### Services
- `IIdleTimeTrackingService` / `IdleTimeTrackingService`: Core service for tracking user activity

### Events
- Enhanced `CustomBearerEvents`: JWT events with idle time validation

### Controllers
- `SessionController`: Endpoints for session status and manual refresh

### Middleware
- `ActivityTrackingMiddleware`: Optional middleware for additional activity tracking

### Configuration
- `JwtSettings`: Configuration model for JWT settings
- `AuthenticationExtensions`: Extension methods for cleaner setup

## Usage Examples

### Check Session Status
```http
GET /api/session/status
Authorization: Bearer {your-jwt-token}
```

Response:
```json
{
  "success": true,
  "data": {
    "userId": "123",
    "lastActivity": "2026-01-03T10:30:00Z",
    "timeSinceLastActivity": "00:05:30",
    "idleTimeoutMinutes": 30,
    "isActive": true,
    "remainingTime": "00:24:30",
    "message": "Session is active"
  }
}
```

### Refresh Activity
```http
POST /api/session/refresh
Authorization: Bearer {your-jwt-token}
```

## Benefits

1. **Enhanced Security**: Users are automatically logged out when inactive
2. **Better User Experience**: Active users don't need to re-authenticate frequently
3. **Configurable**: Different timeout periods for different environments
4. **Monitoring**: Built-in endpoints to check session status
5. **Backward Compatible**: Existing authentication flow remains unchanged

## Notes

- The idle timeout is independent of JWT token expiration
- Users can be active with expired tokens if they've been continuously active
- The system uses in-memory cache, so activity tracking is lost on server restart
- Consider implementing persistent storage (database/Redis) for production high-availability scenarios