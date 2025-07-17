---
title: "NotificationService API"
version: "1.0.0"
date: "2025-07-17"
---

# Overview
Service for sending notifications via Email, SMS, and Push using SMTP, Twilio, and Firebase Cloud Messaging respectively. Implements `INotificationService` interface.

# Constructor
```csharp
public NotificationService(
    ILogger<NotificationService> logger,
    SmtpSettings smtp,
    TwilioSettings twilio,
    FirebaseMessaging firebase)
```

|Parameter|	Description|
|----------|---------|
|logger|	Logger instance for diagnostic messages|
|smtp|	SMTP server configuration (host, port, credentials)|
|twilio|	Twilio account credentials (SID, auth token, from number)|
|firebase|	Configured Firebase Messaging instance|
# API Reference
|Method|	Description|	Return Type|
|-------|----|---|
|SendEmailAsync(to, subject, body)|	Sends email via SMTP|	Task<NotificationResult>|
|SendSmsAsync(phoneNumber, message)	| Sends SMS via Twilio|	Task<NotificationResult>|
|SendPushAsync(deviceToken, title, body)|	Sends push notification via FCM	|Task<NotificationResult>|
# Examples

## C# Usage

### Email Notification
```csharp
var result = await notificationService.SendEmailAsync(
    "user@example.com",
    "Account Verification",
    "Please confirm your email by clicking the link below..."
);

if (!result.IsSuccess)
{
    Console.WriteLine($"Failed: {result.ErrorCode} - {result.ErrorMessage}");
}
```
### SMS Notification
```csharp
var result = await notificationService.SendSmsAsync(
    "+15551234567",
    "Your verification code is: 123456"
);
```
### Push Notification
```csharp
var result = await notificationService.SendPushAsync(
    "device_fcm_token_abc123",
    "New Message",
    "You have 3 unread messages"
);
```

## API Example (cURL)

### Send Email
```bash
curl -X POST https://api.example.com/notifications/email \
  -H "Content-Type: application/json" \
  -d '{
    "to": "recipient@domain.com",
    "subject": "Urgent: Action Required",
    "body": "Your account requires verification"
  }'
```

### Send SMS
```bash
curl -X POST https://api.example.com/notifications/sms \
  -H "Content-Type: application/json" \
  -d '{
    "phoneNumber": "+15559876543",
    "message": "Your appointment is confirmed"
  }'
```

### Send Push
```bash
curl -X POST https://api.example.com/notifications/push \
  -H "Content-Type: application/json" \
  -d '{
    "deviceToken": "fcm-token-xyz789",
    "title": "Security Alert",
    "body": "New login detected"
  }'
```

### Expected JSON Response
```json
{
  "isSuccess": true,
  "errorCode": null,
  "errorMessage": null
}
```

### Failure Response Example
```json
{
  "isSuccess": false,
  "errorCode": "SmsError",
  "errorMessage": "Invalid phone number format"
}
```

# Error Handling
|Error Code|	Description|
|---|---|
|EmailError|	SMTP delivery failure (network/auth issues)|
|SmsError	| Twilio API failure (invalid number/credit) |
| PushError	| FCM delivery failure (invalid/expired token)|

# Implementation Notes
- All methods return NotificationResult with success/failure status
- Failures include error codes for programmatic handling
- Detailed errors are logged via ILogger:
- Information level for successful deliveries
- Error level for failures with full exception details
- Twilio client initialized once during service construction
- SMTP connections are disposed after each email send
