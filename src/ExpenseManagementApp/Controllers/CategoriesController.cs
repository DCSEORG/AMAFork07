using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseManagementApp.Data;
using ExpenseManagementApp.Models;

namespace ExpenseManagementApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ExpenseDbContext _context;

    public CategoriesController(ExpenseDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all expense categories
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseCategory>>> GetCategories()
    {
        return await _context.ExpenseCategories
            .Where(c => c.IsActive)
            .ToListAsync();
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseCategory>> GetCategory(int id)
    {
        var category = await _context.ExpenseCategories.FindAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        return category;
    }
}
