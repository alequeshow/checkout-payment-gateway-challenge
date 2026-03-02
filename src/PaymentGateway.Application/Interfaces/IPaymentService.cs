using PaymentGateway.Application.Models;
using PaymentGateway.Contracts.Requests;
using PaymentGateway.Contracts.Responses;

namespace PaymentGateway.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponse> SubmitPaymentAsync(SubmitPaymentRequest request);

    Task<PaymentResponse> GetPaymentByIdAsync(Guid id);
}
