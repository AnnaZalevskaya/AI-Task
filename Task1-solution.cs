using FluentAssertions;
using Task1.Services;
using Xunit;

namespace Task1.Tests.Services
{
    public class OrderServiceTests
    {
        [Fact]
        public void CalculateOrder_ThrowsArgumentException_WhenTotalAmountIsNegative()
        {
            // Arrange
            var service = new OrderService();
            int totalAmount = -100;
            bool isMember = true;
            int itemsCount = 1;

            // Act
            Action act = () => service.CalculateOrder(totalAmount, isMember, itemsCount);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Amount must be non-negative*")
                .And.ParamName.Should().Be("totalAmount");
        }

        [Theory]
        [InlineData(1500, true, 1, 15, 1275, 30)]   // Member, amount > 1000
        [InlineData(900, true, 1, 10, 810, 18)]     // Member, amount <= 1000
        [InlineData(1000, true, 1, 10, 900, 20)]    // Member, amount = 1000 (boundary)
        [InlineData(0, true, 1, 10, 0, 0)]          // Member, amount = 0
        [InlineData(6000, false, 1, 5, 5700, 60)]   // Non-member, amount > 5000
        [InlineData(3000, false, 1, 0, 3000, 30)]   // Non-member, amount <= 5000
        [InlineData(5000, false, 1, 0, 5000, 50)]   // Non-member, amount = 5000 (boundary)
        [InlineData(0, false, 1, 0, 0, 0)]          // Non-member, amount = 0
        public void CalculateOrder_ReturnsCorrectDiscountResult(int totalAmount, bool isMember, int itemsCount, 
            int expectedDiscountPercent, int expectedFinalAmount, int expectedBonusPoints)
        {
            // Arrange
            var service = new OrderService();

            // Act
            var result = service.CalculateOrder(totalAmount, isMember, itemsCount);

            // Assert
            result.Should().NotBeNull();
            result.DiscountPercent.Should().Be(expectedDiscountPercent);
            result.FinalAmount.Should().Be(expectedFinalAmount);
            result.BonusPoints.Should().Be(expectedBonusPoints);
        }
    }
}
