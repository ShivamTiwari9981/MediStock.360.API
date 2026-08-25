IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'AppSetting'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN

CREATE TABLE [dbo].[AppSetting](
	[AppSettingId] BIGINT IDENTITY(1,1) PRIMARY KEY,
	[ClientId] BIGINT not NULL,
	[SettingKey] [nvarchar](100) NOT NULL,
	[SettingValue] [nvarchar](500) NOT NULL,
	[DataType] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL DEFAULT (1),
	CreatedAt DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL
)
END