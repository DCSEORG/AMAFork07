using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseManagementApp.Data;
using ExpenseManagementApp.Models;

namespace ExpenseManagementApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ExpenseDbContext _context;

    public UsersController(ExpenseDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Manager)
            .ToListAsync();
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Manager)
            .FirstOrDefaultAsync(u => u.UserId == id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    /// <summary>
    /// Get all managers
    /// </summary>
    [HttpGet("managers")]
    public async Task<ActionResult<IEnumerable<User>>> GetManagers()
    {
        return await _context.Users
            .Include(u => u.Role)
            .Where(u => u.Role.RoleName == "Manager")
            .ToListAsync();
    }
}
