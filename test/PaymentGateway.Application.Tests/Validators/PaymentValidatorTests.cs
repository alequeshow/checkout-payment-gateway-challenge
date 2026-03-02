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
    public void Validate_WithInvalidFormats_ShouldReturnErrorForProperties()
    {
        // Arrange
        var request = new SubmitPaymentRequest
        {
            Amount = 0,
            Currency = "INVALID",
            CardNumber = "ABD123456",
            ExpiryMonth = 0,
            ExpiryYear = 0,
            Cvv = "A"
        };

        // Act
        var result = _paymentValidator.Validate(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().ContainKeys(
            nameof(request.Amount),
            nameof(request.Currency),
            nameof(request.CardNumber),
            nameof(request.ExpiryMonth),
            nameof(request.ExpiryYear),
            nameof(request.Cvv));
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
        result.Should().ContainKey(nameof(request.Currency));
    }

    [Fact]
    public void Validate_WithInvalidExpiryDate_ShouldReturnErrorForInvalidExpiryDate()
    {
        // Arrange
        var targetDate = DateTime.UtcNow.AddMonths(-1);

        var request = new SubmitPaymentRequest
        {
            Amount = 100,
            Currency = "USD",
            CardNumber = "4111111111111111",
            ExpiryMonth = targetDate.Month,
            ExpiryYear = targetDate.Year,
            Cvv = "123"
        };

        // Act
        var result = _paymentValidator.Validate(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().ContainKeys(nameof(request.ExpiryMonth), nameof(request.ExpiryYear));
    }
}
