
CREATE TABLE dbo.ClientPayment
(
    ClientPaymentId BIGINT IDENTITY(1,1) NOT NULL,

    ClientId BIGINT NOT NULL,
    ClientSubscriptionId BIGINT NOT NULL,
    PaymentConfigurationId BIGINT NOT NULL,

    Amount DECIMAL(18,2) NOT NULL,
    CurrencyCode CHAR(3) NOT NULL
        CONSTRAINT DF_ClientPayment_CurrencyCode DEFAULT ('INR'),

    TransactionReference NVARCHAR(150) NOT NULL,
    SenderNotes NVARCHAR(250) NULL,

    -- 1 = Pending, 2 = Approved, 3 = Rejected
    PaymentStatus TINYINT NOT NULL
        CONSTRAINT DF_ClientPayment_PaymentStatus DEFAULT (1),

    AdminRemarks NVARCHAR(500) NULL,
    ReviewedBy BIGINT NULL,
    ReviewedAt DATETIME2 NULL,

    SubmittedAt DATETIME2 NOT NULL
        CONSTRAINT DF_ClientPayment_SubmittedAt
        DEFAULT (SYSUTCDATETIME()),

    CreatedBy BIGINT NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,

    CONSTRAINT PK_ClientPayment
        PRIMARY KEY (ClientPaymentId),

    CONSTRAINT FK_ClientPayment_PaymentConfiguration
        FOREIGN KEY (PaymentConfigurationId)
        REFERENCES dbo.PaymentConfiguration (PaymentConfigurationId),

    CONSTRAINT CK_ClientPayment_Amount
        CHECK (Amount > 0),

    CONSTRAINT CK_ClientPayment_Status
        CHECK (PaymentStatus IN (1, 2, 3))
);
