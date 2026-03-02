using FluentAssertions;
using PaymentGateway.Application.Extensions;
using PaymentGateway.Application.Models;
using PaymentGateway.Contracts.Requests;

namespace PaymentGateway.Application.Tests.Extensions;

public class PaymentMapExtensionsTests
{
    [Fact]
    public void ToResponse_ShouldReturnMappedResponseContract()
    {
        // Arrange
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Authorized,
            Amount = 100,
            Currency = "USD",
            CardNumberLastFour = "4321",
            ExpiryMonth = 1,
            ExpiryYear = 2030,
            AuthorizationCode = "AUTH123"
        };

        // Act
        var result = payment.ToResponse();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(payment.Id);
        result.Status.Should().Be(payment.Status.ToString());
        result.Amount.Should().Be(payment.Amount);
        result.Currency.Should().Be(payment.Currency);
        result.CardNumberLastFour.Should().Be(payment.CardNumberLastFour);
        result.ExpiryMonth.Should().Be(payment.ExpiryMonth);
        result.ExpiryYear.Should().Be(payment.ExpiryYear);
    }

    [Fact]
    public void ToModel_ShouldReturnMappedPaymentModel()
    {
        // Arrange
        var request = new SubmitPaymentRequest
        {
            Amount = 100,
            Currency = "usd",
            CardNumber = "123456789654321",
            ExpiryMonth = 1,
            ExpiryYear = 2030,
            Cvv = "123"
        };

        // Act
        var result = request.ToModel(PaymentStatus.Authorized, "AUTH123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(Guid.Empty);
        result.Status.Should().Be(PaymentStatus.Authorized);
        result.Amount.Should().Be(request.Amount);
        result.Currency.Should().Be(request.Currency.ToUpperInvariant());
        result.CardNumberLastFour.Should().Be(request.CardNumber[^4..]);
        result.ExpiryMonth.Should().Be(request.ExpiryMonth);
        result.ExpiryYear.Should().Be(request.ExpiryYear);
    }
}
