using FluentAssertions;
using PaymentGateway.Application.Validators;
using PaymentGateway.Contracts.Requests;

namespace PaymentGateway.Application.Tests.Validators;

public class PaymentValidatorTests
{
    private readonly PaymentValidator _paymentValidator = new();

    [Fact]
    public void Validate_WithValidInput_ShouldReturnEmptyValidationResult()
    {
        // Arrange
        var request = new SubmitPaymentRequest
        {
            Amount = 100,
            Currency = "USD",
            CardNumber = "4111111111111111",
            ExpiryMonth = 1,
            ExpiryYear = 2030,
            Cvv = "123"
        };

        // Act
        var result = _paymentValidator.Validate(request);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithInvalidCurrency_ShouldReturnErrorForInvalidCurrency()
    {
        // Arrange
        var request = new SubmitPaymentRequest
        {
            Amount = 100,
            Currency = "INVALID",
            CardNumber = "4111111111111111",
            ExpiryMonth = 1,
            ExpiryYear = 2030,
            Cvv = "123"
        };

        // Act
        var result = _paymentValidator.Validate(request);

        // Assert
        result.Should().NotBeNull();        
        result.ContainsKey(request.Currency);
    }

    [Theory]
    [InlineData(1, 2020, "ExpiryYear")]
    [InlineData(1, 2026, "ExpiryMonth")]
    public void Validate_WithInvalidExpiryDate_ShouldReturnErrorForInvalidExpiryDate(int month, int year, string expectedErrorField)
    {
        // Arrange
        var request = new SubmitPaymentRequest
        {
            Amount = 100,
            Currency = "USD",
            CardNumber = "4111111111111111",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            Cvv = "123"
        };

        // Act
        var result = _paymentValidator.Validate(request);

        // Assert
        result.Should().NotBeNull();
        result.ContainsKey(expectedErrorField);
    }
}
