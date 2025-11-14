![Header image](https://github.com/DougChisholm/App-Mod-Assist/blob/main/repo-header.png)

# Expense Management System - Azure Cloud-Native Solution

A modern, cloud-native expense management application built on Azure, featuring AI-powered chat assistance and comprehensive REST APIs.

## 🚀 Quick Start

```bash
# 1. Clone the repository
git clone <repository-url>
cd AMAFork07

# 2. Login to Azure
az login
az account set --subscription <your-subscription-id>

# 3. Deploy everything with one command
./deploy.sh
```

After deployment, navigate to: `https://app-expense-mgmt-{uniqueid}.azurewebsites.net/Index`

**⚠️ Important:** Access `/Index` endpoint, not just the root URL.

## 📋 Features

### Core Functionality
- ✅ **Expense Management** - Create, submit, approve/reject expenses
- ✅ **Role-Based Workflow** - Employee and Manager roles with approval process
- ✅ **Category Management** - Travel, Meals, Supplies, Accommodation, Other
- ✅ **Status Tracking** - Draft, Submitted, Approved, Rejected states
- ✅ **Currency Support** - GBP with proper minor unit handling (pence)

### Technical Features
- 🌐 **REST APIs** - Full CRUD operations with Swagger documentation
- 🎨 **Modern UI** - Bootstrap-based Razor Pages interface
- 🤖 **AI Chat Assistant** - Natural language interaction with expenses
- 📊 **Real-time Dashboard** - Summary cards and expense lists
- 🔒 **Secure** - Azure AD authentication, HTTPS enforced, managed identities

## 🏗️ Architecture

```
┌─────────────┐      ┌──────────────────┐      ┌─────────────────┐
│   Users     │─────▶│  Azure App       │─────▶│  Azure SQL      │
│             │      │  Service         │      │  Database       │
└─────────────┘      │  (.NET 8.0)      │      └─────────────────┘
                     │                  │
                     │  • Razor Pages   │
                     │  • REST APIs     │      ┌─────────────────┐
                     │  • Swagger       │─────▶│  Azure OpenAI   │
                     └──────────────────┘      │  (GPT-4)        │
                              │                └─────────────────┘
                              │
                              ▼                 ┌─────────────────┐
                     ┌──────────────────┐      │  Azure          │
                     │  Chat UI         │─────▶│  Cognitive      │
                     │  (HTML/JS)       │      │  Search (RAG)   │
                     └──────────────────┘      └─────────────────┘
```

See [ARCHITECTURE.md](ARCHITECTURE.md) for detailed diagrams and data flow.

## 📦 What Gets Deployed

| Resource | SKU | Purpose |
|----------|-----|---------|
| App Service Plan | F1 (Free) | Hosting for .NET application |
| App Service | Linux | Web application and APIs |
| Azure OpenAI | S0 | GPT-4 for chat assistant |
| Cognitive Search | Free | RAG document retrieval |
| SQL Database | Existing | Expense data storage |

## 🛠️ Technology Stack

- **Backend:** ASP.NET Core 8.0, Entity Framework Core
- **Frontend:** Razor Pages, Bootstrap 5, JavaScript
- **Database:** Azure SQL Database
- **AI:** Azure OpenAI (GPT-4), Azure Cognitive Search
- **Infrastructure:** Bicep, Azure CLI
- **API:** REST with Swagger/OpenAPI

## 📖 Documentation

- [DEPLOYMENT.md](DEPLOYMENT.md) - Detailed deployment instructions
- [ARCHITECTURE.md](ARCHITECTURE.md) - System architecture and diagrams  
- [TESTING.md](TESTING.md) - Testing guide and checklist
- [chatui/README.md](chatui/README.md) - Chat UI documentation
- [RAG/expense-system-knowledge.md](RAG/expense-system-knowledge.md) - Knowledge base

## 🔧 Local Development

### Prerequisites
- .NET 8.0 SDK
- Azure CLI
- Visual Studio Code or Visual Studio

### Run Locally
```bash
cd src/ExpenseManagementApp
dotnet restore
dotnet build
dotnet run
```

Access at:
- UI: http://localhost:5000/Index
- API: http://localhost:5000/swagger
- Chat: Open chatui/index.html in browser

## 🌐 API Endpoints

### Expenses
- `GET /api/expenses` - List all expenses
- `GET /api/expenses/{id}` - Get specific expense
- `POST /api/expenses` - Create new expense
- `POST /api/expenses/{id}/submit` - Submit for approval
- `POST /api/expenses/{id}/approve` - Approve expense
- `POST /api/expenses/{id}/reject` - Reject expense

### Users & Categories
- `GET /api/users` - List users
- `GET /api/categories` - List categories

**Full API documentation:** `/swagger` endpoint

## 🤖 AI Chat Assistant

The system includes an AI-powered chat interface that allows natural language interaction:

```
User: "Show me all pending expenses"
AI: [Lists all submitted expenses awaiting approval]

User: "Create a travel expense for £45.50"
AI: [Guides through expense creation]
```

**Demo Mode:** Works without Azure OpenAI configuration for testing
**Production:** Integrates with Azure OpenAI for true natural language understanding

## 💾 Database Schema

Based on the provided SQL schema with:
- **Users** - Employee and manager information
- **Roles** - Employee vs Manager
- **Expenses** - Expense claims with amounts in pence
- **ExpenseCategories** - Predefined categories
- **ExpenseStatus** - Workflow states

Connection: Uses existing Azure SQL Database with Active Directory authentication.

## 🔐 Security Features

- ✅ HTTPS enforced on all endpoints
- ✅ Azure AD Managed Identity for service-to-service auth
- ✅ SQL Database encrypted at rest and in transit
- ✅ TLS 1.2 minimum
- ✅ Connection strings secured in App Settings
- ✅ Input validation and SQL injection protection

## 🎯 Workflow

1. **Create** - Employee creates expense in Draft status
2. **Submit** - Employee submits expense for approval
3. **Review** - Manager approves or rejects
4. **Track** - View status and history

## 🧪 Testing

See [TESTING.md](TESTING.md) for comprehensive testing guide.

Quick test:
```bash
# Test API
curl https://your-app-url/api/expenses

# Test health
curl https://your-app-url/Index
```

## 📝 Configuration

### Database Connection
Set in `appsettings.json` or Azure App Service Configuration:
```json
{
  "ConnectionStrings": {
    "ExpenseDB": "Server=tcp:sql-expense-mgmt-xyz.database.windows.net,1433;Initial Catalog=ExpenseManagementDB;..."
  }
}
```

### GenAI Settings
Configure in `GenAISettings.json`:
```json
{
  "AzureOpenAI": {
    "Endpoint": "https://openai-expense-{id}.openai.azure.com/",
    "DeploymentName": "gpt-4",
    "UseManagedIdentity": true
  }
}
```

## 🚧 Troubleshooting

**App won't load:**
- Check you're accessing `/Index` not just `/`
- Verify App Service is running in Azure Portal

**Database errors:**
- Ensure managed identity has SQL access
- Check firewall rules

**API returns 404:**
- Verify URL includes `/api/` prefix
- Check Swagger docs for correct endpoints

## 🎓 Learning Resources

- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service/)
- [Azure OpenAI Service](https://docs.microsoft.com/azure/ai-services/openai/)
- [ASP.NET Core Best Practices](https://docs.microsoft.com/aspnet/core/fundamentals/best-practices)

## 📄 License

See [LICENSE](LICENSE) file.

## 🤝 Contributing

This is a demonstration project created using GitHub Copilot's coding agent to showcase app modernization from legacy screenshots and database schemas to cloud-native Azure solutions.

---

**Original Concept:** App-Mod-Assist by [DougChisholm](https://github.com/DougChisholm)

For questions or issues, refer to the documentation files or create an issue in the repository.
