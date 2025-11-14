# Deployment Instructions

## Quick Start - One Line Deployment

After cloning this repository and logging into Azure CLI, run:

```bash
./deploy.sh
```

## Prerequisites

1. Azure CLI installed ([Installation Guide](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli))
2. Azure subscription with appropriate permissions
3. Logged into Azure CLI: `az login`
4. Set your subscription context: `az account set --subscription <subscription-id>`

## What Gets Deployed

The deployment script will create:

1. **Resource Group** (`rg-expense-mgmt-dev`) in UK South
2. **App Service Plan** (Free/F1 tier for development)
3. **App Service** (Linux, .NET 8.0)
4. **Azure OpenAI Service** (for GenAI chat functionality)
5. **Azure Cognitive Search** (for RAG pattern)
6. **Application Code** (if app.zip exists)

## Deployment Steps

1. Clone this repository:
   ```bash
   git clone <repository-url>
   cd AMAFork07
   ```

2. Login to Azure:
   ```bash
   az login
   az account set --subscription <your-subscription-id>
   ```

3. Run the deployment script:
   ```bash
   ./deploy.sh
   ```

## Accessing Your Application

After deployment completes, navigate to:
```
https://<app-service-name>.azurewebsites.net/Index
```

**Important:** Make sure to append `/Index` to the URL, not just the root URL.

## Manual Deployment (Alternative)

If you prefer to deploy components individually:

### 1. Create Resource Group
```bash
az group create --name rg-expense-mgmt-dev --location uksouth
```

### 2. Deploy App Service
```bash
az deployment group create \
  --resource-group rg-expense-mgmt-dev \
  --template-file infrastructure/main.bicep
```

### 3. Deploy GenAI Resources
```bash
az deployment group create \
  --resource-group rg-expense-mgmt-dev \
  --template-file infrastructure/genai.bicep
```

### 4. Deploy Application Code
```bash
az webapp deploy \
  --resource-group rg-expense-mgmt-dev \
  --name <app-service-name> \
  --src-path ./app.zip
```

## Configuration

### Database Connection
The application connects to an existing Azure SQL Database. Update the connection string in App Service configuration:

```bash
az webapp config connection-string set \
  --resource-group rg-expense-mgmt-dev \
  --name <app-service-name> \
  --settings ExpenseDB="Server=tcp:sql-expense-mgmt-xyz.database.windows.net,1433;Initial Catalog=ExpenseManagementDB;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";" \
  --connection-string-type SQLAzure
```

### GenAI Settings
GenAI configuration settings are stored in `GenAISettings.json` and deployed with the application.

## Troubleshooting

- **Deployment fails**: Ensure you're logged into Azure CLI and have selected the correct subscription
- **App doesn't load**: Check that you're navigating to `/Index` endpoint
- **Database connection issues**: Verify your managed identity has access to the SQL database
- **GenAI features not working**: Ensure Azure OpenAI service has completed deployment and models are deployed

## Clean Up

To remove all deployed resources:

```bash
az group delete --name rg-expense-mgmt-dev --yes --no-wait
```
