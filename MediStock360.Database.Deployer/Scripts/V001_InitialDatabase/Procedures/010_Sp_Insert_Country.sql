--EXEC [Sp_Insert_Country]

CREATE OR ALTER PROCEDURE [dbo].[Sp_Insert_Country]
AS
BEGIN
    SET NOCOUNT ON;
    
   /* =========================================================
   1. Insert Country
   ========================================================= */

DECLARE @IndiaCountryId INT;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.Country
    WHERE CountryName = N'India'
)
BEGIN
    INSERT INTO dbo.Country
    (
        CountryName,
        IsActive,
        IsSynced
    )
    VALUES
    (
        N'India',
        1,
        0
    );
END;

SELECT @IndiaCountryId = CountryId
FROM dbo.Country
WHERE CountryName = N'India';


/* =========================================================
   2. Insert States
   ========================================================= */

DECLARE @UttarPradeshStateId INT;
DECLARE @PunjabStateId INT;


/* Uttar Pradesh */

IF NOT EXISTS (
    SELECT 1
    FROM dbo.[State]
    WHERE StateName = N'Uttar Pradesh'
      AND CountryId = @IndiaCountryId
)
BEGIN
    INSERT INTO dbo.[State]
    (
        StateName,
        CountryId,
        IsActive,
        IsSynced
    )
    VALUES
    (
        N'Uttar Pradesh',
        @IndiaCountryId,
        1,
        0
    );
END;

SELECT @UttarPradeshStateId = StateId
FROM dbo.[State]
WHERE StateName = N'Uttar Pradesh'
  AND CountryId = @IndiaCountryId;


/* Punjab */

IF NOT EXISTS (
    SELECT 1
    FROM dbo.[State]
    WHERE StateName = N'Punjab'
      AND CountryId = @IndiaCountryId
)
BEGIN
    INSERT INTO dbo.[State]
    (
        StateName,
        CountryId,
        IsActive,
        IsSynced
    )
    VALUES
    (
        N'Punjab',
        @IndiaCountryId,
        1,
        0
    );
END;

SELECT @PunjabStateId = StateId
FROM dbo.[State]
WHERE StateName = N'Punjab'
  AND CountryId = @IndiaCountryId;


/* =========================================================
   3. Insert Cities
   ========================================================= */

/* Uttar Pradesh Cities */

IF NOT EXISTS (
    SELECT 1
    FROM dbo.City
    WHERE CityName = N'Bidhuna'
      AND StateId = @UttarPradeshStateId
)
BEGIN
    INSERT INTO dbo.City
    (
        CityName,
        StateId,
        CountryId,
        IsActive,
        IsSynced
    )
    VALUES
    (
        N'Bidhuna',
        @UttarPradeshStateId,
        @IndiaCountryId,
        1,
        0
    );
END;


IF NOT EXISTS (
    SELECT 1
    FROM dbo.City
    WHERE CityName = N'Etawah'
      AND StateId = @UttarPradeshStateId
)
BEGIN
    INSERT INTO dbo.City
    (
        CityName,
        StateId,
        CountryId,
        IsActive,
        IsSynced
    )
    VALUES
    (
        N'Etawah',
        @UttarPradeshStateId,
        @IndiaCountryId,
        1,
        0
    );
END;


/* Punjab Cities */

IF NOT EXISTS (
    SELECT 1
    FROM dbo.City
    WHERE CityName = N'Ludhiana'
      AND StateId = @PunjabStateId
)
BEGIN
    INSERT INTO dbo.City
    (
        CityName,
        StateId,
        CountryId,
        IsActive,
        IsSynced
    )
    VALUES
    (
        N'Ludhiana',
        @PunjabStateId,
        @IndiaCountryId,
        1,
        0
    );
END;


IF NOT EXISTS (
    SELECT 1
    FROM dbo.City
    WHERE CityName = N'Amritsar'
      AND StateId = @PunjabStateId
)
BEGIN
    INSERT INTO dbo.City
    (
        CityName,
        StateId,
        CountryId,
        IsActive,
        IsSynced
    )
    VALUES
    (
        N'Amritsar',
        @PunjabStateId,
        @IndiaCountryId,
        1,
        0
    );
END;


IF NOT EXISTS (
    SELECT 1
    FROM dbo.City
    WHERE CityName = N'Jalandhar'
      AND StateId = @PunjabStateId
)
BEGIN
    INSERT INTO dbo.City
    (
        CityName,
        StateId,
        CountryId,
        IsActive,
        IsSynced
    )
    VALUES
    (
        N'Jalandhar',
        @PunjabStateId,
        @IndiaCountryId,
        1,
        0
    );
END;
END


