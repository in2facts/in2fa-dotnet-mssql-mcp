# Multi-Key API Authentication System

This document explains the multi-key authentication system implemented in mssqlMCP.

## Overview

The multi-key authentication system allows the mssqlMCP server to authenticate requests using both a master API key and multiple user-specific API keys stored in a SQLite database. This provides more robust security, allows for API key rotation, and enables fine-grained access control for different clients or integrations.

## Architecture

![Multi-Key Authentication Architecture](architecture_diagram.svg)

### Components

1. **Master API Key**:

   - Environment variable: `MSSQL_MCP_API_KEY`
   - Allows full administrative access, including API key management

2. **Encryption Master Key**:

   - Environment variable: `MSSQL_MCP_KEY`
   - Used to encrypt API keys stored in the SQLite database

3. **SQLite Database**:

   - Stores encrypted API keys and usage logs
   - Located in the same directory as the connection database

4. **ApiKeyAuthMiddleware**:

   - Validates requests against both the master API key and stored keys
   - Logs API key usage

5. **API Key Manager**:
   - Handles API key creation, validation, and revocation
   - Records usage statistics and enforces security policies

## API Keys Structure

Each API key includes the following attributes:

- **Id**: Unique identifier
- **Name**: Descriptive name for the key
- **Key**: The encrypted API key value
- **UserId**: User or service associated with the key
- **CreatedAt**: Creation timestamp
- **ExpirationDate** (optional): Date when the key becomes invalid
- **LastUsed**: Timestamp of last use
- **IsActive**: Whether the key is currently active
- **KeyType**: Type of key (user, service, admin, etc.)
- **Description**: Optional description of the key's purpose

## Authentication Flow

1. Client includes the API key in the request using one of two methods:

   - `Authorization: Bearer <api-key>` header
   - `X-API-Key: <api-key>` header

2. ApiKeyAuthMiddleware intercepts the request and:

   - Compares the provided key against the master key
   - If not a match, checks against the stored keys in the SQLite database
   - Validates that the key is active and not expired
   - Logs the API usage for monitoring

3. If authentication succeeds, the request is processed; otherwise, a 401 or 403 error is returned.

## Management Endpoints

The following MCP endpoints are available for API key management:

- **CreateApiKey**: Create a new API key for a user or service
- **ListUserApiKeys**: List API keys for a specific user
- **ListAllApiKeys**: List all API keys in the system (admin only)
- **RevokeApiKey**: Revoke an API key (mark as inactive)
- **DeleteApiKey**: Permanently delete an API key
- **GetApiKeyUsageLogs**: Get usage logs for a specific API key
- **GetUserUsageLogs**: Get API usage logs for a specific user

These endpoints follow the same JSON-RPC 2.0 format as other MCP endpoints. See the API_ENDPOINTS.md document for detailed usage examples.

## Security Considerations

1. **Master Key Protection**: The master API key should be stored securely and rotated periodically.

2. **Encryption**: All API keys are encrypted at rest in the SQLite database using the master encryption key.

3. **Key Expiration**: API keys can be configured with expiration dates for automatic invalidation.

4. **Usage Logging**: All API key usage is logged for audit and security monitoring.

5. **Revocation**: API keys can be immediately revoked if compromised.

## Adding New API Keys

To create a new API key with the master key, use the CreateApiKey endpoint:

```http
POST http://localhost:3001/mcp
Content-Type: application/json
Authorization: Bearer <master-api-key>
```

```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "method": "tools/call",
  "params": {
    "name": "CreateApiKey",
    "arguments": {
      "name": "My Test API Key",
      "userId": "SomeUserId",
      "keyType": "user",
      "expirationDate": "2026-06-29T00:00:00Z"
    }
  }
}
```

The response will include the API key value, which should be stored securely, as it cannot be retrieved later.
