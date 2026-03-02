using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Contracts.Requests;

public record SubmitPaymentRequest
{
    [RegularExpression(@"^\d{14,19}$", ErrorMessage = "Card number must be between 14-19 numeric digits.")]
    public required string CardNumber { get; init; }

    [Range(1, 12, ErrorMessage = "Expiry month must be between 1 and 12.")]
    public required int ExpiryMonth { get; init; }

    [Range(1, 9999, ErrorMessage = "Expiry year must be a valid year.")]
    public required int ExpiryYear { get; init; }

    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be exactly 3 characters.")]
    public required string Currency { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Amount must be a positive value.")]
    public required int Amount { get; init; }

    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 numeric digits.")]
    public required string Cvv { get; init; }    
}