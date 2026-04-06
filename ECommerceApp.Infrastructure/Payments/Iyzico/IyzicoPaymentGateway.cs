using ECommerceApp.Application.DTOs.Payments;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Infrastructure.Options;
using ECommerceApp.Infrastructure.Payments.Iyzico.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Payments.Iyzico
{
    public class IyzicoPaymentGateway : IPaymentGateway
    {
        private readonly HttpClient _httpClient;
        private readonly IyzicoOptions _options;
        private readonly ILogger<IyzicoPaymentGateway> _logger;

        public IyzicoPaymentGateway(HttpClient httpClient, IOptions<IyzicoOptions> options, ILogger<IyzicoPaymentGateway> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<PaymentResult> ProcessAsync(ProcessPaymentRequest request, CancellationToken cancellationToken = default)
        {
            var iyzicoRequest = MapRequest(request);

            const string uriPath = "/payment/auth";

            var jsonBody = JsonSerializer.Serialize(iyzicoRequest);
            var rnd = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var authorization = CreateAuthorizationHeader(uriPath, jsonBody, rnd);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, uriPath);
            requestMessage.Headers.Add("x-iyzi-rnd", rnd);
            requestMessage.Headers.Add("x-iyzi-client-version", "iyzipay-dotnet-1.0");
            requestMessage.Headers.Add("Authorization", authorization);
            requestMessage.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(requestMessage, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Iyzico request failed. StatusCode={StatusCode}, Response={ResponseBody}", response.StatusCode, responseBody);

                var failedPayload = JsonSerializer.Deserialize<CreatePaymentResponse>(responseBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return new PaymentResult
                {
                    IsSuccess = false,
                    ConversationId = failedPayload?.ConversationId,
                    PaymentId = failedPayload?.PaymentId,
                    ErrorCode = failedPayload?.ErrorCode,
                    ErrorMessage = failedPayload?.ErrorMessage ?? $"Payment request failed with HTTP {(int)response.StatusCode}."
                };
            }


            var payload = JsonSerializer.Deserialize<CreatePaymentResponse>(
                responseBody,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (payload is null)
            {
                return new PaymentResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Payment provider response was empty."
                };
            }

            return new PaymentResult
            {
                IsSuccess = string.Equals(payload.Status, "success", StringComparison.OrdinalIgnoreCase),
                PaymentId = payload.PaymentId,
                ConversationId = payload.ConversationId,
                ErrorCode = payload.ErrorCode,
                ErrorMessage = payload.ErrorMessage
            };
        }

        private string CreateAuthorizationHeader(string uriPath, string jsonBody, string rnd)
        {
            var payload = string.IsNullOrWhiteSpace(jsonBody)
                ? rnd + uriPath
                : rnd + uriPath + jsonBody;

            var signature = ComputeHmacSha256Hex(payload, _options.SecretKey);

            var authorizationString =
                $"apiKey:{_options.ApiKey}&randomKey:{rnd}&signature:{signature}";

            var base64EncodedAuthorization =
                Convert.ToBase64String(Encoding.UTF8.GetBytes(authorizationString));

            return $"IYZWSv2 {base64EncodedAuthorization}";
        }

        private static string ComputeHmacSha256Hex(string data, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);

            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                //hexadecimal string
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
        private static CreatePaymentRequest MapRequest(ProcessPaymentRequest request)
        {
            return new CreatePaymentRequest
            {
                ConversationId = request.ConversationId,
                Price = request.Price,
                PaidPrice = request.PaidPrice,
                BasketId = request.BasketId,
                PaymentCard = new IyzicoPaymentCard
                {
                    CardHolderName = request.PaymentCard.CardHolderName,
                    CardNumber = request.PaymentCard.CardNumber,
                    ExpireMonth = request.PaymentCard.ExpireMonth,
                    ExpireYear = request.PaymentCard.ExpireYear,
                    Cvc = request.PaymentCard.Cvc,
                    RegisterCard = 0
                },
                Buyer = new IyzicoBuyer
                {
                    Id = request.BuyerId,
                    Name = request.BuyerFirstName,
                    Surname = request.BuyerLastName,
                    IdentityNumber = request.BuyerIdentityNumber,
                    Email = request.BuyerEmail,
                    GsmNumber = request.BuyerPhoneNumber,
                    RegistrationAddress = request.AddressLine,
                    City = request.City,
                    Country = request.Country,
                    ZipCode = request.ZipCode,
                    Ip = request.BuyerIp
                },
                ShippingAddress = new IyzicoAddress
                {
                    ContactName = request.ShippingContactName,
                    City = request.City,
                    Country = request.Country,
                    Address = request.AddressLine,
                    ZipCode = request.ZipCode
                },
                BillingAddress = new IyzicoAddress
                {
                    ContactName = request.ShippingContactName,
                    City = request.City,
                    Country = request.Country,
                    Address = request.AddressLine,
                    ZipCode = request.ZipCode
                },
                BasketItems = request.Items.Select(item => new IyzicoBasketItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    Category1 = item.Category1,
                    ItemType = item.ItemType,
                    Price = item.Price
                }).ToList()
            };
        }
    }
}
