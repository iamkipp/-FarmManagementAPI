using Microsoft.EntityFrameworkCore;
using FarmManagementAPI.FarmManagement.Core.Entities;
using FarmManagementAPI.FarmManagement.Core.Interfaces.Repositories;
using FarmManagementAPI.FarmManagement.Infrastructure.Data;

namespace FarmManagementAPI.FarmManagement.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Farms)
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Farms)
            .ThenInclude(f => f.FarmRecords)
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.Subscription)
            .Include(u => u.Farms)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
    {
        return await _context.Users
            .Include(u => u.Subscription)
            .Where(u => u.Role == role)
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();
    }
}