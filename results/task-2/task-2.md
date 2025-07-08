---
title: "NotificationService API"
version: "1.0.0"
date: "2025-01-27"
---

# Overview
A comprehensive notification service that provides unified interfaces for sending emails, SMS messages, and push notifications. The service integrates with SMTP for email delivery, Twilio for SMS messaging, and Firebase Cloud Messaging for push notifications.

# API Reference
| Method                                           | Description                                        | Return Type                   |
|--------------------------------------------------|----------------------------------------------------|-------------------------------|
| `SendEmailAsync(to, subject, body)`              | Sends an email using SMTP configuration.            | `Task<NotificationResult>`    |
| `SendSmsAsync(phoneNumber, message)`             | Sends an SMS using Twilio API.                      | `Task<NotificationResult>`    |
| `SendPushAsync(deviceToken, title, body)`        | Sends a push notification using Firebase FCM.       | `Task<NotificationResult>`    |

# Examples

## C# Usage
```csharp
// Send email notification
var emailResult = await notificationService.SendEmailAsync(
    "user@example.com",
    "Welcome to Our Service",
    "Thank you for registering with our platform!"
);
// Check success
if (emailResult.IsSuccess)
{
    Console.WriteLine("Email sent successfully");
}

// Send SMS notification
var smsResult = await notificationService.SendSmsAsync(
    "+1234567890",
    "Your verification code is: 123456"
);
// Check success
if (smsResult.IsSuccess)
{
    Console.WriteLine("SMS sent successfully");
}

// Send push notification
var pushResult = await notificationService.SendPushAsync(
    "device_token_here",
    "New Message",
    "You have received a new message from John"
);
// Check success
if (pushResult.IsSuccess)
{
    Console.WriteLine("Push notification sent successfully");
}
```

## Dependency Injection Setup
```csharp
// Configure services in Startup.cs or Program.cs
services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
services.Configure<TwilioSettings>(configuration.GetSection("Twilio"));
services.AddSingleton<FirebaseMessaging>(provider => 
    FirebaseMessaging.GetMessaging(FirebaseApp.DefaultInstance));
services.AddScoped<INotificationService, NotificationService>();
```

## Configuration Example (appsettings.json)
```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "From": "noreply@yourcompany.com"
  },
  "Twilio": {
    "AccountSid": "your-account-sid",
    "AuthToken": "your-auth-token",
    "FromNumber": "+1234567890"
  }
}
```

# Error Codes
| Code              | Description                          |
|-------------------|--------------------------------------|
| EmailError        | General email sending failure        |
| SmsError          | General SMS sending failure          |
| PushError         | General push notification failure    |

# Dependencies

## Required NuGet Packages
- `Microsoft.Extensions.Logging` - For structured logging
- `FirebaseAdmin` - For Firebase Cloud Messaging
- `Twilio` - For SMS messaging via Twilio API

## External Services
- **SMTP Server** - For email delivery (e.g., Gmail, SendGrid, AWS SES)
- **Twilio API** - For SMS messaging
- **Firebase Cloud Messaging** - For push notifications

# Models

## NotificationResult
```csharp
public class NotificationResult
{
    public bool IsSuccess { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    
    public static NotificationResult Success() => 
        new() { IsSuccess = true };
    
    public static NotificationResult Failed(string errorCode, string message) => 
        new() { IsSuccess = false, ErrorCode = errorCode, ErrorMessage = message };
}
```

## SmtpSettings
```csharp
public class SmtpSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string From { get; set; }
}
```

## TwilioSettings
```csharp
public class TwilioSettings
{
    public string AccountSid { get; set; }
    public string AuthToken { get; set; }
    public string FromNumber { get; set; }
}
```

# Notes
- **Thread Safety**: The service is designed to be thread-safe and can be used as a singleton or scoped service
- **Logging**: All operations are logged with structured logging using Microsoft.Extensions.Logging
- **Error Handling**: Comprehensive exception handling with detailed error messages
- **SSL/TLS**: Email sending uses SSL/TLS encryption by default
- **Async Operations**: All methods are asynchronous and should be awaited
- **Dependency Injection**: Designed to work with Microsoft's dependency injection container 