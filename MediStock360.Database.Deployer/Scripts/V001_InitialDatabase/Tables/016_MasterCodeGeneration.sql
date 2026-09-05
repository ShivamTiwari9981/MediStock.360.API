IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'MasterCodeGeneration'
      AND schema_id = SCHEMA_ID('dbo')
)
CREATE TABLE MasterCodeGeneration
(
    MasterCodeGenerationId BIGINT IDENTITY(1,1) PRIMARY KEY,
    ClientId BIGINT NOT NULL,
    StoreId BIGINT NULL,
    CodeType NVARCHAR(50) NOT NULL,
    CodePrefix NVARCHAR(20) NOT NULL,
    CurrentNumber BIGINT NOT NULL DEFAULT 0,
    NumberLength INT NOT NULL DEFAULT 3,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ModifiedDate DATETIME2 NULL,
    CONSTRAINT FK_MasterCodeGeneration_Client
        FOREIGN KEY (ClientId)
        REFERENCES Client(ClientId),

    CONSTRAINT FK_MasterCodeGeneration_Store
        FOREIGN KEY (StoreId)
        REFERENCES Store(StoreId)
);
