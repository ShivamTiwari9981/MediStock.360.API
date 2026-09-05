IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'ClientAppSetting'
      AND schema_id = SCHEMA_ID('dbo')
)

CREATE TABLE ClientAppSetting
(
    ClientAppSettingId BIGINT IDENTITY(1,1) PRIMARY KEY,

    ClientId BIGINT NOT NULL,
    AppSettingId BIGINT NOT NULL,

    SettingValue NVARCHAR(MAX) NULL,

    IsActive BIT NOT NULL DEFAULT 1,

   CreatedBy BIGINT NULL,
   UpdatedAt DATETIME2 NULL,
   UpdatedBy BIGINT NULL

    CONSTRAINT FK_ClientAppSetting_Client
        FOREIGN KEY (ClientId)
        REFERENCES Client(ClientId),

    CONSTRAINT FK_ClientAppSetting_AppSetting
        FOREIGN KEY (AppSettingId)
        REFERENCES AppSetting(AppSettingId),

    CONSTRAINT UQ_ClientAppSetting
        UNIQUE (ClientId, AppSettingId)
);
