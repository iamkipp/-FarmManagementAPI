using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        IPaymentService paymentService,
        IUserRepository userRepository,
        ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpPost("initiate")]
    public async Task<ActionResult<PaymentResponseDto>> InitiatePayment(InitiatePaymentDto paymentDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var response = await _paymentService.InitiateStkPushAsync(paymentDto, userId);

            if (response.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating payment");
            return BadRequest(new PaymentResponseDto
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpPost("callback")]
    [AllowAnonymous] // M-Pesa calls this endpoint anonymously
    public async Task<IActionResult> PaymentCallback([FromBody] PaymentCallbackDto callbackDto)
    {
        try
        {
            _logger.LogInformation($"Received payment callback: {JsonSerializer.Serialize(callbackDto)}");

            var success = await _paymentService.HandlePaymentCallbackAsync(callbackDto);

            if (success)
            {
                // Return success response to M-Pesa
                return Ok(new
                {
                    ResultCode = 0,
                    ResultDesc = "Success"
                });
            }
            else
            {
                return Ok(new
                {
                    ResultCode = 1,
                    ResultDesc = "Failed"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment callback");
            return Ok(new
            {
                ResultCode = 1,
                ResultDesc = "Failed"
            });
        }
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<Payment>>> GetPaymentHistory()
    {
        try
        {
            var userId = GetCurrentUserId();
            var payments = await _paymentService.GetUserPaymentHistoryAsync(userId);
            return Ok(payments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment history");
            return BadRequest("Error retrieving payment history");
        }
    }

    [HttpGet("verify/{checkoutRequestId}")]
    public async Task<ActionResult<bool>> VerifyPayment(string checkoutRequestId)
    {
        try
        {
            var isVerified = await _paymentService.VerifyPaymentAsync(checkoutRequestId);
            return Ok(isVerified);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying payment");
            return BadRequest(false);
        }
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new Exception("User not authenticated");

        return Guid.Parse(userId);
    }
}