/* ============================================================
   Seed : MasterCodeGeneration
   Purpose : Initial code generation configuration
   ============================================================ */

SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    /* ========================================================
       CLIENT LEVEL CODES
       ClientId = NULL
       StoreId  = NULL
       ======================================================== */

    IF NOT EXISTS
    (
        SELECT 1
        FROM MasterCodeGeneration
        WHERE ClientId IS NULL
          AND StoreId IS NULL
          AND CodeType = 'Client'
    )
    BEGIN
        INSERT INTO MasterCodeGeneration(ClientId,StoreId,CodeType,CodePrefix,CurrentNumber,NumberLength,IsActive)
        VALUES(NULL,NULL,'Client','CLI',0,3,1);
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM MasterCodeGeneration
        WHERE ClientId IS NULL
          AND StoreId IS NULL
          AND CodeType = 'Role'
    )
    BEGIN
        INSERT INTO MasterCodeGeneration
        (
            ClientId,
            StoreId,
            CodeType,
            CodePrefix,
            CurrentNumber,
            NumberLength,
            IsActive
        )
        VALUES
        (
            NULL,
            NULL,
            'Role',
            'ROL',
            0,
            3,
            1
        );
    END;


    /* ========================================================
       CLIENT-SPECIFIC / STORE LEVEL CONFIGURATION

       These are templates/configurations.
       Actual ClientId and StoreId will be created dynamically
       when client/store is onboarded.

       Example:
       Product  -> PRD00001
       Employee -> EMP00001
       Invoice  -> INV000001
       ======================================================== */

    COMMIT TRANSACTION;

    PRINT 'MasterCodeGeneration seed completed successfully.';
END TRY
BEGIN CATCH

    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;