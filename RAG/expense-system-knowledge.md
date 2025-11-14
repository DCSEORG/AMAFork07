# Expense Management System - Knowledge Base

## System Overview
The Expense Management System is a web-based application for managing employee expenses with approval workflows.

## Key Features
- **Expense Creation**: Employees can create and submit expense claims
- **Categories**: Travel, Meals, Supplies, Accommodation, and Other
- **Approval Workflow**: Submitted expenses require manager approval
- **Status Tracking**: Draft, Submitted, Approved, Rejected

## Database Schema

### Users Table
- UserId (Primary Key)
- UserName
- Email (Unique)
- RoleId (Foreign Key to Roles)
- ManagerId (Self-referencing to another User)
- IsActive
- CreatedAt

### Roles Table
- RoleId (Primary Key)
- RoleName (Employee or Manager)
- Description

### Expenses Table
- ExpenseId (Primary Key)
- UserId (Foreign Key to Users)
- CategoryId (Foreign Key to ExpenseCategories)
- StatusId (Foreign Key to ExpenseStatus)
- AmountMinor (Amount in pence, e.g., £12.34 = 1234)
- Currency (Default: GBP)
- ExpenseDate
- Description
- ReceiptFile
- SubmittedAt
- ReviewedBy (Foreign Key to Users)
- ReviewedAt
- CreatedAt

### ExpenseCategories Table
- CategoryId (Primary Key)
- CategoryName
- IsActive

### ExpenseStatus Table
- StatusId (Primary Key)
- StatusName (Draft, Submitted, Approved, Rejected)

## API Endpoints

### Expenses
- GET /api/expenses - Get all expenses
- GET /api/expenses/{id} - Get expense by ID
- GET /api/expenses/user/{userId} - Get expenses by user
- GET /api/expenses/status/{statusName} - Get expenses by status
- POST /api/expenses - Create new expense
- PUT /api/expenses/{id} - Update expense
- POST /api/expenses/{id}/submit - Submit expense for approval
- POST /api/expenses/{id}/approve - Approve expense (requires reviewerId in body)
- POST /api/expenses/{id}/reject - Reject expense (requires reviewerId in body)
- DELETE /api/expenses/{id} - Delete expense

### Users
- GET /api/users - Get all users
- GET /api/users/{id} - Get user by ID
- GET /api/users/managers - Get all managers

### Categories
- GET /api/categories - Get all categories
- GET /api/categories/{id} - Get category by ID

## Workflow Rules

### Creating an Expense
1. Employee fills in: category, amount, date, description
2. Expense is created with "Draft" status
3. Employee can edit draft expenses

### Submitting for Approval
1. Employee clicks "Submit" on a draft expense
2. Status changes from "Draft" to "Submitted"
3. SubmittedAt timestamp is recorded

### Manager Approval
1. Manager views submitted expenses
2. Manager can either:
   - Approve: Status → "Approved", ReviewedBy and ReviewedAt are set
   - Reject: Status → "Rejected", ReviewedBy and ReviewedAt are set

## Currency Handling
- All amounts are stored in minor units (pence for GBP)
- To convert: £12.34 = 1234 pence
- To display: 1234 pence = £12.34

## Common Questions and Answers

**Q: How do I create a new expense?**
A: Use POST /api/expenses with userId, categoryId, statusId (usually 1 for Draft), amountMinor (in pence), expenseDate, and optional description.

**Q: How do I submit an expense for approval?**
A: Use POST /api/expenses/{id}/submit endpoint.

**Q: How do I approve an expense?**
A: Use POST /api/expenses/{id}/approve endpoint with the manager's userId in the request body.

**Q: What categories are available?**
A: Travel, Meals, Supplies, Accommodation, and Other.

**Q: How do I see pending expenses?**
A: Use GET /api/expenses/status/Submitted endpoint.

**Q: How do I see all expenses for a specific user?**
A: Use GET /api/expenses/user/{userId} endpoint.
