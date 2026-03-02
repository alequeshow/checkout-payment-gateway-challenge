using PaymentGateway.Application.Models;
using PaymentGateway.Contracts.Requests;

namespace PaymentGateway.Application.Interfaces;

public interface IPaymentValidator
{
    ValidationErrors Validate(SubmitPaymentRequest request);
}
