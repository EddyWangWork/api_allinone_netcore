-- Performance Optimization: Add Indexes to UserSession Table

-- Index on MemberID for fast session lookups by user
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserSession_MemberID' AND object_id = OBJECT_ID('UserSession'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_UserSession_MemberID
    ON UserSession(MemberID)
    INCLUDE (LastActivityTime, CreatedAt)
END
GO

-- Index on LastActivityTime for cleanup queries and active session checks
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserSession_LastActivityTime' AND object_id = OBJECT_ID('UserSession'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_UserSession_LastActivityTime
    ON UserSession(LastActivityTime)
    WHERE LastActivityTime IS NOT NULL
END
GO

-- Index on ExpiresAt for expired session cleanup
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserSession_ExpiresAt' AND object_id = OBJECT_ID('UserSession'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_UserSession_ExpiresAt
    ON UserSession(ExpiresAt)
    WHERE ExpiresAt IS NOT NULL
END
GO

-- Composite index for active session queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserSession_MemberID_LastActivityTime' AND object_id = OBJECT_ID('UserSession'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_UserSession_MemberID_LastActivityTime
    ON UserSession(MemberID, LastActivityTime DESC)
END
GO

PRINT 'All indexes created successfully!'
GO

-- Verify indexes
SELECT 
    i.name AS IndexName,
    i.type_desc AS IndexType,
    COL_NAME(ic.object_id, ic.column_id) AS ColumnName,
    i.is_unique,
    i.fill_factor
FROM sys.indexes i
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
WHERE i.object_id = OBJECT_ID('UserSession')
ORDER BY i.name, ic.key_ordinal
GO
