using FarmManagementAPI.FarmManagement.Shared.Dtos;
using System.Text;
using System.Text.Json;

namespace FarmManagementAPI.FarmManagement.Infrastructure.Services
{
    public class ManualMpesaClient
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

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var baseUrl = _config["Mpesa:Environment"] == "Production"
                    ? "https://api.safaricom.co.ke"
                    : "https://sandbox.safaricom.co.ke";

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{baseUrl}/mpesa/stkpush/v1/processrequest", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"STK push failed: {responseContent}");
                    return new StkPushResponse
                    {
                        ResponseCode = "1",
                        ResponseDescription = "STK push request failed"
                    };
                }

                var result = JsonSerializer.Deserialize<StkPushResponse>(responseContent);
                return result ?? new StkPushResponse { ResponseCode = "1", ResponseDescription = "Failed to parse response" };
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
