IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'Permission'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN

CREATE TABLE dbo.Permission
(
    PermissionId INT IDENTITY(1,1) NOT NULL,
    PermissionCode NVARCHAR(100) NOT NULL,
    PermissionName NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500) NULL,
    ModuleName NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL DEFAULT (1),
    CreatedAt DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    UpdatedAt DATETIME2 NULL,

    CONSTRAINT PK_Permission
        PRIMARY KEY (PermissionId),

    CONSTRAINT UQ_Permission_Code
        UNIQUE (PermissionCode)
);
END
