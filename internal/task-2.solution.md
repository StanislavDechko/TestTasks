---
title: "NotificationService API"
version: "1.0.0"
date: "2025-07-02"
---

# Overview
The `NotificationService` provides a unified interface for sending notifications via **Email**, **SMS**, and **Push** channels.  
It encapsulates the configuration and integration with external services such as SMTP (for email), Twilio (for SMS), and Firebase Cloud Messaging (for push notifications).

Each method returns a `NotificationResult`, which indicates whether the operation was successful or failed, and includes the following properties:
- `IsSuccess`: `true` if the notification was sent successfully; otherwise, `false`.
- `ErrorCode`: A string representing the type of error (e.g., `EmailError`, `SmsError`, `PushError`).
- `ErrorMessage`: A descriptive message explaining the reason for failure.

# API Reference
| Method                                    | Description                                        | Return Type                 |
|-------------------------------------------|----------------------------------------------------|-----------------------------|
| `SendEmailAsync(to, subject, body)`       | Sends an email using configured SMTP settings.      | `Task<NotificationResult>`  |
| `SendSmsAsync(phoneNumber, message)`      | Sends an SMS using the Twilio service.              | `Task<NotificationResult>`  |
| `SendPushAsync(deviceToken, title, body)` | Sends a push notification via Firebase Cloud Messaging. | `Task<NotificationResult>`  |

# Examples

## C# Usage

### Send Email
```csharp
var result = await notificationService.SendEmailAsync(
    to: "john@example.com",
    subject: "Welcome!",
    body: "Thank you for signing up."
);

if (!result.IsSuccess)
{
    Console.WriteLine($"Email failed: {result.ErrorCode} - {result.ErrorMessage}");
}
```

### Send SMS
```csharp
var result = await notificationService.SendSmsAsync(
    phoneNumber: "+1234567890",
    message: "Your verification code is 123456"
);

if (!result.IsSuccess)
{
    Console.WriteLine($"SMS failed: {result.ErrorCode} - {result.ErrorMessage}");
}
```

### Send Push Notification
```csharp
var result = await notificationService.SendPushAsync(
    deviceToken: "<FCM-Device-Token>",
    title: "Reminder",
    body: "Your session starts in 10 minutes."
);

if (!result.IsSuccess)
{
    Console.WriteLine($"Push failed: {result.ErrorCode} - {result.ErrorMessage}");
}
```

## API Example (cURL)

### Send SMS
```bash
curl -X POST https://api.myapp.com/notifications/sms \
  -H "Content-Type: application/json" \
  -d '{
        "phoneNumber": "+1234567890",
        "message": "Your appointment is confirmed."
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

# Error Codes
| Code        | Description                                 |
|-------------|---------------------------------------------|
| EmailError  | Email failed to send due to SMTP error.     |
| SmsError    | SMS failed to send via Twilio.              |
| PushError   | Push notification failed via Firebase.      |

# Notes
- YAML front matter is used to specify metadata like title, version, and date.
- Sections are organized in the following order: Overview, API Reference, Examples, Error Codes, and Notes.
- Code blocks are provided using specific language hints: `csharp`, `bash`, and `json`.
- Tables are used for the API Reference and Error Codes for clarity and quick lookup.
- `NotificationResult` is the unified return model for all notification methods and helps standardize success/error handling.