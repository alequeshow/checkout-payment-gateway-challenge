using PaymentGateway.Application.Models;
using PaymentGateway.Contracts.Requests;

namespace PaymentGateway.Application.Interfaces;

public interface IPaymentProcessor
{
    /// <summary>
    /// Processes a payment based on the provided request details and returns the result of the payment processing    
    /// </summary>    
    /// <returns>The result of the payment processing, including the status: Authorized or Declined and authorization code when successful.</returns>
    Task<PaymentProcessResult> ProcessPaymentAsync(SubmitPaymentRequest request);
}
