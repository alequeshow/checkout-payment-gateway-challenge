using PaymentGateway.Application.Models;
using PaymentGateway.Contracts.Requests;

namespace PaymentGateway.Application.Interfaces;

public interface IPaymentProcessor
{
    Task<PaymentProcessResult> ProcessPaymentAsync(SubmitPaymentRequest request);
}
