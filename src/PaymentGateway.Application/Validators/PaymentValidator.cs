using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.Models;
using PaymentGateway.Contracts.Requests;

namespace PaymentGateway.Application.Validators;

public class PaymentValidator : IPaymentValidator
{
    private static readonly HashSet<string> ValidCurrencyISOCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        "USD", "EUR", "GBP"
    };

    public ValidationErrors Validate(SubmitPaymentRequest request)
    {
        var errors = new ValidationErrors();
        
        if (!ValidCurrencyISOCodes.Contains(request.Currency))
        {
            errors.AddFieldError(nameof(request.Currency), "Invalid currency code.");
        }

        var now = DateTime.UtcNow;
        if (new DateTime(request.ExpiryYear, request.ExpiryMonth, 1) < new DateTime(now.Year, now.Month, 1))
        {
            if(request.ExpiryYear < now.Year)
                errors.AddFieldError(nameof(request.ExpiryYear), "Card expiry date must be in the future.");
            else            
                errors.AddFieldError(nameof(request.ExpiryMonth), "Card expiry date must be in the future.");
        }

        return errors;
    }
}
