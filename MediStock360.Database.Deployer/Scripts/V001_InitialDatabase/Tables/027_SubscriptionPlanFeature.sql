IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'SubscriptionPlanFeature'
      AND schema_id = SCHEMA_ID('dbo')
)
CREATE TABLE dbo.SubscriptionPlanFeature
(
    SubscriptionPlanFeatureId BIGINT IDENTITY(1,1) NOT NULL,

    SubscriptionPlanId INT NOT NULL,
    SubscriptionFeatureId INT NOT NULL,

    FeatureValue INT NULL, -- Example: 5 stores or 100 products
    IsEnabled BIT NOT NULL
        CONSTRAINT DF_SubscriptionPlanFeature_IsEnabled
        DEFAULT (1),

    DisplayOrder INT NOT NULL
        CONSTRAINT DF_SubscriptionPlanFeature_DisplayOrder
        DEFAULT (0),

    CONSTRAINT PK_SubscriptionPlanFeature
        PRIMARY KEY (SubscriptionPlanFeatureId),

    CONSTRAINT FK_SubscriptionPlanFeature_Plan
        FOREIGN KEY (SubscriptionPlanId)
        REFERENCES dbo.SubscriptionPlan (SubscriptionPlanId),

    CONSTRAINT FK_SubscriptionPlanFeature_Feature
        FOREIGN KEY (SubscriptionFeatureId)
        REFERENCES dbo.SubscriptionFeature (SubscriptionFeatureId),

    CONSTRAINT UQ_SubscriptionPlanFeature_Plan_Feature
        UNIQUE (SubscriptionPlanId, SubscriptionFeatureId),

    CONSTRAINT CK_SubscriptionPlanFeature_FeatureValue
        CHECK (FeatureValue IS NULL OR FeatureValue >= 0)
);