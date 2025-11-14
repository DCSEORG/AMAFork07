#!/bin/bash

# Expense Management System - Master Deployment Script
# This script deploys all Azure infrastructure and the application code
# Prerequisites: Azure CLI installed and logged in (az login)

set -e  # Exit on error

echo "======================================"
echo "Expense Management System Deployment"
echo "======================================"
echo ""

# Configuration Variables
RESOURCE_GROUP="rg-expense-mgmt-dev"
LOCATION="uksouth"
SUBSCRIPTION_ID=$(az account show --query id -o tsv)

echo "Using subscription: $SUBSCRIPTION_ID"
echo "Resource Group: $RESOURCE_GROUP"
echo "Location: $LOCATION"
echo ""

# Step 1: Create Resource Group
echo "Step 1: Creating Resource Group..."
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION \
  --output table

echo ""

# Step 2: Deploy App Service Infrastructure
echo "Step 2: Deploying App Service infrastructure..."
APP_SERVICE_DEPLOYMENT=$(az deployment group create \
  --resource-group $RESOURCE_GROUP \
  --template-file infrastructure/main.bicep \
  --parameters location=$LOCATION \
  --query properties.outputs \
  --output json)

APP_SERVICE_NAME=$(echo $APP_SERVICE_DEPLOYMENT | jq -r '.appServiceName.value')
APP_SERVICE_URL=$(echo $APP_SERVICE_DEPLOYMENT | jq -r '.appServiceUrl.value')

echo "App Service Name: $APP_SERVICE_NAME"
echo "App Service URL: $APP_SERVICE_URL"
echo ""

# Step 3: Deploy GenAI Infrastructure
echo "Step 3: Deploying GenAI infrastructure (Azure OpenAI & Cognitive Search)..."
GENAI_DEPLOYMENT=$(az deployment group create \
  --resource-group $RESOURCE_GROUP \
  --template-file infrastructure/genai.bicep \
  --parameters location=$LOCATION \
  --query properties.outputs \
  --output json)

OPENAI_SERVICE_NAME=$(echo $GENAI_DEPLOYMENT | jq -r '.openAIServiceName.value')
OPENAI_ENDPOINT=$(echo $GENAI_DEPLOYMENT | jq -r '.openAIEndpoint.value')
SEARCH_SERVICE_NAME=$(echo $GENAI_DEPLOYMENT | jq -r '.searchServiceName.value')
SEARCH_ENDPOINT=$(echo $GENAI_DEPLOYMENT | jq -r '.searchServiceEndpoint.value')

echo "Azure OpenAI Service: $OPENAI_SERVICE_NAME"
echo "Azure OpenAI Endpoint: $OPENAI_ENDPOINT"
echo "Search Service: $SEARCH_SERVICE_NAME"
echo "Search Endpoint: $SEARCH_ENDPOINT"
echo ""

# Step 4: Deploy Application Code (if app.zip exists)
if [ -f "app.zip" ]; then
  echo "Step 4: Deploying application code..."
  az webapp deploy \
    --resource-group $RESOURCE_GROUP \
    --name $APP_SERVICE_NAME \
    --src-path ./app.zip \
    --type zip
  
  echo ""
  echo "Application deployed successfully!"
else
  echo "Step 4: Skipping application deployment (app.zip not found)"
  echo "Build the application first and run this script again to deploy the code"
fi

echo ""
echo "======================================"
echo "Deployment Summary"
echo "======================================"
echo "Resource Group: $RESOURCE_GROUP"
echo "App Service: $APP_SERVICE_NAME"
echo "App URL: $APP_SERVICE_URL/Index"
echo ""
echo "IMPORTANT: Navigate to $APP_SERVICE_URL/Index (not just the root URL)"
echo ""
echo "Azure OpenAI: $OPENAI_SERVICE_NAME"
echo "Azure Cognitive Search: $SEARCH_SERVICE_NAME"
echo ""
echo "Next Steps:"
echo "1. If app.zip doesn't exist yet, build the application"
echo "2. Run this script again to deploy the application code"
echo "3. Configure the database connection in App Service settings"
echo "======================================"
