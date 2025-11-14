# Expense Management AI Chat Interface

This folder contains a chat-based AI interface for interacting with the Expense Management System using natural language.

## Overview

The chat UI provides a conversational interface powered by Azure OpenAI that allows users to:
- Query expenses using natural language
- Create new expenses through conversation
- Submit, approve, or reject expenses
- Get summaries and reports

## Architecture

### Components

1. **Frontend (index.html)**
   - Bootstrap-based chat interface
   - JavaScript functions for API calls
   - Pattern matching for demo mode

2. **Azure OpenAI Integration**
   - Uses GPT-4 for natural language understanding
   - Function calling to execute API operations
   - Configured via GenAISettings.json in main app

3. **RAG (Retrieval-Augmented Generation)**
   - Knowledge base in `/RAG/expense-system-knowledge.md`
   - Provides context about the system to the AI
   - Indexed in Azure Cognitive Search

## Setup Instructions

### 1. Configure Azure OpenAI

Update the following in `index.html` or create a separate config:
```javascript
const AZURE_OPENAI_ENDPOINT = 'https://openai-expense-{uniqueid}.openai.azure.com/';
const AZURE_OPENAI_KEY = 'YOUR_API_KEY'; // Or use managed identity
```

### 2. Deploy RAG Content to Azure Cognitive Search

Use the Azure SDK to index the knowledge base:
```bash
# Example using Azure CLI
az search index create --name expense-knowledge --service-name search-expense-xyz
```

### 3. Configure Function Calling

The AI can call these functions:
- `get_expenses()` - Get all expenses
- `get_expenses_by_status(status)` - Filter by status
- `create_expense(userId, categoryId, amount, date, description)` - Create new
- `submit_expense(expenseId)` - Submit for approval
- `approve_expense(expenseId, reviewerId)` - Approve
- `reject_expense(expenseId, reviewerId)` - Reject
- `get_users()` - Get user list
- `get_categories()` - Get categories

## Usage Examples

**Query expenses:**
> "Show me all pending expenses"

**Create expense:**
> "Create a travel expense for £45.50 for Alice dated today"

**Approve expense:**
> "Approve expense ID 123"

**Get summary:**
> "How many expenses are awaiting approval?"

## Production Deployment

For production use:

1. **Security**
   - Use Azure Managed Identity instead of API keys
   - Implement proper authentication/authorization
   - Add rate limiting

2. **Integration**
   - Replace pattern matching with actual Azure OpenAI API calls
   - Implement proper error handling
   - Add logging and monitoring

3. **RAG Setup**
   - Index knowledge base in Azure Cognitive Search
   - Configure semantic search
   - Add document chunking strategy

4. **Hosting**
   - Host as static site in Azure Storage
   - Or integrate into main ASP.NET app
   - Use CDN for performance

## Demo Mode

The current implementation includes a demo mode that uses pattern matching for common queries. This allows testing without Azure OpenAI configured.

To enable full AI functionality:
1. Configure Azure OpenAI endpoint and key
2. Implement the `callAzureOpenAI()` function
3. Set up function calling schema
4. Deploy and test

## Files

- `index.html` - Main chat interface
- `README.md` - This documentation
- `../RAG/expense-system-knowledge.md` - Knowledge base for RAG

## Best Practices

Following Azure best practices:
- Use managed identities for authentication
- Store secrets in Azure Key Vault
- Enable diagnostic logging
- Implement retry policies
- Use content filtering for safety
