# JWT Authentication 401 Error - Troubleshooting Guide

## Problem Fixed
The 401 error was caused by **mismatched JWT configuration** between token generation and validation.

### Root Cause
- **Token Generation** (JWTHelper.cs) was using hardcoded values:
  - Issuer: `"yourIssuer"`
  - Audience: `"yourAudience"`
  - Key: `"your_super_secret_key_that_is_long_enough_123!"`

- **Token Validation** (Program.cs) was using appsettings.json values:
  - Issuer: `"ProductServiceApp"`
  - Audience: `"ProductServiceUsers"`
  - Key: `"YourSuperSecretKeyHere123!"`

## Changes Made

### 1. Updated JWTHelper.cs
- Now accepts `IConfiguration` parameter
- Reads values from appsettings.json
- Falls back to correct defaults if configuration is unavailable

### 2. Updated MemberService.cs
- Injects `IConfiguration` via constructor
- Passes configuration to `JWTHelper.GenerateJwtToken()`

### 3. Enhanced JwtDebugMiddleware.cs
- Added proper logging with `ILogger`
- Decodes and displays token information
- Shows token issuer, audience, expiration, and claims
- Helps identify authentication failures

### 4. Created AuthTestController
New debugging endpoints to help troubleshoot authentication:

## How to Test

### Step 1: Check Configuration
```http
GET /api/authtest/config
```
Verify your JWT configuration settings.

### Step 2: Login
```http
POST /member/loginV2
Content-Type: application/json

{
  "name": "your-username",
  "password": "your-password"
}
```
Save the token from the response.

### Step 3: Decode Token (Optional)
```http
POST /api/authtest/decode-token
Content-Type: application/json

{
  "token": "your-jwt-token-here"
}
```
This will show you what's inside the token and if it matches expected configuration.

### Step 4: Test Public Endpoint
```http
GET /api/authtest/public
```
Should work without authentication.

### Step 5: Test Protected Endpoint
```http
GET /api/authtest/protected
Authorization: Bearer your-jwt-token-here
```
Should return your user information if token is valid.

### Step 6: Test Your Actual Endpoint
```http
GET /todolist
Authorization: Bearer your-jwt-token-here
```
Should now work!

## Common Issues to Check

### 1. Missing Authorization Header
Make sure you include the header:
```
Authorization: Bearer your-jwt-token-here
```
Note: "Bearer " prefix is required!

### 2. Token Format
- The token should be a long string of characters (JWT format)
- Example: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

### 3. Token Expiration
- Default expiration is 60 minutes
- Check the logs to see if token is expired

### 4. Check Logs
The enhanced JwtDebugMiddleware will log:
```
==== Incoming Request ====
Path: /todolist
Method: GET
Authorization Header Present: Bearer eyJhbGci...
Token Issuer: ProductServiceApp
Token Audience: ProductServiceUsers
Token Expires: 2026-01-03T11:30:00Z
Token IsExpired: False
Token Claims:
  - sub: username
  - MemberId: 123
✓ User is authenticated.
==========================
```

If you see:
```
✗ User is NOT authenticated. Response Status: 401
```
Then check the token claims and configuration match.

## Debugging Steps

1. **Check the console logs** - The middleware will show detailed token information
2. **Use `/api/authtest/decode-token`** - Decode your token to see what's inside
3. **Use `/api/authtest/config`** - Verify your JWT configuration
4. **Compare token issuer/audience** with configuration values
5. **Check token expiration** - Tokens expire after configured time

## Expected Token Structure

Your token should contain these claims:
```json
{
  "sub": "username",
  "jti": "unique-guid",
  "MemberId": "123",
  "iss": "ProductServiceApp",
  "aud": "ProductServiceUsers",
  "exp": 1234567890
}
```

## Configuration (appsettings.json)
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

## Quick Fix Checklist
- ✅ Token is generated with correct issuer/audience
- ✅ Token is sent in Authorization header with "Bearer " prefix
- ✅ Token is not expired
- ✅ JWT configuration matches between generation and validation
- ✅ Endpoint has [Authorize] attribute
- ✅ Authentication middleware is registered in Program.cs
- ✅ Token contains "MemberId" claim