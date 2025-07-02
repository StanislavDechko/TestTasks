Prompt:

You are working in a C# project targeting .NET 7. The `PaymentProcessingService` class is responsible for processing payments by communicating with an external payment gateway. Currently, the service intermittently throws a `NullReferenceException` when deserializing the gateway's JSON response, which can be empty or malformed. Additionally, the service lacks resilience logic (such as retries or a circuit breaker), resulting in unhandled errors at runtime.

**(Refference to LOGS)**

Your task is to:
- Fix the `NullReferenceException` by implementing robust error handling for empty or malformed JSON responses from the payment gateway in the `ProcessPaymentAsync` method.
- Add resilience logic to the service, such as retries with exponential backoff and/or a circuit breaker pattern, to handle transient gateway failures gracefully.
- Ensure all error cases are logged appropriately and that the method returns meaningful error messages via the `PaymentResult` model.
- Follow the project's naming, structure, and code style conventions.
- Write clear, maintainable, and well-commented code.
- Highlight all class, method, and code elements using markdown backticks in your response.

**(Reference the existing code for context)**
Your solution should be production-ready and easy to understand for other developers.

Also describe all changes that you've did.
