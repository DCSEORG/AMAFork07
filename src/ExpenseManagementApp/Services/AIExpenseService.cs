using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExpenseManagementApp.Services;

/// <summary>
/// Service for integrating Azure OpenAI with expense management APIs
/// Provides function calling capabilities for natural language interactions
/// </summary>
public class AIExpenseService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AIExpenseService> _logger;

    public AIExpenseService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AIExpenseService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Get available functions that can be called by the AI
    /// </summary>
    public List<FunctionDefinition> GetAvailableFunctions()
    {
        return new List<FunctionDefinition>
        {
            new FunctionDefinition
            {
                Name = "get_expenses",
                Description = "Get all expenses in the system",
                Parameters = new { type = "object", properties = new { } }
            },
            new FunctionDefinition
            {
                Name = "get_expenses_by_status",
                Description = "Get expenses filtered by status (Draft, Submitted, Approved, Rejected)",
                Parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        status = new
                        {
                            type = "string",
                            description = "The status to filter by",
                            @enum = new[] { "Draft", "Submitted", "Approved", "Rejected" }
                        }
                    },
                    required = new[] { "status" }
                }
            },
            new FunctionDefinition
            {
                Name = "create_expense",
                Description = "Create a new expense",
                Parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        userId = new { type = "integer", description = "The ID of the user creating the expense" },
                        categoryId = new { type = "integer", description = "The category ID (1=Travel, 2=Meals, 3=Supplies, 4=Accommodation, 5=Other)" },
                        amount = new { type = "number", description = "The amount in GBP" },
                        date = new { type = "string", description = "The expense date (YYYY-MM-DD)" },
                        description = new { type = "string", description = "Description of the expense" }
                    },
                    required = new[] { "userId", "categoryId", "amount", "date" }
                }
            },
            new FunctionDefinition
            {
                Name = "submit_expense",
                Description = "Submit an expense for approval",
                Parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        expenseId = new { type = "integer", description = "The ID of the expense to submit" }
                    },
                    required = new[] { "expenseId" }
                }
            },
            new FunctionDefinition
            {
                Name = "approve_expense",
                Description = "Approve an expense",
                Parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        expenseId = new { type = "integer", description = "The ID of the expense to approve" },
                        reviewerId = new { type = "integer", description = "The ID of the manager approving" }
                    },
                    required = new[] { "expenseId", "reviewerId" }
                }
            },
            new FunctionDefinition
            {
                Name = "reject_expense",
                Description = "Reject an expense",
                Parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        expenseId = new { type = "integer", description = "The ID of the expense to reject" },
                        reviewerId = new { type = "integer", description = "The ID of the manager rejecting" }
                    },
                    required = new[] { "expenseId", "reviewerId" }
                }
            },
            new FunctionDefinition
            {
                Name = "get_users",
                Description = "Get all users in the system",
                Parameters = new { type = "object", properties = new { } }
            },
            new FunctionDefinition
            {
                Name = "get_categories",
                Description = "Get all expense categories",
                Parameters = new { type = "object", properties = new { } }
            }
        };
    }

    /// <summary>
    /// Execute a function call from the AI
    /// This is a simple implementation - in production, would use proper dependency injection
    /// </summary>
    public async Task<string> ExecuteFunctionAsync(string functionName, JsonElement arguments)
    {
        _logger.LogInformation("Executing function: {FunctionName} with arguments: {Arguments}", 
            functionName, arguments.ToString());

        var client = _httpClientFactory.CreateClient();
        var baseUrl = _configuration["BaseUrl"] ?? "http://localhost:5000";

        try
        {
            switch (functionName)
            {
                case "get_expenses":
                    var allExpenses = await client.GetStringAsync($"{baseUrl}/api/expenses");
                    return allExpenses;

                case "get_expenses_by_status":
                    var status = arguments.GetProperty("status").GetString();
                    var filteredExpenses = await client.GetStringAsync($"{baseUrl}/api/expenses/status/{status}");
                    return filteredExpenses;

                case "create_expense":
                    var createData = new
                    {
                        userId = arguments.GetProperty("userId").GetInt32(),
                        categoryId = arguments.GetProperty("categoryId").GetInt32(),
                        statusId = 1, // Draft
                        amountMinor = (int)(arguments.GetProperty("amount").GetDouble() * 100),
                        currency = "GBP",
                        expenseDate = arguments.GetProperty("date").GetString(),
                        description = arguments.TryGetProperty("description", out var desc) ? desc.GetString() : null
                    };
                    var createContent = new StringContent(
                        JsonSerializer.Serialize(createData),
                        System.Text.Encoding.UTF8,
                        "application/json");
                    var createResponse = await client.PostAsync($"{baseUrl}/api/expenses", createContent);
                    return await createResponse.Content.ReadAsStringAsync();

                case "submit_expense":
                    var submitId = arguments.GetProperty("expenseId").GetInt32();
                    var submitResponse = await client.PostAsync($"{baseUrl}/api/expenses/{submitId}/submit", null);
                    return await submitResponse.Content.ReadAsStringAsync();

                case "approve_expense":
                    var approveId = arguments.GetProperty("expenseId").GetInt32();
                    var reviewerId = arguments.GetProperty("reviewerId").GetInt32();
                    var approveContent = new StringContent(
                        JsonSerializer.Serialize(reviewerId),
                        System.Text.Encoding.UTF8,
                        "application/json");
                    var approveResponse = await client.PostAsync($"{baseUrl}/api/expenses/{approveId}/approve", approveContent);
                    return await approveResponse.Content.ReadAsStringAsync();

                case "reject_expense":
                    var rejectId = arguments.GetProperty("expenseId").GetInt32();
                    var rejectReviewerId = arguments.GetProperty("reviewerId").GetInt32();
                    var rejectContent = new StringContent(
                        JsonSerializer.Serialize(rejectReviewerId),
                        System.Text.Encoding.UTF8,
                        "application/json");
                    var rejectResponse = await client.PostAsync($"{baseUrl}/api/expenses/{rejectId}/reject", rejectContent);
                    return await rejectResponse.Content.ReadAsStringAsync();

                case "get_users":
                    var users = await client.GetStringAsync($"{baseUrl}/api/users");
                    return users;

                case "get_categories":
                    var categories = await client.GetStringAsync($"{baseUrl}/api/categories");
                    return categories;

                default:
                    throw new NotImplementedException($"Function {functionName} is not implemented");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing function {FunctionName}", functionName);
            throw;
        }
    }
}

/// <summary>
/// Function definition for Azure OpenAI function calling
/// </summary>
public class FunctionDefinition
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("parameters")]
    public object Parameters { get; set; } = new { };
}
