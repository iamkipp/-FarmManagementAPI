using FarmManagementAPI.FarmManagement.Core.Interfaces.IServices;
using FarmManagementAPI.FarmManagement.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;

namespace FarmManagementAPI.FarmManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestMpesaController : ControllerBase
    {
        private readonly IMpesaClient _mpesaClient;
        private readonly ILogger<TestMpesaController> _logger;

        public TestMpesaController(IMpesaClient mpesaClient, ILogger<TestMpesaController> logger)
        {
            _mpesaClient = mpesaClient;
            _logger = logger;
        }

        // ✅ Simple health check
        [HttpGet("health")]
        public async Task<ActionResult> HealthCheck()
        {
            try
            {
                var token = await _mpesaClient.GetAccessTokenAsync();

                return Ok(new
                {
                    status = "Healthy",
                    hasToken = !string.IsNullOrEmpty(token),
                    tokenLength = token?.Length,
                    AccessToken = token
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in M-Pesa health check");
                return BadRequest(new { error = ex.Message });
            }
        }

        // ✅ Test STK Push
        [HttpPost("test-stk-push")]
        public async Task<ActionResult> TestStkPush()
        {
            try
            {
                var passkey = "bfb279f9aa9bdbcf158e97dd71a467cd2e0c893059b10f78e6b72ada1ed2c919";
                var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var businessShortCode = "174379";
                var password = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{businessShortCode}{passkey}{timestamp}"));

                var stkRequest = new StkPushRequest
                {
                    BusinessShortCode = businessShortCode,
                    Password = password,
                    Timestamp = timestamp,
                    TransactionType = "CustomerPayBillOnline",
                    Amount = "1",
                    PartyA = "254791700667",      // phone
                    PartyB = "174379",            // shortcode
                    PhoneNumber = "254791700667", // phone
                    CallBackURL = "https://www.callback.com/api/payments/callback",
                    AccountReference = "TestPayment",
                    TransactionDesc = "Test Transaction"
                };

                var response = await _mpesaClient.StkPushRequestAsync(stkRequest);

                return Ok(new
                {
                    success = response.ResponseCode == "0",
                    responseCode = response.ResponseCode,
                    responseDescription = response.ResponseDescription,
                    customerMessage = response.CustomerMessage,
                    checkoutRequestId = response.CheckoutRequestID,
                    merchantRequestId = response.MerchantRequestID
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test STK Push failed");
                return BadRequest(new { error = ex.Message });
            }
        }

        // ✅ Callback endpoint (simulated)
        [HttpPost("callback-test")]
        [AllowAnonymous]
        public ActionResult TestCallback([FromBody] PaymentCallbackDto callback)
        {
            _logger.LogInformation($"CALLBACK RECEIVED: {JsonSerializer.Serialize(callback)}");

            return Ok(new
            {
                ResultCode = 0,
                ResultDesc = "Success"
            });
        }
    }
}
