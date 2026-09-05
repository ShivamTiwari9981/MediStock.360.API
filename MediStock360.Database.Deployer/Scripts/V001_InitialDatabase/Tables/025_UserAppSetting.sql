IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'UserAppSetting'
      AND schema_id = SCHEMA_ID('dbo')
)

CREATE TABLE UserAppSetting
(
    UserAppSettingId BIGINT IDENTITY(1,1) PRIMARY KEY,
    ClientId BIGINT NOT NULL,
    StoreId BIGINT NOT NULL,
    UserId BIGINT NOT NULL,
    AppSettingId BIGINT NOT NULL,

    SettingValue NVARCHAR(MAX) NULL,

    IsActive BIT NOT NULL DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL 

    CONSTRAINT FK_UserAppSetting_User
        FOREIGN KEY (UserId)
        REFERENCES [User](UserId),

    CONSTRAINT FK_UserAppSetting_AppSetting
        FOREIGN KEY (AppSettingId)
        REFERENCES AppSetting(AppSettingId),

    CONSTRAINT UQ_UserAppSetting
        UNIQUE (UserId, AppSettingId)
);
