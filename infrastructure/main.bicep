// Main Bicep file for Expense Management System
// Deploys App Service with low-cost development SKU in UK South

@description('Name of the resource group')
param resourceGroupName string = 'rg-expense-mgmt-dev'

@description('Location for all resources')
param location string = 'uksouth'

@description('Name of the App Service')
param appServiceName string = 'app-expense-mgmt-${uniqueString(resourceGroup().id)}'

@description('Name of the App Service Plan')
param appServicePlanName string = 'asp-expense-mgmt-${uniqueString(resourceGroup().id)}'

@description('SKU for App Service Plan (low-cost development)')
param appServicePlanSku string = 'F1'

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: appServicePlanSku
    tier: 'Free'
    size: 'F1'
    family: 'F'
    capacity: 1
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

// App Service
resource appService 'Microsoft.Web/sites@2022-09-01' = {
  name: appServiceName
  location: location
  kind: 'app,linux'
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      alwaysOn: false
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      http20Enabled: true
      appSettings: [
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
      ]
    }
  }
}

// Outputs
output appServiceName string = appService.name
output appServiceUrl string = 'https://${appService.properties.defaultHostName}'
output appServicePlanName string = appServicePlan.name
