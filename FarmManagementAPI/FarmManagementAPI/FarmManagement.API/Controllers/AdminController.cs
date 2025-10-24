using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IAdminService _adminService;

    public AdminController(IUserRepository userRepository, IAdminService adminService)
    {
        _userRepository = userRepository;
        _adminService = adminService;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _adminService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("subscriptions")]
    public async Task<ActionResult<SubscriptionReportDto>> GetSubscriptionReport()
    {
        var report = await _adminService.GetSubscriptionReportAsync();
        return Ok(report);
    }

    [HttpGet("system-analytics")]
    public async Task<ActionResult<SystemAnalyticsDto>> GetSystemAnalytics()
    {
        var analytics = await _adminService.GetSystemAnalyticsAsync();
        return Ok(analytics);
    }
}