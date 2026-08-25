IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'Country'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
CREATE TABLE dbo.Country (
    CountryId INT IDENTITY PRIMARY KEY,                -- Internal PK
    CountryName NVARCHAR(100) NOT NULL,
    IsActive bit default 1,
    IsSynced bit default 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,
);

CREATE UNIQUE INDEX IX_Country_CountryGuid ON Country(CountryId);
end
