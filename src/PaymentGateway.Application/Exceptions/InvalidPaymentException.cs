using PaymentGateway.Application.Models;

namespace PaymentGateway.Application.Exceptions;

public class InvalidPaymentException(ValidationErrors errors) : Exception()
{
    public ValidationErrors Errors { get; set; } = errors;
}
