using PaymentGateway.Application.Models;
using PaymentGateway.Contracts.Requests;

namespace PaymentGateway.Application.Interfaces;

public interface IPaymentValidator
{
    /// <summary>
    /// Validates the payment request and returns any validation errors. 
    /// If there are no errors, the returned ValidationErrors will be empty.
    /// </summary>
    ValidationErrors Validate(SubmitPaymentRequest request);
}
