using PaymentGateway.Contracts.Requests;
using PaymentGateway.Contracts.Responses;

namespace PaymentGateway.Application.Interfaces;

public interface IPaymentService
{
    /// <summary>
    /// Submits a payment request for processing and storing and returns the payment response.
    /// </summary>
    Task<PaymentResponse> SubmitPaymentAsync(SubmitPaymentRequest request);

    /// <summary>
    /// Retrieves a payment by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the payment.</param>    
    Task<PaymentResponse> GetPaymentByIdAsync(Guid id);
}
