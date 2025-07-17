Prompt:

You are an experienced C# developer writing unit tests using xUnit and FluentAssertions.

The task is to write a unit test class for the method OrderService.CalculateOrder(int totalAmount, bool isMember, int itemsCount) from the namespace Task1.Services.

The test must:
1. Use xUnit [Fact] and [Theory] attributes as appropriate and FluentAssertions for all assertions.
2. Include all test cases where isMember true/false, including relevant limits and negative totalAmount.
3. Validate the returned DiscountResult fields: DiscountPercent, FinalAmount, and BonusPoints.
4. Use descriptive method names and // Arrange // Act // Assert structure.
5. Use hardcoded input values and expected results (no mocking or randomness).
6. Match the naming and formatting style of CalculateShipping_ReturnsFree_WhenWeightUnderThreshold for consistency.
