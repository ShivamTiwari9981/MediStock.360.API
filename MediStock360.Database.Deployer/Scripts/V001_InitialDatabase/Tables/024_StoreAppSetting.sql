IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'StoreAppSetting'
      AND schema_id = SCHEMA_ID('dbo')
)

CREATE TABLE StoreAppSetting
(
    StoreAppSettingId BIGINT IDENTITY(1,1) PRIMARY KEY,
    ClientId BIGINT NOT NULL,
    StoreId BIGINT NOT NULL,
    AppSettingId BIGINT NOT NULL,

    SettingValue NVARCHAR(MAX) NULL,

    IsActive BIT NOT NULL DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL

    CONSTRAINT FK_ClientAppSetting_Store
        FOREIGN KEY (ClientId)
        REFERENCES Client(ClientId),

    CONSTRAINT FK_StoreAppSetting_Store
        FOREIGN KEY (StoreId)
        REFERENCES Store(StoreId),

    CONSTRAINT FK_StoreAppSetting_AppSetting
        FOREIGN KEY (AppSettingId)
        REFERENCES AppSetting(AppSettingId),

    CONSTRAINT UQ_StoreAppSetting
        UNIQUE (StoreId, AppSettingId)
);
