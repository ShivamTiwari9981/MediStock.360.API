IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'NotificationTemplate'
      AND schema_id = SCHEMA_ID('dbo')
)

CREATE TABLE NotificationTemplate
(
    NotificationTemplateId BIGINT IDENTITY(1,1) PRIMARY KEY,

    TemplateCode NVARCHAR(100) NOT NULL UNIQUE,
    TemplateName NVARCHAR(200) NOT NULL,

    NotificationType NVARCHAR(20) NOT NULL,
    -- EMAIL, SMS

    Subject NVARCHAR(500) NULL,

    Body NVARCHAR(MAX) NOT NULL,

    IsActive BIT NOT NULL DEFAULT 1,

    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NULL
);
