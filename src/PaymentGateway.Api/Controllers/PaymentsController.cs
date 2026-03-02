using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.Exceptions;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Contracts.Requests;
using PaymentGateway.Contracts.Responses;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentService paymentService) : Controller
{
    [HttpPost]
    [ProducesResponseType(typeof(SuccessPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaymentResponse?>> SubmitPaymentAsync(SubmitPaymentRequest request)
    {
        try
        {
            var result = await paymentService.SubmitPaymentAsync(request);

            return new OkObjectResult(result);
        }
        catch(Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SuccessPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaymentResponse?>> GetPaymentAsync(Guid id)
    {
        try
        {
            var result = await paymentService.GetPaymentByIdAsync(id);

            return new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    private ObjectResult HandleException(Exception ex) => ex switch
    {
        InvalidPaymentException invalidEx => new BadRequestObjectResult(
            new ValidationProblemDetails(invalidEx.Errors)),

        PaymentNotFoundException notFoundEx => StatusCode(
            StatusCodes.Status404NotFound,
            new ValidationProblemDetails() { Detail = notFoundEx.Message }),

        _ => StatusCode(
            StatusCodes.Status500InternalServerError,
            new ValidationProblemDetails() { Detail = ex.Message })
    };
}