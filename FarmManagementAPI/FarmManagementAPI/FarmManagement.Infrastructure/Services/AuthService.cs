using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using FarmManagementAPI.FarmManagement.Infrastructure.Data;
using FarmManagementAPI.FarmManagement.Core.Interfaces.IServices;
using FarmManagementAPI.FarmManagement.Shared.Dtos;
using FarmManagementAPI.FarmManagement.Core.Entities;
using FarmManagementAPI.FarmManagement.Core.Interfaces.Repositories;

namespace FarmManagementAPI.FarmManagement.Infrastructure.Services;

public class AuthService(
    IUserRepository userRepository,
    ITokenService tokenService,
    ApplicationDbContext context,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<AuthService> _logger = logger;

    public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
    {
        if (await _userRepository.UserExistsAsync(registerDto.Email))
            throw new Exception("User with this email already exists");

        // Validate phone number format for M-Pesa
        if (!registerDto.PhoneNumber.StartsWith("254") || registerDto.PhoneNumber.Length != 12)
            throw new Exception("Phone number must be in format 254XXXXXXXXX");

        using var hmac = new HMACSHA512();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = registerDto.Email.ToLower(),
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            PhoneNumber = registerDto.PhoneNumber,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key,
            Role = "Farmer",
            CreatedAt = DateTime.UtcNow
        };

        // Create 30-day trial subscription
        user.Subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30), // 30-day trial
            IsActive = true,
            IsTrial = true,
            Status = "Active"
        };

        await _userRepository.AddUserAsync(user);

        _logger.LogInformation($"New user registered: {user.Email}");

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            PhoneNumber = user.PhoneNumber
        };
    }

    public async Task<string> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);

        if (user == null)
            throw new Exception("Invalid email");

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        if (!computedHash.SequenceEqual(user.PasswordHash))
            throw new Exception("Invalid password");

        // Check if subscription is still active
        if (user.Subscription?.EndDate < DateTime.UtcNow && user.Subscription.IsActive)
        {
            user.Subscription.IsActive = false;
            user.Subscription.Status = "Expired";
            await _context.SaveChangesAsync();
            throw new Exception("Subscription has expired. Please renew to continue using the service.");
        }

        return _tokenService.CreateToken(user);
    }

    public async Task<UserDto> GetCurrentUserAsync(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        if (user == null)
            throw new Exception("User not found");

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            PhoneNumber = user.PhoneNumber
        };
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
            return false;

        // Verify current password
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(currentPassword));

        if (!computedHash.SequenceEqual(user.PasswordHash))
            return false;

        // Update to new password
        using var newHmac = new HMACSHA512();
        user.PasswordHash = newHmac.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
        user.PasswordSalt = newHmac.Key;

        await _userRepository.UpdateUserAsync(user);
        return true;
    }
}