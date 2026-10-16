IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'SubscriptionPlan'
      AND schema_id = SCHEMA_ID('dbo')
)
CREATE TABLE dbo.SubscriptionPlan
(
    SubscriptionPlanId INT IDENTITY(1,1) NOT NULL,
    PlanCode NVARCHAR(50) NOT NULL,
    PlanName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,

    BillingCycle TINYINT NOT NULL, -- 1 = Monthly, 2 = Yearly
    Price DECIMAL(18,2) NOT NULL,
    CurrencyCode CHAR(3) NOT NULL
        CONSTRAINT DF_SubscriptionPlan_CurrencyCode DEFAULT ('INR'),

    TrialDays INT NULL, -- Example: FREE plan = 30 days

    IsActive BIT NOT NULL
        CONSTRAINT DF_SubscriptionPlan_IsActive DEFAULT (1),
    IsSynced BIT NOT NULL
        CONSTRAINT DF_SubscriptionPlan_IsSynced DEFAULT (0),

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_SubscriptionPlan_CreatedAt
        DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,

    CONSTRAINT PK_SubscriptionPlan
        PRIMARY KEY (SubscriptionPlanId),

    CONSTRAINT UQ_SubscriptionPlan_PlanCode
        UNIQUE (PlanCode),

    CONSTRAINT CK_SubscriptionPlan_BillingCycle
        CHECK (BillingCycle IN (1, 2)),

    CONSTRAINT CK_SubscriptionPlan_Price
        CHECK (Price >= 0),

    CONSTRAINT CK_SubscriptionPlan_TrialDays
        CHECK (TrialDays IS NULL OR TrialDays >= 0)
);