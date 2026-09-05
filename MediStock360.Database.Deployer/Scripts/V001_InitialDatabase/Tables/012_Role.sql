IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'Role'
      AND schema_id = SCHEMA_ID('dbo')
)
CREATE TABLE dbo.Role
(
    RoleId INT IDENTITY(1,1) NOT NULL,
    ClientId BIGINT NOT NULL,
    RoleCode NVARCHAR(50) NOT NULL,
    RoleName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    IsSystemRole BIT NOT NULL DEFAULT (0),
    IsActive BIT NOT NULL DEFAULT (1),
    CreatedAt DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,

    CONSTRAINT PK_Role
        PRIMARY KEY (RoleId),

    CONSTRAINT FK_Role_Client
        FOREIGN KEY (ClientId)
        REFERENCES Client(ClientId),

    CONSTRAINT UQ_Role_Client_RoleCode
        UNIQUE (ClientId, RoleCode)
);
