# Using MCP JSON-RPC with C#

This document provides examples of how to use the SQL Server MCP JSON-RPC endpoints from C# applications.

## Overview

The Model Context Protocol (MCP) server provides JSON-RPC 2.0 endpoints that allow you to interact with SQL Server databases. This guide shows how to use these endpoints from C# applications.

> **Note**: All JSON-RPC requests are sent directly to the base URL (e.g., `http://localhost:3001/mcp`) via HTTP POST. The server processes these requests according to the JSON-RPC 2.0 specification.

## Setting up the Client

First, create an HTTP client that can communicate with the MCP server:

```csharp
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

public class McpClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public McpClient(string baseUrl, string apiKey)
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
    }

    // Client methods will go here
}
```

## Calling MCP Tools

### Listing Available Tools

To list all available tools:

```csharp
public async Task<JsonElement> ListToolsAsync()
{
    var requestBody = new
    {
        jsonrpc = "2.0",
        id = Guid.NewGuid().ToString(),
        method = "tools/list"
    };

    var response = await _httpClient.PostAsJsonAsync(_baseUrl, requestBody);
    response.EnsureSuccessStatusCode();

    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
    return jsonResponse.GetProperty("result");
}
```

### Calling a Tool

To call a specific tool, you can use either the standard JSON-RPC format or the Copilot Agent format:

#### Standard JSON-RPC Format

```csharp
public async Task<T> CallToolStandardAsync<T>(string toolName, object arguments)
{
    var requestId = Guid.NewGuid().ToString();

    var requestBody = new
    {
        jsonrpc = "2.0",
        id = requestId,
        method = toolName,
        @params = arguments
    };

    var response = await _httpClient.PostAsJsonAsync(_baseUrl, requestBody);
    response.EnsureSuccessStatusCode();

    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

    // Process response...
    // (error handling code omitted for brevity)

    return JsonSerializer.Deserialize<T>(jsonResponse.GetProperty("result").GetRawText());
}
```

#### Copilot Agent Format

```csharp
public async Task<T> CallToolAsync<T>(string toolName, object arguments)
{
    var requestId = Guid.NewGuid().ToString();

    var requestBody = new
    {
        jsonrpc = "2.0",
        id = requestId,
        method = "tools/call",
        @params = new
        {
            name = toolName,
            arguments = arguments
        }
    };

    var response = await _httpClient.PostAsJsonAsync(_baseUrl, requestBody);
    response.EnsureSuccessStatusCode();

    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

    // Check for errors
    if (jsonResponse.TryGetProperty("error", out var errorElement))
    {
        var errorMessage = errorElement.GetProperty("message").GetString();
        throw new Exception($"Tool execution failed: {errorMessage}");
    }

    // Extract result
    var resultElement = jsonResponse.GetProperty("result");

    if (resultElement.TryGetProperty("isError", out var isErrorElement) && isErrorElement.GetBoolean())
    {
        var errorContent = resultElement.GetProperty("content")[0].GetProperty("text").GetString();
        throw new Exception($"Tool execution error: {errorContent}");
    }

    if (resultElement.TryGetProperty("structuredContent", out var structuredContent))
    {
        return JsonSerializer.Deserialize<T>(structuredContent.GetRawText());
    }

    return JsonSerializer.Deserialize<T>(resultElement.GetProperty("content")[0].GetProperty("text").GetString());
}
```

## Response Formats

### Standard JSON-RPC Response

For standard JSON-RPC calls, the response format is:

```json
{
  "jsonrpc": "2.0",
  "id": "request-id",
  "result": {
    // Tool-specific result data
  }
}
```

### Copilot Agent Response Format

When using the Copilot Agent format with `tools/call`, the response format is:

```json
{
  "jsonrpc": "2.0",
  "id": "request-id",
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

The C# client code above handles both response formats appropriately.

## Example Usage

### Using Standard JSON-RPC Format

```csharp
public async Task<string> InitializeStandardAsync(string connectionName = "DefaultConnection")
{
    return await CallToolStandardAsync<string>("Initialize", new { connectionName });
}

public async Task<JsonElement> ExecuteQueryStandardAsync(string query, string connectionName = "DefaultConnection")
{
    return await CallToolStandardAsync<JsonElement>("ExecuteQuery", new { query, connectionName });
}
```

### Using Copilot Agent Format

```csharp
public async Task<string> InitializeAsync(string connectionName = "DefaultConnection")
{
    return await CallToolAsync<string>("Initialize", new { connectionName });
}

public async Task<JsonElement> ExecuteQueryAsync(string query, string connectionName = "DefaultConnection")
{
    return await CallToolAsync<JsonElement>("ExecuteQuery", new { query, connectionName });
}

public async Task<JsonElement> GetTableMetadataAsync(string connectionName = "DefaultConnection", string schema = null)
{
    return await CallToolAsync<JsonElement>("GetTableMetadata", new { connectionName, schema });
}

public async Task<JsonElement> GetDatabaseObjectsMetadataAsync(
    string connectionName = "DefaultConnection",
    string schema = null,
    bool includeViews = true)
{
    return await CallToolAsync<JsonElement>("GetDatabaseObjectsMetadata",
        new { connectionName, schema, includeViews });
}
```

## Complete Example

A complete example application is available in the `Examples` directory:

- `McpClientExample.cs`: A reusable client class for MCP JSON-RPC
- `McpConsoleExample.cs`: A console application demonstrating how to use the client

To run the console example:

```bash
cd mssqlMCP/Examples
dotnet run McpConsoleExample.cs
```

## Error Handling

The client handles two types of errors:

1. **Protocol errors**: These are standard JSON-RPC errors (e.g., unknown tool, invalid parameters)
2. **Tool execution errors**: These are errors that occur during tool execution (e.g., SQL syntax errors, connection failures)

Both types of errors are converted to exceptions with appropriate error messages.

## Security Best Practices

- Store API keys securely, preferably in environment variables or a secure configuration store
- Use HTTPS for production environments
- Validate and sanitize all SQL queries to prevent SQL injection
- Implement proper error handling to avoid leaking sensitive information

## References

- [Model Context Protocol Specification](https://modelcontextprotocol.io/specification/2025-06-18/server/tools)
- [SQL Server MCP Documentation](../README.md)
- [MCP JSON-RPC API Documentation](./McpJsonRpc.md)
