namespace PaymentGateway.Application.Exceptions;

public class PaymentNotFoundException(Guid paymentId) : Exception($"Payment with ID {paymentId} was not found.")
{
}
