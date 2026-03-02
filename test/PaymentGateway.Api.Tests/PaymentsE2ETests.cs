using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PaymentGateway.Api.Tests.Fixtures;
using PaymentGateway.Application.Data;
using PaymentGateway.Application.Models;
using PaymentGateway.Contracts.Requests;
using PaymentGateway.Contracts.Responses;

namespace PaymentGateway.Api.Tests;

public class PaymentsE2ETests(PaymentGatewayWebApplicationFactory factory) : IClassFixture<PaymentGatewayWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();

    public Task InitializeAsync()
    {
        factory.PaymentProcessorMock.Reset();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        using var scope = factory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Payments.RemoveRange(db.Payments);
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetPaymentAsync_WithExistingPayment_ShouldReturnOk()
    {
        // Arrange
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            CardNumberLastFour = "1234",
            ExpiryMonth = 12,
            ExpiryYear = DateTime.UtcNow.Year + 1,
            Currency = "GBP",
            Amount = 100,
            Status = PaymentStatus.Authorized
        };

        using (var scope = factory.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Payments.Add(payment);
            await db.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync($"/api/Payments/{payment.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SuccessPaymentResponse>();
        result.Should().NotBeNull();
        result.Id.Should().Be(payment.Id);
    }

    [Fact]
    public async Task GetPaymentAsync_WithNonExistingPayment_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/Payments/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(PaymentStatus.Authorized, "AUTH-001")]
    [InlineData(PaymentStatus.Declined, null)]
    public async Task SubmitPaymentAsync_AuthorizedByBank_ShouldPersistAndReturnAuthorized(PaymentStatus providerStatus, string? authorizationCode)
    {
        // Arrange
        factory.PaymentProcessorMock
            .Setup(p => p.ProcessPaymentAsync(It.IsAny<SubmitPaymentRequest>()))
            .ReturnsAsync(new PaymentProcessResult
            {
                Status = providerStatus,
                AuthorizationCode = authorizationCode
            });

        var request = new SubmitPaymentRequest
        {
            CardNumber = "4111111111111111",
            ExpiryMonth = 12,
            ExpiryYear = DateTime.UtcNow.Year + 1,
            Currency = "USD",
            Amount = 1000,
            Cvv = "123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Payments", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SuccessPaymentResponse>();
        result.Should().NotBeNull();
        result.Status.Should().Be(providerStatus.ToString());

        using var scope = factory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        (await db.Payments.FindAsync(result.Id)).Should().NotBeNull();
    }
}
