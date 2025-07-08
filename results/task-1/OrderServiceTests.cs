using Xunit;
using Task1.Services;

namespace Task1.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderService = new OrderService();
        }

        [Fact]
        public void CalculateOrder_ThrowsArgumentException_WhenTotalAmountIsNegative()
        {
            // Arrange
            int totalAmount = -100;
            bool isMember = true;
            int itemsCount = 5;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _orderService.CalculateOrder(totalAmount, isMember, itemsCount));

            Assert.Equal("Amount must be non-negative", exception.Message);
            Assert.Equal("totalAmount", exception.ParamName);
        }

        [Theory]
        [InlineData(0, true, 1, 0, 0, 0)] // Edge case: zero amount for member
        [InlineData(0, false, 1, 0, 0, 0)] // Edge case: zero amount for non-member
        [InlineData(500, true, 3, 10, 450, 10)] // Member with amount ≤ 1000
        [InlineData(1000, true, 2, 10, 900, 20)] // Member with amount = 1000 (edge case)
        [InlineData(1500, true, 4, 15, 1275, 30)] // Member with amount > 1000
        [InlineData(100, false, 1, 0, 100, 1)] // Non-member with amount ≤ 5000
        [InlineData(5000, false, 2, 0, 5000, 50)] // Non-member with amount = 5000 (edge case)
        [InlineData(6000, false, 3, 5, 5700, 60)] // Non-member with amount > 5000
        public void CalculateOrder_ReturnsCorrectDiscountAndBonusPoints_ForValidInputs(
            int totalAmount,
            bool isMember,
            int itemsCount,
            int expectedDiscount,
            int expectedFinalAmount,
            int expectedBonusPoints)
        {
            // Arrange
            // (parameters are already arranged)

            // Act
            var result = _orderService.CalculateOrder(totalAmount, isMember, itemsCount);

            // Assert
            Assert.Equal(expectedDiscount, result.DiscountPercent);
            Assert.Equal(expectedFinalAmount, result.FinalAmount);
            Assert.Equal(expectedBonusPoints, result.BonusPoints);
        }

        [Theory]
        [InlineData(100, true, 1, 10)] // Member gets 10% discount
        [InlineData(100, false, 1, 0)] // Non-member gets 0% discount
        [InlineData(1001, true, 1, 15)] // Member with amount > 1000 gets 15% discount
        [InlineData(5001, false, 1, 5)] // Non-member with amount > 5000 gets 5% discount
        public void CalculateOrder_AppliesCorrectDiscountPercentage_ForDifferentScenarios(
            int totalAmount,
            bool isMember,
            int itemsCount,
            int expectedDiscountPercent)
        {
            // Arrange
            // (parameters are already arranged)

            // Act
            var result = _orderService.CalculateOrder(totalAmount, isMember, itemsCount);

            // Assert
            Assert.Equal(expectedDiscountPercent, result.DiscountPercent);
        }

        [Theory]
        [InlineData(100, true, 2)] // Member: (100/100) * 2 = 2
        [InlineData(100, false, 1)] // Non-member: (100/100) * 1 = 1
        [InlineData(250, true, 4)] // Member: (250/100) * 2 = 4
        [InlineData(250, false, 2)] // Non-member: (250/100) * 1 = 2
        [InlineData(99, true, 0)] // Member: (99/100) * 2 = 0 (integer division)
        [InlineData(99, false, 0)] // Non-member: (99/100) * 1 = 0 (integer division)
        public void CalculateOrder_CalculatesCorrectBonusPoints_ForMemberAndNonMember(
            int totalAmount,
            bool isMember,
            int expectedBonusPoints)
        {
            // Arrange
            int itemsCount = 1;

            // Act
            var result = _orderService.CalculateOrder(totalAmount, isMember, itemsCount);

            // Assert
            Assert.Equal(expectedBonusPoints, result.BonusPoints);
        }

        [Theory]
        [InlineData(1000, true, 900)] // Member with 10% discount: 1000 * 0.9 = 900
        [InlineData(1001, true, 850)] // Member with 15% discount: 1001 * 0.85 = 850
        [InlineData(5000, false, 5000)] // Non-member with 0% discount: 5000 * 1 = 5000
        [InlineData(5001, false, 4750)] // Non-member with 5% discount: 5001 * 0.95 = 4750
        public void CalculateOrder_CalculatesCorrectFinalAmount_AfterDiscount(
            int totalAmount,
            bool isMember,
            int expectedFinalAmount)
        {
            // Arrange
            int itemsCount = 1;

            // Act
            var result = _orderService.CalculateOrder(totalAmount, isMember, itemsCount);

            // Assert
            Assert.Equal(expectedFinalAmount, result.FinalAmount);
        }

        [Theory]
        [InlineData(999, true, 10)] // Member just under 1000 threshold
        [InlineData(1000, true, 10)] // Member exactly at 1000 threshold
        [InlineData(1001, true, 15)] // Member just over 1000 threshold
        [InlineData(4999, false, 0)] // Non-member just under 5000 threshold
        [InlineData(5000, false, 0)] // Non-member exactly at 5000 threshold
        [InlineData(5001, false, 5)] // Non-member just over 5000 threshold
        public void CalculateOrder_HandlesThresholdEdgeCases_Correctly(
            int totalAmount,
            bool isMember,
            int expectedDiscount)
        {
            // Arrange
            int itemsCount = 1;

            // Act
            var result = _orderService.CalculateOrder(totalAmount, isMember, itemsCount);

            // Assert
            Assert.Equal(expectedDiscount, result.DiscountPercent);
        }

        [Fact]
        public void CalculateOrder_ReturnsValidDiscountResult_ForComplexScenario()
        {
            // Arrange
            int totalAmount = 2500;
            bool isMember = true;
            int itemsCount = 5;
            int expectedDiscount = 15; // Member with amount > 1000
            int expectedFinalAmount = 2125; // 2500 * 0.85
            int expectedBonusPoints = 50; // (2500/100) * 2

            // Act
            var result = _orderService.CalculateOrder(totalAmount, isMember, itemsCount);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDiscount, result.DiscountPercent);
            Assert.Equal(expectedFinalAmount, result.FinalAmount);
            Assert.Equal(expectedBonusPoints, result.BonusPoints);
        }
    }
}