IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'UserRole'
      AND schema_id = SCHEMA_ID('dbo')
)
CREATE TABLE UserRole
(

    UserRoleId BIGINT IDENTITY(1,1) NOT NULL,
    ClientId BIGINT NOT NULL,
    UserId BIGINT NOT NULL,
    RoleId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NULL,

    CONSTRAINT PK_UserRole
        PRIMARY KEY (UserRoleId),

    CONSTRAINT FK_UserRole_User
        FOREIGN KEY (UserId)
        REFERENCES [User](UserId),

    CONSTRAINT FK_UserRole_Role
        FOREIGN KEY (RoleId)
        REFERENCES Role(RoleId),

    CONSTRAINT UQ_UserRole_User_Role
        UNIQUE (UserId, RoleId)
);