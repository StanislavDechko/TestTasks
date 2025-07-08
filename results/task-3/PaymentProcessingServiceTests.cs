using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Task3.Models;
using Task3.Services;
using Xunit;

namespace Task3.Tests.Services
{
    public class PaymentProcessingServiceTests
    {
        private readonly Mock<ILogger<PaymentProcessingService>> _loggerMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly PaymentProcessingService _service;

        public PaymentProcessingServiceTests()
        {
            _loggerMock = new Mock<ILogger<PaymentProcessingService>>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _service = new PaymentProcessingService(_loggerMock.Object, _httpClient);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ReturnsFailed_WhenAccountNotFound()
        {
            // Arrange
            int accountId = 999; // Non-existent account
            decimal amount = 100m;

            // Act
            var result = await _service.ProcessPaymentAsync(accountId, amount);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Account not found", result.ErrorMessage);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ReturnsFailed_WhenAmountIsNegative()
        {
            // Arrange
            int accountId = 1;
            decimal amount = -50m;

            // Act
            var result = await _service.ProcessPaymentAsync(accountId, amount);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Amount must be non-negative", result.ErrorMessage);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ReturnsFailed_WhenInsufficientFunds()
        {
            // Arrange
            int accountId = 2; // Account with 0 balance
            decimal amount = 100m;

            // Act
            var result = await _service.ProcessPaymentAsync(accountId, amount);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Insufficient funds", result.ErrorMessage);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ReturnsSuccess_WhenValidPayment()
        {
            // Arrange
            int accountId = 1; // Account with 5000 balance
            decimal amount = 100m;
            var expectedBalance = 4900m;

            SetupSuccessfulGatewayResponse();

            // Act
            var result = await _service.ProcessPaymentAsync(accountId, amount);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedBalance, result.NewBalance);
        }

        [Fact]
        public async Task ProcessPaymentAsync_HandlesEmptyJsonResponse_WithRetry()
        {
            // Arrange
            int accountId = 1;
            decimal amount = 100m;

            // Setup handler to return empty response on first attempt, success on second
            var callCount = 0;
            _httpMessageHandlerMock
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("", Encoding.UTF8, "application/json")
                })
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new GatewayResponse { Success = true, Message = "OK" }), Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _service.ProcessPaymentAsync(accountId, amount);

            // Assert
            Assert.True(result.IsSuccess);
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(2),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task ProcessPaymentAsync_HandlesMalformedJsonResponse_WithRetry()
        {
            // Arrange
            int accountId = 1;
            decimal amount = 100m;

            // Setup handler to return malformed JSON on first attempt, success on second
            var callCount = 0;
            _httpMessageHandlerMock
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{ invalid json }", Encoding.UTF8, "application/json")
                })
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new GatewayResponse { Success = true, Message = "OK" }), Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _service.ProcessPaymentAsync(accountId, amount);

            // Assert
            Assert.True(result.IsSuccess);
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(2),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task ProcessPaymentAsync_HandlesHttpTimeout_WithRetry()
        {
            // Arrange
            int accountId = 1;
            decimal amount = 100m;

            // Setup handler to simulate timeout on first attempt, success on second
            _httpMessageHandlerMock
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new TaskCanceledException("Request timeout"))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new GatewayResponse { Success = true, Message = "OK" }), Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _service.ProcessPaymentAsync(accountId, amount);

            // Assert
            Assert.True(result.IsSuccess);
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(2),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task ProcessPaymentAsync_HandlesHttpRequestException_WithRetry()
        {
            // Arrange
            int accountId = 1;
            decimal amount = 100m;

            // Setup handler to simulate network error on first attempt, success on second
            _httpMessageHandlerMock
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network error"))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new GatewayResponse { Success = true, Message = "OK" }), Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _service.ProcessPaymentAsync(accountId, amount);

            // Assert
            Assert.True(result.IsSuccess);
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(2),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task ProcessPaymentAsync_ReturnsFailed_WhenGatewayReturnsError()
        {
            // Arrange
            int accountId = 1;
            decimal amount = 100m;

            SetupGatewayErrorResponse("Payment declined");

            // Act
            var result = await _service.ProcessPaymentAsync(accountId, amount);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Gateway failure: Payment declined", result.ErrorMessage);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ReturnsFailed_WhenGatewayReturnsNonSuccessStatusCode()
        {
            // Arrange
            int accountId = 1;
            decimal amount = 100m;

            SetupHttpErrorResponse(HttpStatusCode.InternalServerError);

            // Act
            var result = await _service.ProcessPaymentAsync(accountId, amount);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Gateway communication failed", result.ErrorMessage);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ActivatesCircuitBreaker_AfterMultipleFailures()
        {
            // Arrange
            int accountId = 1;
            decimal amount = 100m;

            // Setup handler to always fail
            SetupHttpErrorResponse(HttpStatusCode.InternalServerError);

            // Act - Make multiple requests to trigger circuit breaker
            var result1 = await _service.ProcessPaymentAsync(accountId, amount);
            var result2 = await _service.ProcessPaymentAsync(accountId, amount);
            var result3 = await _service.ProcessPaymentAsync(accountId, amount);
            var result4 = await _service.ProcessPaymentAsync(accountId, amount);

            // Assert
            Assert.False(result1.IsSuccess);
            Assert.False(result2.IsSuccess);
            Assert.False(result3.IsSuccess);
            Assert.False(result4.IsSuccess);
            Assert.Equal("Gateway communication failed", result1.ErrorMessage);
            Assert.Equal("Gateway communication failed", result2.ErrorMessage);
            Assert.Equal("Gateway communication failed", result3.ErrorMessage);
            Assert.Equal("Service temporarily unavailable", result4.ErrorMessage); // Circuit breaker activated
        }

        private void SetupSuccessfulGatewayResponse()
        {
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(new GatewayResponse { Success = true, Message = "OK" }),
                        Encoding.UTF8,
                        "application/json"
                    )
                });
        }

        private void SetupGatewayErrorResponse(string errorMessage)
        {
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(new GatewayResponse { Success = false, Message = errorMessage }),
                        Encoding.UTF8,
                        "application/json"
                    )
                });
        }

        private void SetupHttpErrorResponse(HttpStatusCode statusCode)
        {
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(statusCode));
        }
    }
}