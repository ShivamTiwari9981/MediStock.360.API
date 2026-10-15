CREATE OR ALTER PROCEDURE dbo.Sp_PaymentMethodes
AS
BEGIN
    SELECT
    PaymentMethodId,
    MethodCode,
    MethodName,
    Description,
    IsActive,
    DisplayOrder    
    FROM dbo.PaymentMethod
    where IsActive =1
    ORDER BY
       DisplayOrder


    Select 
    PaymentConfigurationId,
    PC.PaymentMethodId,
    ConfigurationCode,
    ConfigurationName,
    UpiId,
    UpiNumber,
    QRCodeUrl,
    AccountHolderName,
    AccountNumber,
    IFSCCode,
    BankName,
    PaymentModeLabel,
    BadgeText,
    CollectionInstructions,
    ContactNumber,
    ContactLabel,
    Instructions,
    PC.IsActive,
    PC.DisplayOrder

    from PaymentConfiguration as PC inner join PaymentMethod PM on Pc.PaymentMethodId= PM.PaymentMethodId
    where PC.IsActive =1
    order by PC.DisplayOrder


END


			