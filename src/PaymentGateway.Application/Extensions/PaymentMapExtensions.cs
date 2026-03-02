using PaymentGateway.Application.Models;
using PaymentGateway.Contracts.Requests;
using PaymentGateway.Contracts.Responses;

namespace PaymentGateway.Application.Extensions;

public static class PaymentMapExtensions
{
    public static SuccessPaymentResponse ToResponse(this Payment model)
    {
        return new SuccessPaymentResponse
        {
            Id = model.Id,
            Amount = model.Amount,
            Currency = model.Currency,
            Status = model.Status.ToString(),
            CardNumberLastFour = model.CardNumberLastFour,
            ExpiryMonth = model.ExpiryMonth,
            ExpiryYear = model.ExpiryYear
        };
    }

    public static Payment ToModel(this SubmitPaymentRequest request, PaymentStatus status, string? authorizationCode = null)
    {
        return new Payment
        {
            Amount = request.Amount,
            Currency = request.Currency.ToUpperInvariant(),
            CardNumberLastFour = request.CardNumber[^4..],
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Status = status,
            AuthorizationCode = authorizationCode
        };
    }
}
