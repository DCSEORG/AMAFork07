// Bicep file for Azure OpenAI and Cognitive Search resources
// For GenAI chat functionality with RAG pattern

@description('Location for all resources')
param location string = 'uksouth'

@description('Name of the Azure OpenAI service')
param openAIServiceName string = 'openai-expense-${uniqueString(resourceGroup().id)}'

@description('Name of the Azure Cognitive Search service')
param searchServiceName string = 'search-expense-${uniqueString(resourceGroup().id)}'

@description('Azure OpenAI SKU (low-cost development)')
param openAISku string = 'S0'

@description('Azure Cognitive Search SKU (low-cost development)')
param searchSku string = 'free'

// Azure OpenAI Service
resource openAIService 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
  name: openAIServiceName
  location: location
  sku: {
    name: openAISku
  }
  kind: 'OpenAI'
  properties: {
    customSubDomainName: openAIServiceName
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      defaultAction: 'Allow'
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

// Azure OpenAI GPT-4 Deployment
resource gpt4Deployment 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = {
  parent: openAIService
  name: 'gpt-4'
  sku: {
    name: 'Standard'
    capacity: 10
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-4'
      version: '0613'
    }
  }
}

// Azure Cognitive Search for RAG
resource searchService 'Microsoft.Search/searchServices@2023-11-01' = {
  name: searchServiceName
  location: location
  sku: {
    name: searchSku
  }
  properties: {
    replicaCount: 1
    partitionCount: 1
    hostingMode: 'default'
    publicNetworkAccess: 'enabled'
  }
  identity: {
    type: 'SystemAssigned'
  }
}

// Outputs
output openAIServiceName string = openAIService.name
output openAIEndpoint string = openAIService.properties.endpoint
output openAIId string = openAIService.id
output searchServiceName string = searchService.name
output searchServiceEndpoint string = 'https://${searchService.name}.search.windows.net'
output searchServiceId string = searchService.id
