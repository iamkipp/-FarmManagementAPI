using FarmManagementAPI.FarmManagement.Core.Entities;
using FarmManagementAPI.FarmManagement.Shared.Dtos;
using FarmManagementAPI.FarmManagement.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FarmManagementAPI.FarmManagement.Core.Interfaces.IServices;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IUserRepository _userRepository;

    public SubscriptionsController(ISubscriptionService subscriptionService, IUserRepository userRepository)
    {
        _subscriptionService = subscriptionService;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<Subscription>> GetCurrentSubscription()
    {
        var userId = GetCurrentUserId();
        var user = await _userRepository.GetUserByIdAsync(userId);
        return Ok(user?.Subscription);
    }

    [HttpGet("status")]
    public async Task<ActionResult<SubscriptionStatusDto>> GetSubscriptionStatus()
    {
        var userId = GetCurrentUserId();
        var status = await _subscriptionService.GetSubscriptionStatusAsync(userId);
        return Ok(status);
    }

    [HttpPost("cancel")]
    public async Task<IActionResult> CancelSubscription()
    {
        var userId = GetCurrentUserId();
        await _subscriptionService.CancelSubscriptionAsync(userId);
        return Ok(new { message = "Subscription cancelled successfully" });
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userId!);
    }
}