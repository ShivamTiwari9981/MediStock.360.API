SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    -- ============================================================
    -- App Settings - Global/System Level
    -- ClientId = NULL means these are global settings
    -- ============================================================

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.AppSetting
        WHERE 
           SettingKey = 'OTP_EXPIRY_MINUTES'
    )
    BEGIN
        INSERT INTO dbo.AppSetting
        (
            SettingKey,
            SettingValue,
            DataType,
            Description,
            IsActive
        )
        VALUES
        (
            'OTP_EXPIRY_MINUTES',
            '10',
            'INT',
            'OTP validity duration in minutes.',
            1
        );
    END;


    IF NOT EXISTS (
        SELECT 1
        FROM dbo.AppSetting Where
           SettingKey = 'OTP_LENGTH'
    )
    BEGIN
        INSERT INTO dbo.AppSetting
        (
            SettingKey,
            SettingValue,
            DataType,
            Description,
            IsActive
        )
        VALUES
        (
            'OTP_LENGTH',
            '6',
            'INT',
            'Number of digits in OTP.',
            1
        );
    END;


    IF NOT EXISTS (
        SELECT 1
        FROM dbo.AppSetting
        WHERE 
           SettingKey = 'OTP_MAX_ATTEMPTS'
    )
    BEGIN
        INSERT INTO dbo.AppSetting
        (
            SettingKey,
            SettingValue,
            DataType,
            Description,
            IsActive
        )
        VALUES
        (
            'OTP_MAX_ATTEMPTS',
            '5',
            'INT',
            'Maximum number of OTP verification attempts allowed.',
            1
        );
    END;


    IF NOT EXISTS (
        SELECT 1
        FROM dbo.AppSetting
        WHERE 
           SettingKey = 'OTP_RESEND_COOLDOWN_SECONDS'
    )
    BEGIN
        INSERT INTO dbo.AppSetting
        (
            SettingKey,
            SettingValue,
            DataType,
            Description,
            IsActive
        )
        VALUES
        (
            'OTP_RESEND_COOLDOWN_SECONDS',
            '60',
            'INT',
            'Minimum waiting time in seconds before requesting another OTP.',
            1
        );
    END;


    IF NOT EXISTS (
        SELECT 1
        FROM dbo.AppSetting
        WHERE 
           SettingKey = 'OTP_MAX_RESEND_ATTEMPTS'
    )
    BEGIN
        INSERT INTO dbo.AppSetting
        (
            SettingKey,
            SettingValue,
            DataType,
            Description,
            IsActive
        )
        VALUES
        (
            'OTP_MAX_RESEND_ATTEMPTS',
            '3',
            'INT',
            'Maximum number of OTP resend requests allowed within the configured period.',
            1
        );
    END;


    IF NOT EXISTS (
        SELECT 1
        FROM dbo.AppSetting
        WHERE 
           SettingKey = 'JWT_EXPIRY_MINUTES'
    )
    BEGIN
        INSERT INTO dbo.AppSetting
        (
            SettingKey,
            SettingValue,
            DataType,
            Description,
            IsActive
        )
        VALUES
        (
            'JWT_EXPIRY_MINUTES',
            '60',
            'INT',
            'JWT access token validity duration in minutes.',
            1
        );
    END;


    IF NOT EXISTS (
        SELECT 1
        FROM dbo.AppSetting
        WHERE 
           SettingKey = 'REFRESH_TOKEN_EXPIRY_DAYS'
    )
    BEGIN
        INSERT INTO dbo.AppSetting
        (
            SettingKey,
            SettingValue,
            DataType,
            Description,
            IsActive
        )
        VALUES
        (
            'REFRESH_TOKEN_EXPIRY_DAYS',
            '30',
            'INT',
            'Refresh token validity duration in days.',
            1
        );
    END;


    IF NOT EXISTS (
        SELECT 1
        FROM dbo.AppSetting
        WHERE
           SettingKey = 'PASSWORD_MIN_LENGTH'
    )
    BEGIN
        INSERT INTO dbo.AppSetting
        (
            SettingKey,
            SettingValue,
            DataType,
            Description,
            IsActive
        )
        VALUES
        (
            'PASSWORD_MIN_LENGTH',
            '8',
            'INT',
            'Minimum password length.',
            1
        );
    END;


    IF NOT EXISTS (
        SELECT 1
        FROM dbo.AppSetting
        WHERE 
           SettingKey = 'ACCOUNT_LOCKOUT_MAX_ATTEMPTS'
    )
    BEGIN
        INSERT INTO dbo.AppSetting
        (
            SettingKey,
            SettingValue,
            DataType,
            Description,
            IsActive
        )
        VALUES
        (
            'ACCOUNT_LOCKOUT_MAX_ATTEMPTS',
            '5',
            'INT',
            'Maximum failed login attempts before account lockout.',
            1
        );
    END;


    IF NOT EXISTS (
        SELECT 1
        FROM dbo.AppSetting
        WHERE 
           SettingKey = 'ACCOUNT_LOCKOUT_MINUTES'
    )
    BEGIN
        INSERT INTO dbo.AppSetting
        (
            SettingKey,
            SettingValue,
            DataType,
            Description,
            IsActive
        )
        VALUES
        (
            'ACCOUNT_LOCKOUT_MINUTES',
            '15',
            'INT',
            'Account lockout duration in minutes.',
            1
        );
    END;


    IF NOT EXISTS (
        SELECT 1
        FROM dbo.AppSetting
        WHERE 
           SettingKey = 'EMAIL_OTP_ENABLED'
    )
    BEGIN
        INSERT INTO dbo.AppSetting
        (
            SettingKey,
            SettingValue,
            DataType,
            Description,
            IsActive
        )
        VALUES
        (
            'EMAIL_OTP_ENABLED',
            'true',
            'BOOLEAN',
            'Determines whether OTP verification through email is enabled.',
            1
        );
    END;


    IF NOT EXISTS (
        SELECT 1
        FROM dbo.AppSetting
        WHERE 
          SettingKey = 'SMS_OTP_ENABLED'
    )
    BEGIN
        INSERT INTO dbo.AppSetting
        (
            SettingKey,
            SettingValue,
            DataType,
            Description,
            IsActive
        )
        VALUES
        (
            'SMS_OTP_ENABLED',
            'false',
            'BOOLEAN',
            'Determines whether OTP verification through SMS is enabled.',
            1
        );
    END;


    COMMIT TRANSACTION;

    PRINT 'AppSetting seed data inserted successfully.';

END TRY
BEGIN CATCH

    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;

END CATCH;