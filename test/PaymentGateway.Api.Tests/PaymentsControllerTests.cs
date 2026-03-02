using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Application.Exceptions;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Contracts.Requests;
using PaymentGateway.Contracts.Responses;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTests
{
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly PaymentsController _controller;

    public PaymentsControllerTests()
    {
        _paymentServiceMock = new Mock<IPaymentService>();
        _controller = new PaymentsController(_paymentServiceMock.Object);
    }

    [Fact]
    public async Task SubmitPaymentAsync_WithSuccessResult_ShouldReturnOkWithPaymentResponse()
    {
        // Arrange
        var expectedResponse = new SuccessPaymentResponse
        {
            Id = Guid.NewGuid(),
            Amount = 100,
            Currency = "USD",
            Status = "Authorized"
        };
        
        _paymentServiceMock
            .Setup(service => service.SubmitPaymentAsync(It.IsAny<SubmitPaymentRequest>()))
            .ReturnsAsync(expectedResponse);
        
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
        var result = await _controller.SubmitPaymentAsync(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualResponse = Assert.IsType<SuccessPaymentResponse>(okResult.Value);

        actualResponse.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task SubmitPaymentAsync_WithInvalidPayload_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new SubmitPaymentRequest
        {
            CardNumber = "1313131313131",
            ExpiryMonth = 13,
            ExpiryYear = 2025,
            Currency = "a",
            Amount = 0,
            Cvv = "12"
        };

        _paymentServiceMock
            .Setup(service => service.SubmitPaymentAsync(It.IsAny<SubmitPaymentRequest>()))
            .ThrowsAsync(new InvalidPaymentException([]));

        // Act
        var result = await _controller.SubmitPaymentAsync(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
    }

    [Fact]
    public async Task SubmitPaymentAsync_WithInternalError_ShouldReturnServerError()
    {
        // Arrange
        var request = new SubmitPaymentRequest
        {
            CardNumber = "1313131313131",
            ExpiryMonth = 13,
            ExpiryYear = 2025,
            Currency = "a",
            Amount = 0,
            Cvv = "12"
        };

        _paymentServiceMock
            .Setup(service => service.SubmitPaymentAsync(It.IsAny<SubmitPaymentRequest>()))
            .ThrowsAsync(new PaymentProviderException());

        // Act
        var result = await _controller.SubmitPaymentAsync(request);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task GetPaymentAsync_WithSuccessResult_ShouldReturnOkWithPaymentResponse()
    {
        // Arrange
        var expectedResponse = new SuccessPaymentResponse
        {
            Id = Guid.NewGuid(),
            Amount = 100,
            Currency = "USD",
            Status = "Authorized"
        };

        _paymentServiceMock
            .Setup(service => service.GetPaymentByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetPaymentAsync(Guid.NewGuid());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualResponse = Assert.IsType<SuccessPaymentResponse>(okResult.Value);
        actualResponse.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task GetPaymentAsync_WithNoExistingPayment_ShouldReturnNotFound()
    {
        // Arrange
        var requestId = Guid.NewGuid();

        _paymentServiceMock
            .Setup(service => service.GetPaymentByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new PaymentNotFoundException(requestId));

        // Act
        var result = await _controller.GetPaymentAsync(requestId);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        objectResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        var validationProblemDetails = Assert.IsType<ValidationProblemDetails>(objectResult.Value);
        validationProblemDetails.Detail.Should().Be($"Payment with ID {requestId} was not found.");
    }
}