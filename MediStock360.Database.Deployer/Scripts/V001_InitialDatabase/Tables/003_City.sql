IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'City'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
CREATE TABLE dbo.City (
    CityId INT IDENTITY(1,1) PRIMARY KEY,                   -- Internal PK
    CityName NVARCHAR(100) NOT NULL,
    StateId INT NOT NULL,                              -- FK to State
    CountryId INT NOT NULL, 
    IsActive bit default 1,
    IsSynced bit default 0,
    CreatedAt DATETIME2 NOT NULL
        DEFAULT (SYSUTCDATETIME()),

    CreatedBy BIGINT NULL,

    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,
    CONSTRAINT FK_City_State FOREIGN KEY (StateId)-- FK to Country (optional but useful)
        REFERENCES State(StateId),
    CONSTRAINT FK_City_Country FOREIGN KEY (CountryId)
        REFERENCES Country(CountryId)
);

--CREATE UNIQUE INDEX IX_City_City ON City(CityId);
END