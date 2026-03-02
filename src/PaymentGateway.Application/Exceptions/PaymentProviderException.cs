namespace PaymentGateway.Application.Exceptions;

public class PaymentProviderException() : Exception("The request could not be processed due to a payment provider error. Please try again later.")
{
}
