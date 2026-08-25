IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'State'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
CREATE TABLE dbo.[State] (
    StateId INT IDENTITY(1,1) PRIMARY KEY,                  -- Internal PK
    StateName NVARCHAR(100) NOT NULL,
    CountryId INT NOT NULL,        
    IsActive bit default 1,
    IsSynced bit default 0,
    CreatedAt DATETIME2 NOT NULL
        DEFAULT (SYSUTCDATETIME()),

    CreatedBy BIGINT NULL,

    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,

    CONSTRAINT FK_State_Country FOREIGN KEY (CountryId)-- FK to Country
        REFERENCES Country(CountryId)
);
END

--CREATE UNIQUE INDEX IX_State_State ON State(StateId);