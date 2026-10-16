
    INSERT INTO dbo.PaymentConfiguration
    (
        PaymentMethodId,
        ConfigurationCode,
        ConfigurationName,
        PaymentModeLabel,
        BadgeText,
        CollectionInstructions,
        ContactNumber,
        ContactLabel,
        IsActive,
        DisplayOrder
    )
    SELECT
        PM.PaymentMethodId,
        'CASH_DEFAULT',
        'Direct Cash Payment',
        'Direct Cash Payment',
        'CASH IN HAND',
        'Pay directly at our office/store counter or to our authorized representative upon delivery.',
        '+91 7398377354',
        'Contact & Helpline',
        1,
        3
    FROM dbo.PaymentMethod PM
    WHERE PM.MethodCode = 'CASH'
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.PaymentConfiguration PC WITH (UPDLOCK, HOLDLOCK)
          WHERE PC.ConfigurationCode = 'CASH_DEFAULT'
      );

    -- UPI / QR PAYMENT
    INSERT INTO dbo.PaymentConfiguration
    (
        PaymentMethodId,
        ConfigurationCode,
        ConfigurationName,
        UpiId,
        UpiNumber,
        QRCodeUrl,
        PaymentModeLabel,
        BadgeText,
        CollectionInstructions,
        ContactNumber,
        ContactLabel,
        IsActive,
        DisplayOrder
    )
    SELECT
        PM.PaymentMethodId,
        'UPI_DEFAULT',
        'UPI / QR Code',
        'shivamtiwari8756@ybl',
        '9981985249',
        '/payment/upi-qr.jpeg',
        'UPI / QR Payment',
        'SCAN & PAY',
        'Scan the QR code or pay using the displayed UPI ID. Enter the UTR/reference number after payment.',
        '+91 7398377354',
        'Contact & Helpline',
        1,
        1
    FROM dbo.PaymentMethod PM
    WHERE PM.MethodCode = 'UPI'
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.PaymentConfiguration PC WITH (UPDLOCK, HOLDLOCK)
          WHERE PC.ConfigurationCode = 'UPI_DEFAULT'
      );

    -- BANK TRANSFER
    INSERT INTO dbo.PaymentConfiguration
    (
        PaymentMethodId,
        ConfigurationCode,
        ConfigurationName,
        AccountHolderName,
        AccountNumber,
        IFSCCode,
        BankName,
        PaymentModeLabel,
        BadgeText,
        CollectionInstructions,
        ContactNumber,
        ContactLabel,
        IsActive,
        DisplayOrder
    )
    SELECT
        PM.PaymentMethodId,
        'BANK_DEFAULT',
        'Direct Bank Transfer',
        'MediStock360',
        '157398377354',
        'ICIC0000164',
        'ICICI Bank',
        'Direct Bank Transfer',
        'BANK TRANSFER',
        'Transfer the payable amount to the bank account shown. Enter the transaction reference after completing the transfer.',
        '+91 7398377354',
        'Contact & Helpline',
        1,
        2
    FROM dbo.PaymentMethod PM
    WHERE PM.MethodCode = 'BANK_TRANSFER'
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.PaymentConfiguration PC WITH (UPDLOCK, HOLDLOCK)
          WHERE PC.ConfigurationCode = 'BANK_DEFAULT'
      );





