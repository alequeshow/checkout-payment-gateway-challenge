namespace PaymentGateway.Application.Models;

public class PaymentProcessResult
{
    public PaymentStatus Status { get; set; }
    public string? AuthorizationCode { get; set; }
}
