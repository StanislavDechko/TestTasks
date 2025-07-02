using System;
using FluentAssertions;
using Task1.Services;
using Xunit;

namespace Task1.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly OrderService _service = new();

        [Fact]
        public void CalculateOrder_ThrowsException_WhenTotalAmountIsNegative()
        {
            // Arrange
            int totalAmount = -100;
            bool isMember = true;
            int itemsCount = 3;

            // Act
            Action act = () => _service.CalculateOrder(totalAmount, isMember, itemsCount);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("Amount must be non-negative*")
               .And.ParamName.Should().Be("totalAmount");
        }

        [Theory]
        [InlineData(1100, true, 2, 15, 935, 22)]  // Member > 1000
        [InlineData(1000, true, 2, 10, 900, 20)]  // Member = 1000
        [InlineData(500, true, 2, 10, 450, 10)]   // Member < 1000
        [InlineData(6000, false, 2, 5, 5700, 60)] // Non-member > 5000
        [InlineData(5000, false, 2, 0, 5000, 50)] // Non-member = 5000
        [InlineData(300, false, 2, 0, 300, 3)]    // Non-member < 5000
        public void CalculateOrder_ReturnsCorrectResult(
            int totalAmount, bool isMember, int itemsCount,
            int expectedDiscount, int expectedFinalAmount, int expectedBonusPoints)
        {
            // Act
            var result = _service.CalculateOrder(totalAmount, isMember, itemsCount);

            // Assert
            result.DiscountPercent.Should().Be(expectedDiscount);
            result.FinalAmount.Should().Be(expectedFinalAmount);
            result.BonusPoints.Should().Be(expectedBonusPoints);
        }

        [Theory]
        [InlineData(0, true, 1, 10, 0, 0)]       // Member, amount = 0
        [InlineData(0, false, 1, 0, 0, 0)]       // Non-member, amount = 0
        [InlineData(100, true, 1, 10, 90, 2)]    // Bonus calc: member
        [InlineData(100, false, 1, 0, 100, 1)]   // Bonus calc: non-member
        public void CalculateOrder_HandlesEdgeCases_Correctly(
            int totalAmount, bool isMember, int itemsCount,
            int expectedDiscount, int expectedFinalAmount, int expectedBonusPoints)
        {
            // Act
            var result = _service.CalculateOrder(totalAmount, isMember, itemsCount);

            // Assert
            result.DiscountPercent.Should().Be(expectedDiscount);
            result.FinalAmount.Should().Be(expectedFinalAmount);
            result.BonusPoints.Should().Be(expectedBonusPoints);
        }
    }
}
