using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ExpenseManagementApp.Data;
using ExpenseManagementApp.Models;

namespace ExpenseManagementApp.Pages;

public class IndexModel : PageModel
{
    private readonly ExpenseDbContext _context;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ExpenseDbContext context, ILogger<IndexModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IList<Expense> Expenses { get; set; } = new List<Expense>();
    public IList<User> Users { get; set; } = new List<User>();
    public IList<ExpenseCategory> Categories { get; set; } = new List<ExpenseCategory>();
    public IList<ExpenseStatus> Statuses { get; set; } = new List<ExpenseStatus>();

    public async Task OnGetAsync()
    {
        Expenses = await _context.Expenses
            .Include(e => e.User)
            .Include(e => e.Category)
            .Include(e => e.Status)
            .Include(e => e.Reviewer)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        Users = await _context.Users
            .Include(u => u.Role)
            .ToListAsync();

        Categories = await _context.ExpenseCategories
            .Where(c => c.IsActive)
            .ToListAsync();

        Statuses = await _context.ExpenseStatus
            .ToListAsync();
    }
}
