# Azure Architecture Diagram - Expense Management System

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                          EXPENSE MANAGEMENT SYSTEM                           │
│                         Cloud-Native Azure Solution                          │
└─────────────────────────────────────────────────────────────────────────────┘

                                    ┌──────────┐
                                    │  Users   │
                                    └────┬─────┘
                                         │
                                         │ HTTPS
                                         ▼
                            ┌────────────────────────┐
                            │   Azure App Service    │
                            │  (Linux, .NET 8.0)     │
                            │                        │
                            │  ┌──────────────────┐  │
                            │  │  Razor Pages UI  │  │
                            │  │   /Index         │  │
                            │  └──────────────────┘  │
                            │                        │
                            │  ┌──────────────────┐  │
                            │  │   REST APIs      │  │
                            │  │  /api/expenses   │  │
                            │  │  /api/users      │  │
                            │  │  /api/categories │  │
                            │  └──────────────────┘  │
                            │                        │
                            │  ┌──────────────────┐  │
                            │  │  Swagger/OpenAPI │  │
                            │  │    /swagger      │  │
                            │  └──────────────────┘  │
                            └───┬────────────────┬───┘
                                │                │
                   ┌────────────┘                └────────────┐
                   │                                          │
                   │ SQL Connection                           │ Managed Identity
                   │ (Azure AD Auth)                          │
                   ▼                                          ▼
        ┌──────────────────────┐              ┌──────────────────────────┐
        │   Azure SQL Database │              │   Azure OpenAI Service   │
        │                      │              │                          │
        │  ┌────────────────┐  │              │  ┌────────────────────┐  │
        │  │ ExpenseDB      │  │              │  │   GPT-4 Deployment │  │
        │  │                │  │              │  │                    │  │
        │  │ • Users        │  │              │  │ • NL Understanding │  │
        │  │ • Roles        │  │              │  │ • Function Calling │  │
        │  │ • Expenses     │  │              │  │ • Chat Completion  │  │
        │  │ • Categories   │  │              │  └────────────────────┘  │
        │  │ • Status       │  │              └────────────┬─────────────┘
        │  └────────────────┘  │                           │
        └──────────────────────┘                           │
                                                            │
                                                            │ Search Index
                                                            ▼
                                              ┌──────────────────────────┐
                                              │ Azure Cognitive Search   │
                                              │                          │
                                              │  ┌────────────────────┐  │
                                              │  │  expense-knowledge │  │
                                              │  │                    │  │
                                              │  │ • RAG Documents    │  │
                                              │  │ • System Context   │  │
                                              │  │ • API Schemas      │  │
                                              │  └────────────────────┘  │
                                              └──────────────────────────┘
                                                            ▲
                                                            │
                                                            │
                                              ┌─────────────┴────────────┐
                                              │                          │
                                              │   Chat UI (Static)       │
                                              │   /chatui/index.html     │
                                              │                          │
                                              │  • Natural Language      │
                                              │  • Function Calls        │
                                              │  • RAG Integration       │
                                              └──────────────────────────┘


═══════════════════════════════════════════════════════════════════════════════

RESOURCE DETAILS:

┌─────────────────────────────────────────────────────────────────────────────┐
│ Resource Group: rg-expense-mgmt-dev                                         │
│ Location: UK South                                                          │
│ Environment: Development                                                    │
└─────────────────────────────────────────────────────────────────────────────┘

📦 App Service Plan
   • Name: asp-expense-mgmt-{uniqueid}
   • SKU: F1 (Free Tier)
   • OS: Linux
   • Runtime: .NET 8.0

🌐 App Service
   • Name: app-expense-mgmt-{uniqueid}
   • URL: https://app-expense-mgmt-{uniqueid}.azurewebsites.net/Index
   • HTTPS Only: Enabled
   • Authentication: Azure AD (via Managed Identity)

💾 Azure SQL Database
   • Server: sql-expense-mgmt-xyz.database.windows.net
   • Database: ExpenseManagementDB
   • Authentication: Active Directory Default
   • Connection: Managed Identity

🤖 Azure OpenAI
   • Name: openai-expense-{uniqueid}
   • SKU: S0 (Standard)
   • Deployment: gpt-4 (GPT-4 model)
   • Endpoint: https://openai-expense-{uniqueid}.openai.azure.com/

🔍 Azure Cognitive Search
   • Name: search-expense-{uniqueid}
   • SKU: Free Tier
   • Index: expense-knowledge
   • Purpose: RAG document retrieval

═══════════════════════════════════════════════════════════════════════════════

DATA FLOW:

1. User Access
   └─> HTTPS to App Service
       └─> Razor Pages UI displays expense list
           └─> JavaScript calls REST APIs

2. API Requests
   └─> App Service REST APIs (/api/expenses, etc.)
       └─> Entity Framework Core
           └─> Azure SQL Database (via Managed Identity)

3. AI Chat Interaction
   └─> User types natural language query in Chat UI
       └─> Azure OpenAI processes request
           ├─> Retrieves context from Cognitive Search (RAG)
           └─> Calls appropriate function (API endpoint)
               └─> Returns formatted response

4. Expense Workflow
   └─> Create (Draft) → Submit → Approve/Reject
       └─> Each state change persisted to Azure SQL
           └─> Timestamps and reviewer tracked

═══════════════════════════════════════════════════════════════════════════════

SECURITY:

🔒 Authentication & Authorization
   • Azure AD Managed Identity for service-to-service
   • SQL Database uses Azure AD authentication
   • OpenAI access via Managed Identity

🔐 Network Security
   • HTTPS enforced on all endpoints
   • TLS 1.2 minimum
   • SQL Database firewall rules

🛡️ Data Protection
   • Encryption at rest (SQL Database)
   • Encryption in transit (TLS)
   • Connection strings in App Settings (encrypted)

═══════════════════════════════════════════════════════════════════════════════

DEPLOYMENT:

📋 Infrastructure as Code
   • Bicep templates in /infrastructure
   • main.bicep - App Service resources
   • genai.bicep - AI resources

🚀 Deployment Script
   • deploy.sh - Single script deployment
   • Creates resource group
   • Deploys all infrastructure
   • Deploys application code (app.zip)

📦 Application Package
   • app.zip - Published .NET application
   • Includes all dependencies
   • Ready for App Service deployment

═══════════════════════════════════════════════════════════════════════════════

MONITORING (Recommended):

📊 Application Insights
   • Performance monitoring
   • Error tracking
   • User analytics

📝 Log Analytics
   • Centralized logging
   • Query and analysis
   • Alerting

🔔 Alerts
   • Failed requests
   • High response times
   • Resource utilization

═══════════════════════════════════════════════════════════════════════════════
```

## Component Interactions

### 1. Expense Creation Flow
```
User → UI → POST /api/expenses → App Service → EF Core → Azure SQL → Response → UI
```

### 2. Approval Flow
```
Manager → UI → POST /api/expenses/{id}/approve → App Service → Update DB → Response
```

### 3. AI Chat Flow
```
User → Chat UI → Azure OpenAI → RAG (Search) → Function Call → API → Response
```

## Scalability Considerations

- **App Service**: Can scale up/out as needed
- **SQL Database**: Can increase DTUs or move to elastic pool
- **OpenAI**: Rate limits based on SKU, can request increases
- **Search**: Can upgrade tier for more capacity

## Cost Optimization

Current setup uses low-cost development SKUs:
- App Service: Free (F1) tier
- SQL Database: Existing resource (shared)
- OpenAI: S0 tier (pay per use)
- Cognitive Search: Free tier

For production, consider:
- App Service: B1 or S1 tier
- SQL Database: Based on workload analysis
- OpenAI: Scale based on usage patterns
- Enable autoscaling where appropriate
