using Microsoft.EntityFrameworkCore;
using PaymentGateway.Application.Data;
using PaymentGateway.Application.Exceptions;
using PaymentGateway.Application.Extensions;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Contracts.Requests;
using PaymentGateway.Contracts.Responses;

namespace PaymentGateway.Application.Services;

public class PaymentService(
    IPaymentProcessor paymentProcessor,
    IPaymentValidator paymentValidator,
    ApplicationDbContext dbContext) : IPaymentService
{
    public async Task<PaymentResponse> GetPaymentByIdAsync(Guid id)
    {
        var result = await dbContext.Payments
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync() ?? throw new PaymentNotFoundException(id);

        return result.ToResponse();
    }

    public async Task<PaymentResponse> SubmitPaymentAsync(SubmitPaymentRequest request)
    {
        var validationErrors = paymentValidator.Validate(request);

        if (validationErrors.Count > 0)
            throw new InvalidPaymentException(validationErrors);

        var processResult = await paymentProcessor.ProcessPaymentAsync(request);

        var payment = request.ToModel(processResult.Status, processResult.AuthorizationCode);

        await dbContext.Payments.AddAsync(payment);
        await dbContext.SaveChangesAsync();

        return payment.ToResponse();
    }
}
