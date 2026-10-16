
    -- Insert UPI and Bank Transfer if they do not exist
    INSERT INTO dbo.PaymentMethod
    (
        MethodCode,
        MethodName,
        Description,
        DisplayOrder
    )
    SELECT
        V.MethodCode,
        V.MethodName,
        V.Description,
        V.DisplayOrder
    FROM
    (
        VALUES
        (
            'UPI',
            'UPI / QR Code',
            'Pay using a UPI app or QR code.',
            1
        ),
        (
            'BANK_TRANSFER',
            'Direct Bank Transfer',
            'Pay directly to the registered bank account.',
            2
        )
    ) AS V (MethodCode, MethodName, Description, DisplayOrder)
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.PaymentMethod AS P WITH (UPDLOCK, HOLDLOCK)
        WHERE P.MethodCode = V.MethodCode
    );

    -- Insert Cash if it does not exist
    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.PaymentMethod WITH (UPDLOCK, HOLDLOCK)
        WHERE MethodCode = 'CASH'
    )
    BEGIN
        INSERT INTO dbo.PaymentMethod
        (
            MethodCode,
            MethodName,
            Description,
            DisplayOrder,
            IsActive
        )
        VALUES
        (
            'CASH',
            'Cash Payment',
            'Payment received directly in cash by the admin.',
            3,
            1
        );
    END;

