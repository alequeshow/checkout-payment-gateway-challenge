using System.Net.Http.Json;
using System.Text.Json.Serialization;
using PaymentGateway.Application.Exceptions;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.Models;
using PaymentGateway.Contracts.Requests;

namespace PaymentGateway.Application.Services;

public class AcquiringBankProcessor(IHttpClientFactory httpClientFactory) : IPaymentProcessor
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(AcquiringBankProcessor));

    /// <inheritdoc/>
    /// <summary>Connects to the acquiring bank's API to process the payment request.</summary>
    /// <exception cref="PaymentProviderException"></exception>
    public async Task<PaymentProcessResult> ProcessPaymentAsync(SubmitPaymentRequest request)
    {

        try
        {
            var bankRequest = new BankPaymentRequest(
                CardNumber: request.CardNumber,
                ExpiryDate: $"{request.ExpiryMonth:D2}/{request.ExpiryYear}",
                Currency: request.Currency,
                Amount: request.Amount,
                Cvv: request.Cvv
            );

            var response = await _httpClient.PostAsJsonAsync("/payments", bankRequest);

            if (!response.IsSuccessStatusCode)
                throw new PaymentProviderException();

            var bankResponse = await response.Content.ReadFromJsonAsync<BankPaymentResponse>()
                ?? throw new PaymentProviderException();

            return new PaymentProcessResult
            {
                Status = bankResponse!.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined,
                AuthorizationCode = bankResponse.AuthorizationCode
            };
        }
        catch (PaymentProviderException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new PaymentProviderException();
        }
    }

    private record BankPaymentRequest(
        [property: JsonPropertyName("card_number")] string CardNumber,
        [property: JsonPropertyName("expiry_date")] string ExpiryDate,
        [property: JsonPropertyName("currency")] string Currency,
        [property: JsonPropertyName("amount")] int Amount,
        [property: JsonPropertyName("cvv")] string Cvv
    );

    private record BankPaymentResponse(
        [property: JsonPropertyName("authorized")] bool Authorized,
        [property: JsonPropertyName("authorization_code")] string? AuthorizationCode
    );
}