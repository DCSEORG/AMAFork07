# Expense Management System - Testing Guide

## Overview
This document provides instructions for testing the modernized expense management application.

## Prerequisites

Before testing, ensure:
1. Azure CLI is installed and configured
2. You have access to an Azure subscription
3. You've deployed the infrastructure using `./deploy.sh`

## Local Testing

### 1. Build and Run Locally

```bash
cd src/ExpenseManagementApp
dotnet build
dotnet run
```

The application will start at `http://localhost:5000` (or the port shown in console).

### 2. Access the Application

Navigate to:
- Main UI: `http://localhost:5000/Index`
- Swagger API: `http://localhost:5000/swagger`
- Chat UI: Open `chatui/index.html` in a browser

### 3. Test API Endpoints

Using Swagger UI or curl:

**Get all expenses:**
```bash
curl http://localhost:5000/api/expenses
```

**Get categories:**
```bash
curl http://localhost:5000/api/categories
```

**Get users:**
```bash
curl http://localhost:5000/api/users
```

**Create an expense:**
```bash
curl -X POST http://localhost:5000/api/expenses \
  -H "Content-Type: application/json" \
  -d '{
    "userId": 1,
    "categoryId": 1,
    "statusId": 1,
    "amountMinor": 2540,
    "currency": "GBP",
    "expenseDate": "2025-11-14",
    "description": "Test expense"
  }'
```

**Submit expense for approval:**
```bash
curl -X POST http://localhost:5000/api/expenses/1/submit
```

**Approve expense:**
```bash
curl -X POST http://localhost:5000/api/expenses/1/approve \
  -H "Content-Type: application/json" \
  -d '2'
```

### 4. Test UI Functionality

**Main Interface (/Index):**
- [ ] Page loads successfully
- [ ] Expense list displays
- [ ] Summary cards show correct counts
- [ ] "New Expense" button opens modal
- [ ] Create new expense form works
- [ ] Submit button changes expense status
- [ ] Approve/Reject buttons work

**Chat Interface (chatui/index.html):**
- [ ] Page loads with welcome message
- [ ] Can type messages
- [ ] Demo mode responds to queries
- [ ] Function call display works
- [ ] Typing indicator appears

## Azure Deployment Testing

### 1. Deploy to Azure

```bash
./deploy.sh
```

Monitor the deployment output for:
- Resource group creation
- App Service deployment
- GenAI resources deployment
- Application package deployment

### 2. Access Deployed Application

After deployment completes, the script will output the URL:
```
App URL: https://app-expense-mgmt-{uniqueid}.azurewebsites.net/Index
```

**Important:** Navigate to `/Index`, not just the root URL.

### 3. Test Deployed Application

Run the same tests as local testing, but using the Azure URL.

### 4. Test Database Connection

The application should connect to the existing Azure SQL Database:
- Server: `sql-expense-mgmt-xyz.database.windows.net`
- Database: `ExpenseManagementDB`
- Auth: Active Directory Default (Managed Identity)

**Note:** Ensure the App Service's managed identity has been granted access to the SQL Database.

## API Testing Checklist

### Expenses API
- [ ] GET /api/expenses - List all expenses
- [ ] GET /api/expenses/{id} - Get specific expense
- [ ] GET /api/expenses/user/{userId} - Filter by user
- [ ] GET /api/expenses/status/{status} - Filter by status
- [ ] POST /api/expenses - Create new expense
- [ ] PUT /api/expenses/{id} - Update expense
- [ ] POST /api/expenses/{id}/submit - Submit for approval
- [ ] POST /api/expenses/{id}/approve - Approve expense
- [ ] POST /api/expenses/{id}/reject - Reject expense
- [ ] DELETE /api/expenses/{id} - Delete expense

### Users API
- [ ] GET /api/users - List all users
- [ ] GET /api/users/{id} - Get specific user
- [ ] GET /api/users/managers - List managers

### Categories API
- [ ] GET /api/categories - List all categories
- [ ] GET /api/categories/{id} - Get specific category

## GenAI Testing

### Chat UI Demo Mode

Test queries:
- [ ] "Show me all pending expenses"
- [ ] "Show me all expenses"
- [ ] "Show me approved expenses"
- [ ] "Help"

### Azure OpenAI Integration (Post-Configuration)

After configuring Azure OpenAI:
1. Update `chatui/index.html` with endpoint and key
2. Test natural language queries
3. Verify function calling works
4. Check RAG integration

## Performance Testing

### Load Testing

Use a tool like Apache Bench or k6:

```bash
# Test GET requests
ab -n 1000 -c 10 https://your-app-url/api/expenses

# Test POST requests
ab -n 100 -c 5 -p expense.json -T application/json https://your-app-url/api/expenses
```

### Expected Response Times
- API requests: < 200ms
- Page loads: < 1s
- Database queries: < 100ms

## Troubleshooting

### Application Won't Start
- Check connection string in appsettings.json
- Verify all NuGet packages are restored
- Check .NET 8.0 SDK is installed

### Database Connection Fails
- Verify managed identity has SQL access
- Check firewall rules on SQL Server
- Confirm connection string is correct

### API Returns 404
- Ensure URL includes `/api/` prefix
- Check routing in controllers
- Verify controller is registered

### Swagger Not Loading
- Confirm app is in Development mode, or
- Enable Swagger in Production in Program.cs

### Chat UI Not Working
- Check browser console for errors
- Verify API endpoints are accessible
- Ensure CORS is configured if needed

## Security Testing

### Authentication
- [ ] Verify managed identity is used for SQL
- [ ] Check Azure AD authentication works
- [ ] Confirm HTTPS is enforced

### Data Validation
- [ ] Test with invalid input data
- [ ] Verify SQL injection protection
- [ ] Check XSS prevention

### Authorization
- [ ] Verify users can only access their data
- [ ] Check manager approval permissions
- [ ] Test role-based access

## Test Data

### Sample Users
- Alice Example (Employee) - ID: 1
- Bob Manager (Manager) - ID: 2

### Sample Categories
1. Travel
2. Meals
3. Supplies
4. Accommodation
5. Other

### Sample Statuses
1. Draft
2. Submitted
3. Approved
4. Rejected

## Success Criteria

The application is considered successfully tested when:
- [ ] All API endpoints return expected responses
- [ ] UI displays data correctly
- [ ] Create/Update/Delete operations work
- [ ] Approval workflow functions properly
- [ ] Swagger documentation is accessible
- [ ] Chat UI demo mode works
- [ ] Application deploys to Azure successfully
- [ ] Database connection is established
- [ ] No security vulnerabilities detected

## Next Steps After Testing

1. Configure Azure OpenAI with actual credentials
2. Set up Azure Cognitive Search index
3. Implement proper authentication
4. Add Application Insights monitoring
5. Configure alerts and diagnostics
6. Set up CI/CD pipeline
7. Implement automated testing
8. Plan for production deployment
