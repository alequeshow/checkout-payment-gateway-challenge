using System.ComponentModel.DataAnnotations;
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

    /// <inheritdoc/>
    /// <summary>
    /// Validates the payment request by checking:
    /// - Input formats
    /// - Currency codes
    /// - Expiry date
    /// </summary>    
    public ValidationErrors Validate(SubmitPaymentRequest request)
    {
        var errors = new ValidationErrors();

        ValidateInputFormats(errors, request);

        ValidateCurrencyCodes(errors, request);

        ValidateExpiryDate(errors, request);

        return errors;
    }

    private static void ValidateInputFormats(ValidationErrors errors, SubmitPaymentRequest request)
    {
        var annotationResults = new List<ValidationResult>();

        Validator.TryValidateObject(request, new ValidationContext(request), annotationResults, validateAllProperties: true);

        foreach (var result in annotationResults)
            foreach (var member in result.MemberNames)
                errors.AddFieldError(member, result.ErrorMessage ?? "Invalid value.");
    }

    private static void ValidateCurrencyCodes(ValidationErrors errors, SubmitPaymentRequest request)
    {
        if (errors.ContainsKey(nameof(request.Currency)))
            return;

        if (!ValidCurrencyISOCodes.Contains(request.Currency))
            errors.AddFieldError(nameof(request.Currency), $"Unsupported currency code. Supported codes are: {string.Join(", ", ValidCurrencyISOCodes)}.");
    }

    private static void ValidateExpiryDate(ValidationErrors errors, SubmitPaymentRequest request)
    {
        if (errors.ContainsKey(nameof(request.ExpiryMonth)) || errors.ContainsKey(nameof(request.ExpiryYear)))
            return;

        var now = DateTime.UtcNow;
        if (new DateTime(request.ExpiryYear, request.ExpiryMonth, 1) < new DateTime(now.Year, now.Month, 1))
        {
            errors.AddFieldError(nameof(request.ExpiryYear), "Card expiry date must be in the future.");
            errors.AddFieldError(nameof(request.ExpiryMonth), "Card expiry date must be in the future.");
        }
    }
}
