IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'RolePermission'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
CREATE TABLE dbo.RolePermission
(
    RolePermissionId BIGINT IDENTITY(1,1) NOT NULL,
    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NULL,

    CONSTRAINT PK_RolePermission
        PRIMARY KEY (RolePermissionId),

    CONSTRAINT FK_RolePermission_Role
        FOREIGN KEY (RoleId)
        REFERENCES Role(RoleId),

    CONSTRAINT FK_RolePermission_Permission
        FOREIGN KEY (PermissionId)
        REFERENCES Permission(PermissionId),

    CONSTRAINT UQ_RolePermission_Role_Permission
        UNIQUE (RoleId, PermissionId)
);
END
