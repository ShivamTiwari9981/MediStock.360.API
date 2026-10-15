IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'SubscriptionFeature'
      AND schema_id = SCHEMA_ID('dbo')
)

CREATE TABLE dbo.SubscriptionFeature
(
    SubscriptionFeatureId INT IDENTITY(1,1) NOT NULL,
    FeatureCode NVARCHAR(50) NOT NULL,
    FeatureName NVARCHAR(100) NOT NULL,
    DisplayTemplate NVARCHAR(200) NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_SubscriptionFeature_IsActive DEFAULT (1),
    IsSynced BIT NOT NULL
        CONSTRAINT DF_SubscriptionFeature_IsSynced DEFAULT (0),

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_SubscriptionFeature_CreatedAt
        DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,

    CONSTRAINT PK_SubscriptionFeature
        PRIMARY KEY (SubscriptionFeatureId),

    CONSTRAINT UQ_SubscriptionFeature_FeatureCode
        UNIQUE (FeatureCode)
);