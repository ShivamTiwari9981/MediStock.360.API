INSERT INTO NotificationTemplate
(
    TemplateCode,
    TemplateName,
    NotificationType,
    Subject,
    Body
)
VALUES
(
    'EMAIL_OTP_VERIFICATION',
    'Email OTP Verification',
    'EMAIL',
    'Your MediStock360 Verification Code',
    'Hello {{UserName}},

Your One-Time Password (OTP) for MediStock360 is:

{{OTP}}

This OTP is valid for {{ExpiryMinutes}} minutes.

Please do not share this OTP with anyone.

If you did not request this verification code, please ignore this email.

Regards,
MediStock360 Team'
);