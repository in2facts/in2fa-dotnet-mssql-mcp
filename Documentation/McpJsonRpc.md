# MCP JSON-RPC 2.0 Controller

This document describes the JSON-RPC 2.0 endpoints for the MCP (Model Context Protocol) server.

## Overview

The MCP controller provides JSON-RPC 2.0 endpoints for interacting with SQL Server databases through the Model Context Protocol. These endpoints allow language models to discover and invoke tools for database operations.

The SQL Server MCP server exposes **17 MCP tools** through a single HTTP endpoint using JSON-RPC 2.0 protocol. All operations are performed via HTTP POST requests.

**Base URL**: `http://localhost:3001/mcp`  
**Protocol**: JSON-RPC 2.0 over HTTP POST  
**Authentication**: Bearer token or X-API-Key header (optional)

## Authentication

The server supports two authentication methods:

### Bearer Token Authentication

```http
Authorization: Bearer <your-api-key>
```

### X-API-Key Header Authentication

```http
X-API-Key: <your-api-key>
```

**Configuration**:

- Environment variable: `MSSQL_MCP_API_KEY`
- Configuration file: `appsettings.json` under `ApiSecurity.ApiKey`
- Authentication is optional if no API key is configured

## Endpoints

### JSON-RPC 2.0

All JSON-RPC requests should be sent as POST requests to the base URL.

### Capabilities

You can retrieve MCP capabilities by sending a GET request to `/mcp/capabilities`.

## Methods

### tools/list

Lists all available tools that can be used with the MCP server.

**Request:**

```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "method": "tools/list",
  "params": {
    "cursor": "optional-cursor-value"
  }
}
```

**Response:**

```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "result": {
    "tools": [
      {
        "name": "ExecuteQuery",
        "title": "ExecuteQuery",
        "description": "Executes a SQL query and returns the results as JSON.",
        "inputSchema": {
          "type": "object",
          "properties": {
            "connectionName": {
              "type": "string",
              "default": "DefaultConnection"
            },
            "query": {
              "type": "string"
            }
          },
          "required": ["query"]
        }
      }
    ],
    "nextCursor": "next-page-cursor"
  }
}
```

### tools/call

Calls a specific tool with the provided arguments. This is the method used by Copilot Agent and other MCP clients.

**Request:**

```json
{
  "jsonrpc": "2.0",
  "id": 2,
  "method": "tools/call",
  "params": {
    "name": "ExecuteQuery",
    "arguments": {
      "connectionName": "DefaultConnection",
      "query": "SELECT * FROM Users"
    }
  }
}
```

**Response:**

```json
{
  "jsonrpc": "2.0",
  "id": 2,
  "result": {
    "content": [
      {
        "type": "text",
        "text": "{\"columns\":[\"Id\",\"Name\",\"Email\"],\"rows\":[[1,\"John Doe\",\"john@example.com\"],[2,\"Jane Smith\",\"jane@example.com\"]]}"
      }
    ],
    "structuredContent": {
      "columns": ["Id", "Name", "Email"],
      "rows": [
        [1, "John Doe", "john@example.com"],
        [2, "Jane Smith", "jane@example.com"]
      ]
    },
    "isError": false
  }
}
```

### Direct Tool Invocation

Tools can also be invoked directly using the tool name as the method. This is the standard JSON-RPC format.

Below are examples for each of the 17 supported tools:

**Example 1 (Initialize):**

```json
{
  "jsonrpc": "2.0",
  "id": 3,
  "method": "Initialize",
  "params": {
    "connectionName": "DefaultConnection"
  }
}
```

**Example 2 (ExecuteQuery):**

```json
{
  "jsonrpc": "2.0",
  "id": 4,
  "method": "ExecuteQuery",
  "params": {
    "query": "SELECT TOP 10 * FROM sys.databases",
    "connectionName": "DefaultConnection"
  }
}
```

**Example 3 (GetTableMetadata):**

```json
{
  "jsonrpc": "2.0",
  "id": 5,
  "method": "GetTableMetadata",
  "params": {
    "connectionName": "DefaultConnection",
    "schema": "dbo"
  }
}
```

**Example 4 (ListConnections):**

```json
{
  "jsonrpc": "2.0",
  "id": 6,
  "method": "ListConnections",
  "params": {}
}
```

**Example 5 (AddConnection):**

```json
{
  "jsonrpc": "2.0",
  "id": 7,
  "method": "AddConnection",
  "params": {
    "request": {
      "name": "NewConnection",
      "connectionString": "Server=myserver;Database=mydb;User Id=myuser;Password=mypassword;",
      "description": "My new database connection"
    }
  }
}
```

**Example 6 (TestConnection):**

```json
{
  "jsonrpc": "2.0",
  "id": 8,
  "method": "TestConnection",
  "params": {
    "request": {
      "connectionString": "Server=myserver;Database=mydb;User Id=myuser;Password=mypassword;"
    }
  }
}
```

**Example 7 (RemoveConnection):**

```json
{
  "jsonrpc": "2.0",
  "id": 9,
  "method": "RemoveConnection",
  "params": {
    "request": {
      "name": "ConnectionToRemove"
    }
  }
}
```

**Example 8 (UpdateConnection):**

```json
{
  "jsonrpc": "2.0",
  "id": 10,
  "method": "UpdateConnection",
  "params": {
    "request": {
      "name": "ExistingConnection",
      "connectionString": "Server=newserver;Database=newdb;User Id=newuser;Password=newpassword;",
      "description": "Updated connection"
    }
  }
}
```

**Example 9 (GetDatabaseObjectsMetadata):**

```json
{
  "jsonrpc": "2.0",
  "id": 11,
  "method": "tools/call",
  "params": {
    "name": "GetDatabaseObjectsMetadata",
    "arguments": {
      "connectionName": "myDB_ConnectionName",
      "schema": "dbo",
      "includeViews": true
    }
  }
}
```

**Example 10 (GetDatabaseObjectsByType):**

```json
{
  "jsonrpc": "2.0",
  "id": 12,
  "method": "GetDatabaseObjectsByType",
  "params": {
    "connectionName": "DefaultConnection",
    "schema": "dbo",
    "objectType": "TABLE"
  }
}
```

**Example 11 (GetSqlServerAgentJobs):**

```json
{
  "jsonrpc": "2.0",
  "id": 13,
  "method": "GetSqlServerAgentJobs",
  "params": {
    "connectionName": "DefaultConnection"
  }
}
```

**Example 12 (GetSqlServerAgentJobDetails):**

```json
{
  "jsonrpc": "2.0",
  "id": 14,
  "method": "GetSqlServerAgentJobDetails",
  "params": {
    "connectionName": "DefaultConnection",
    "jobName": "DatabaseBackup"
  }
}
```

**Example 13 (GetSsisCatalogInfo):**

```json
{
  "jsonrpc": "2.0",
  "id": 15,
  "method": "GetSsisCatalogInfo",
  "params": {
    "connectionName": "DefaultConnection"
  }
}
```

**Example 14 (GetAzureDevOpsInfo):**

```json
{
  "jsonrpc": "2.0",
  "id": 16,
  "method": "GetAzureDevOpsInfo",
  "params": {
    "connectionName": "DefaultConnection"
  }
}
```

**Example 15 (GenerateSecureKey):**

```json
{
  "jsonrpc": "2.0",
  "id": 17,
  "method": "GenerateSecureKey",
  "params": {
    "length": 32
  }
}
```

**Example 16 (RotateKey):**

```json
{
  "jsonrpc": "2.0",
  "id": 18,
  "method": "RotateKey",
  "params": {
    "newKey": "Uew8Ap2aiZoh5Wae/XiaNX2PVHXpnC6kPVX0Tcow4FA="
  }
}
```

**Example 17 (MigrateConnectionsToEncrypted):**

```json
{
  "jsonrpc": "2.0",
  "id": 19,
  "method": "MigrateConnectionsToEncrypted",
  "params": {}
}
```

**Example 18 (CreateApiKey):**

```json
{
  "jsonrpc": "2.0",
  "id": 20,
  "method": "CreateApiKey",
  "params": {
    "request": {
      "name": "Application API Key",
      "userId": "app-12345",
      "keyType": "application",
      "expirationDate": "2026-01-01T00:00:00Z"
    }
  }
}
```

**Example 19 (ListApiKeys):**

```json
{
  "jsonrpc": "2.0",
  "id": 21,
  "method": "ListUserApiKeys",
  "params": {
    "userId": "app-12345"
  }
}
```

**Example 20 (RevokeApiKey):**

```json
{
  "jsonrpc": "2.0",
  "id": 22,
  "method": "RevokeApiKey",
  "params": {
    "request": {
      "id": "7f8d9e0a-1b2c-3d4e-5f6g-7h8i9j0k1l2m"
    }
  }
}
```

**Example 21 (DeleteApiKey):**

```json
{
  "jsonrpc": "2.0",
  "id": 23,
  "method": "DeleteApiKey",
  "params": {
    "id": "7f8d9e0a-1b2c-3d4e-5f6g-7h8i9j0k1l2m"
  }
}
```

**Example 22 (GetApiKeyUsageLogs):**

```json
{
  "jsonrpc": "2.0",
  "id": 24,
  "method": "GetApiKeyUsageLogs",
  "params": {
    "apiKeyId": "7f8d9e0a-1b2c-3d4e-5f6g-7h8i9j0k1l2m",
    "limit": 50
  }
}
```

## Notifications

### tools/list_changed

When tools change, the server sends a notification to inform clients to refresh their tool list.

**Notification:**

```json
{
  "jsonrpc": "2.0",
  "method": "notifications/tools/list_changed"
}
```

## Response Formats

### Standard JSON-RPC Response

For direct tool invocation, the response format is:

```json
{
  "jsonrpc": "2.0",
  "id": <request-id>,
  "result": {
    // Tool-specific result data
  }
}
```

### Copilot Agent Response Format

When using the `tools/call` method, the response format is structured to provide both human-readable and machine-processable data:

```json
{
  "jsonrpc": "2.0",
  "id": <request-id>,
  "result": {
    "content": [
      {
        "type": "text",
        "text": "Human-readable result description"
      }
    ],
    "structuredContent": {
      // Structured data result that can be deserialized
    },
    "isError": false
  }
}
```

**Example (GetTableMetadata Response):**

```json
{
  "jsonrpc": "2.0",
  "id": "copilot-request-1",
  "result": {
    "content": [
      {
        "type": "text",
        "text": "Table metadata retrieved successfully"
      }
    ],
    "structuredContent": {
      "tables": [
        {
          "name": "Users",
          "schema": "dbo",
          "columns": [
            {
              "name": "UserId",
              "dataType": "int",
              "isNullable": false,
              "isPrimaryKey": true
            },
            { "name": "Username", "dataType": "nvarchar", "isNullable": false },
            { "name": "Email", "dataType": "nvarchar", "isNullable": false }
          ]
        }
      ]
    },
    "isError": false
  }
}
```

## Error Handling

### HTTP Status Codes

- **200**: Success (JSON-RPC response)
- **401**: Unauthorized (missing or malformed authentication)
- **403**: Forbidden (invalid authentication credentials)
- **406**: Not Acceptable (invalid content type)

### Protocol Errors

Standard JSON-RPC 2.0 error codes are used for protocol errors:

```json
{
  "jsonrpc": "2.0",
  "id": 3,
  "error": {
    "code": -32602,
    "message": "Unknown tool: invalid_tool_name"
  }
}
```

### Common JSON-RPC Error Codes

1. **Invalid Request (-32600)**: Request is not a valid JSON-RPC 2.0 request
2. **Method Not Found (-32601)**: Method does not exist
3. **Invalid Parameters (-32602)**: Invalid parameters provided
4. **Internal Error (-32603)**: Server-side error occurred
5. **Parse Error (-32700)**: Invalid JSON received

### Tool Execution Errors

When a tool execution fails using the `tools/call` method, the error is included in the response with `isError: true`:

```json
{
  "jsonrpc": "2.0",
  "id": 4,
  "result": {
    "content": [
      {
        "type": "text",
        "text": "Failed to execute tool ExecuteQuery: Invalid SQL syntax"
      }
    ],
    "isError": true
  }
}
```

### Common Error Scenarios

1. **Authentication Errors**: Invalid or missing API key
2. **Connection Errors**: Database unreachable or invalid connection string
3. **Validation Errors**: Invalid input parameters
4. **Database Errors**: SQL execution failures or permission issues
5. **Timeout Errors**: Long-running operations that exceed timeout limits

## Security Considerations

- All API calls should be authenticated using the API key authentication
- Tool inputs are validated before execution
- Rate limiting is applied to prevent abuse
- All database interactions are sanitized to prevent SQL injection
- Connection strings are encrypted at rest
- Encryption keys can be rotated periodically
- Input validation is performed on all parameters
- SQL Server connections use the principle of least privilege

## Usage Examples

### Copilot Agent Example

When GitHub Copilot uses the MCP server, it sends requests to the server via the JSON-RPC format with the `tools/call` method. Here's an example of how Copilot would retrieve table metadata:

```json
// Copilot Agent request to get table metadata
{
  "jsonrpc": "2.0",
  "id": "copilot-request-1",
  "method": "tools/call",
  "params": {
    "name": "GetTableMetadata",
    "arguments": {
      "connectionName": "p330d_PROTO",
      "schema": null
    }
  }
}
```

### PowerShell Example

```powershell
# Set API key
$apiKey = "your-api-key-here"
$headers = @{
    "Authorization" = "Bearer $apiKey"
    "Content-Type" = "application/json"
}

# Execute query
$body = @{
    jsonrpc = "2.0"
    id = 1
    method = "ExecuteQuery"
    params = @{
        query = "SELECT name FROM sys.databases"
        connectionName = "DefaultConnection"
    }
} | ConvertTo-Json -Depth 3

$response = Invoke-RestMethod -Uri "http://localhost:3001" -Method Post -Headers $headers -Body $body
```

### curl Example

```bash
# Using Bearer token
curl -X POST http://localhost:3001 \
  -H "Authorization: Bearer your-api-key-here" \
  -H "Content-Type: application/json" \
  -d '{
    "jsonrpc": "2.0",
    "id": 1,
    "method": "tools/call",
    "params": {
      "name": "GetTableMetadata",
      "arguments": {
        "connectionName": "DefaultConnection",
        "schema": "dbo"
      }
    }
  }'
```

### JavaScript Example

```javascript
async function callMcpApi(toolName, arguments = {}) {
  const response = await fetch("http://localhost:3001", {
    method: "POST",
    headers: {
      Authorization: "Bearer your-api-key-here",
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      jsonrpc: "2.0",
      id: Date.now(),
      method: "tools/call",
      params: {
        name: toolName,
        arguments: arguments,
      },
    }),
  });

  return response.json();
}

// Usage
const result = await callMcpApi("GetDatabaseObjectsMetadata", {
  connectionName: "DefaultConnection",
  includeViews: true,
});
```
