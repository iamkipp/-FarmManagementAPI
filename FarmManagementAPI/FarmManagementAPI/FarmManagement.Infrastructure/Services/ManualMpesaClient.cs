using FarmManagementAPI.FarmManagement.Core.Interfaces.IServices;
using FarmManagementAPI.FarmManagement.Shared.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FarmManagementAPI.FarmManagement.Infrastructure.Services
{
    public class ManualMpesaClient : IMpesaClient
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ManualMpesaClient> _logger;

        public ManualMpesaClient(
            IConfiguration config,
            IHttpClientFactory httpClientFactory,
            ILogger<ManualMpesaClient> logger)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // ✅ STEP 1: Get M-Pesa Access Token
        public async Task<string> GetAccessTokenAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var consumerKey = _config["Mpesa:ConsumerKey"]!;
                var consumerSecret = _config["Mpesa:ConsumerSecret"]!;

                var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{consumerKey}:{consumerSecret}"));
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {authString}");

                var baseUrl = _config["Mpesa:Environment"] == "Production"
                    ? "https://api.safaricom.co.ke"
                    : "https://sandbox.safaricom.co.ke";

                var response = await client.GetAsync($"{baseUrl}/oauth/v1/generate?grant_type=client_credentials");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"M-Pesa auth failed: {response.StatusCode}");
                    return string.Empty;
                }

                var content = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<JsonElement>(content);

                return tokenResponse.GetProperty("access_token").GetString()!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting M-Pesa access token");
                return string.Empty;
            }
        }

        // ✅ STEP 2: Reusable HTTP Helper (with Authorization header)
        private static async Task<string> MakeAuthorizedRequestAsync(string url, string method, string token, string? body = null)
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(new HttpMethod(method), url);

            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Headers.Add("Accept", "application/json");

            if (!string.IsNullOrEmpty(body) && (method == "POST" || method == "PUT"))
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        // ✅ STEP 3: Send STK Push Request
        public async Task<StkPushResponse> StkPushRequestAsync(StkPushRequest request)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    return new StkPushResponse
                    {
                        ResponseCode = "1",
                        ResponseDescription = "Failed to get access token"
                    };
                }

                var baseUrl = _config["Mpesa:Environment"] == "Production"
                    ? "https://api.safaricom.co.ke"
                    : "https://sandbox.safaricom.co.ke";

                var url = $"{baseUrl}/mpesa/stkpush/v1/processrequest";
                var json = JsonSerializer.Serialize(request);

                var responseContent = await MakeAuthorizedRequestAsync(url, "POST", accessToken, json);

                var result = JsonSerializer.Deserialize<StkPushResponse>(responseContent);
                return result ?? new StkPushResponse
                {
                    ResponseCode = "1",
                    ResponseDescription = "Failed to parse STK push response"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in STK push request");
                return new StkPushResponse
                {
                    ResponseCode = "1",
                    ResponseDescription = ex.Message
                };
            }
        }
    }
}
