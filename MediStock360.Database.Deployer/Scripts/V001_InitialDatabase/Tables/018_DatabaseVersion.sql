IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'DatabaseVersion'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN

CREATE TABLE dbo.DatabaseVersion
(
    DatabaseVersionId BIGINT IDENTITY(1,1) PRIMARY KEY,

    VersionNumber INT NOT NULL UNIQUE,

    Description NVARCHAR(500) NULL,

    AppliedAt DATETIME2 NOT NULL
        DEFAULT SYSUTCDATETIME()
);
END