CREATE TABLE dbo.PaymentMethod
(
    PaymentMethodId INT IDENTITY(1,1) NOT NULL,
    MethodCode NVARCHAR(50) NOT NULL,
    MethodName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(250) NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_PaymentMethod_IsActive DEFAULT (1),

    DisplayOrder INT NOT NULL
        CONSTRAINT DF_PaymentMethod_DisplayOrder DEFAULT (0),

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_PaymentMethod_CreatedAt
        DEFAULT (SYSUTCDATETIME()),

    CreatedBy BIGINT NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,

    CONSTRAINT PK_PaymentMethod
        PRIMARY KEY (PaymentMethodId),

    CONSTRAINT UQ_PaymentMethod_MethodCode
        UNIQUE (MethodCode)
);
    