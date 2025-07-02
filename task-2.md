---
title: "NotificationService API"
version: "1.2.0"
date: "2025-07-02"
---

# Overview
A unified notification service that supports sending **Email**, **SMS**, and **Push** messages.  
Each method returns a `NotificationResult` indicating success or failure with an error code and message.

# API Reference
| Method                       | Description                                | Return Type         |
|------------------------------|--------------------------------------------|---------------------|
| `SendEmailAsync(to, subject, body)` | Sends email via SMTP.                | `Task<NotificationResult>` |
| `SendSmsAsync(phoneNumber, message)` | Sends SMS via Twilio.               | `Task<NotificationResult>` |
| `SendPushAsync(deviceToken, title, body)` | Sends push via Firebase Cloud Messaging. | `Task<NotificationResult>` |

# Examples
**C# usage:**
```csharp
var result = await notificationService.SendEmailAsync(
    "user@example.com",
    "Welcome!",
    "<h1>Hello!</h1>"
// Expected: result.IsSuccess == true
);

# Sending SMS via a CLI tool example
curl -X POST https://api.myapp.com/notify/sms \
  -H "Content-Type: application/json" \
  -d '{"phoneNumber":"+1234567890","message":"Your code is 1234"}'
# Expected JSON: {"success":true}

# Error codes
EmailError	Failed to send email
SmsError	Failed to send SMS
PushError	Failed to send push notification

// NotificationResult model
{
  "bool IsSuccess", 
  "string ErrorCode", 
  "string ErrorMessage"
}

# YAML‑front‑matter: metadata  
# Sections in order: Overview, API Reference, Examples, Error Codes  
# Tables: parameters, return types, error codes  
# Code blocks: ```csharp``` or ```bash```  
