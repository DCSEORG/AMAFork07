using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseManagementApp.Data;
using ExpenseManagementApp.Models;

namespace ExpenseManagementApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly ExpenseDbContext _context;
    private readonly ILogger<ExpensesController> _logger;

    public ExpensesController(ExpenseDbContext context, ILogger<ExpensesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all expenses
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
    {
        return await _context.Expenses
            .Include(e => e.User)
            .Include(e => e.Category)
            .Include(e => e.Status)
            .Include(e => e.Reviewer)
            .ToListAsync();
    }

    /// <summary>
    /// Get expense by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Expense>> GetExpense(int id)
    {
        var expense = await _context.Expenses
            .Include(e => e.User)
            .Include(e => e.Category)
            .Include(e => e.Status)
            .Include(e => e.Reviewer)
            .FirstOrDefaultAsync(e => e.ExpenseId == id);

        if (expense == null)
        {
            return NotFound();
        }

        return expense;
    }

    /// <summary>
    /// Get expenses by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Expense>>> GetExpensesByUser(int userId)
    {
        return await _context.Expenses
            .Include(e => e.User)
            .Include(e => e.Category)
            .Include(e => e.Status)
            .Include(e => e.Reviewer)
            .Where(e => e.UserId == userId)
            .ToListAsync();
    }

    /// <summary>
    /// Get expenses by status
    /// </summary>
    [HttpGet("status/{statusName}")]
    public async Task<ActionResult<IEnumerable<Expense>>> GetExpensesByStatus(string statusName)
    {
        return await _context.Expenses
            .Include(e => e.User)
            .Include(e => e.Category)
            .Include(e => e.Status)
            .Include(e => e.Reviewer)
            .Where(e => e.Status.StatusName == statusName)
            .ToListAsync();
    }

    /// <summary>
    /// Create new expense
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Expense>> CreateExpense(ExpenseCreateDto dto)
    {
        var expense = new Expense
        {
            UserId = dto.UserId,
            CategoryId = dto.CategoryId,
            StatusId = dto.StatusId,
            AmountMinor = dto.AmountMinor,
            Currency = dto.Currency ?? "GBP",
            ExpenseDate = dto.ExpenseDate,
            Description = dto.Description,
            ReceiptFile = dto.ReceiptFile,
            CreatedAt = DateTime.UtcNow
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExpense), new { id = expense.ExpenseId }, expense);
    }

    /// <summary>
    /// Update expense
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExpense(int id, ExpenseUpdateDto dto)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null)
        {
            return NotFound();
        }

        if (dto.CategoryId.HasValue)
            expense.CategoryId = dto.CategoryId.Value;
        if (dto.AmountMinor.HasValue)
            expense.AmountMinor = dto.AmountMinor.Value;
        if (dto.ExpenseDate.HasValue)
            expense.ExpenseDate = dto.ExpenseDate.Value;
        if (dto.Description != null)
            expense.Description = dto.Description;
        if (dto.ReceiptFile != null)
            expense.ReceiptFile = dto.ReceiptFile;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Submit expense for approval
    /// </summary>
    [HttpPost("{id}/submit")]
    public async Task<IActionResult> SubmitExpense(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null)
        {
            return NotFound();
        }

        var submittedStatus = await _context.ExpenseStatus
            .FirstOrDefaultAsync(s => s.StatusName == "Submitted");
        if (submittedStatus == null)
        {
            return BadRequest("Submitted status not found");
        }

        expense.StatusId = submittedStatus.StatusId;
        expense.SubmittedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(expense);
    }

    /// <summary>
    /// Approve expense
    /// </summary>
    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveExpense(int id, [FromBody] int reviewerId)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null)
        {
            return NotFound();
        }

        var approvedStatus = await _context.ExpenseStatus
            .FirstOrDefaultAsync(s => s.StatusName == "Approved");
        if (approvedStatus == null)
        {
            return BadRequest("Approved status not found");
        }

        expense.StatusId = approvedStatus.StatusId;
        expense.ReviewedBy = reviewerId;
        expense.ReviewedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(expense);
    }

    /// <summary>
    /// Reject expense
    /// </summary>
    [HttpPost("{id}/reject")]
    public async Task<IActionResult> RejectExpense(int id, [FromBody] int reviewerId)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null)
        {
            return NotFound();
        }

        var rejectedStatus = await _context.ExpenseStatus
            .FirstOrDefaultAsync(s => s.StatusName == "Rejected");
        if (rejectedStatus == null)
        {
            return BadRequest("Rejected status not found");
        }

        expense.StatusId = rejectedStatus.StatusId;
        expense.ReviewedBy = reviewerId;
        expense.ReviewedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(expense);
    }

    /// <summary>
    /// Delete expense
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExpense(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null)
        {
            return NotFound();
        }

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

// DTOs for API requests
public class ExpenseCreateDto
{
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public int StatusId { get; set; }
    public int AmountMinor { get; set; }
    public string? Currency { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string? Description { get; set; }
    public string? ReceiptFile { get; set; }
}

public class ExpenseUpdateDto
{
    public int? CategoryId { get; set; }
    public int? AmountMinor { get; set; }
    public DateTime? ExpenseDate { get; set; }
    public string? Description { get; set; }
    public string? ReceiptFile { get; set; }
}
