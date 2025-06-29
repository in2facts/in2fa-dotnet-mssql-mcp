# Testing MCP JSON-RPC Endpoints with PowerShell

This document explains how to use the PowerShell script provided in this repository to test all the JSON-RPC endpoints of the mssqlMCP server.

## Prerequisites

1. PowerShell 5.1 or later
2. mssqlMCP server running
3. An API key for authentication

## Setup

1. **Set the API Key as an environment variable**:

   ```powershell
   # In PowerShell
   $env:MSSQL_MCP_API_KEY = "your-api-key"
   ```

   ```cmd
   # In Command Prompt
   set MSSQL_MCP_API_KEY=your-api-key
   ```

   You can get your API key from the server administrator or generate one using the security tools.

2. **Ensure the mssqlMCP server is running**:

   You can start the server using the provided VS Code task or by running:

   ```powershell
   dotnet run --project "c:\Users\U00001\source\repos\MCP\mssqlMCP\mssqlMCP.csproj"
   ```

## Running the Test Script

The script is located at `Examples\test-mcp-jsonrpc.ps1`. To run it:

```powershell
cd "c:\Users\U00001\source\repos\MCP\mssqlMCP\Examples"
.\test-mcp-jsonrpc.ps1
```

### Command-line Options

- **Detailed output**: To see the full JSON response for each request:

  ```powershell
  .\test-mcp-jsonrpc.ps1 -Detailed
  ```

## What the Script Tests

The script tests the following MCP JSON-RPC endpoints:

1. **tools/list**: Lists all available tools
2. **tools/call**: Calls specific tools, including:
   - Connection Management:
     - ListConnections
     - TestConnection
   - SQL Server Tools:
     - Initialize
     - ExecuteQuery
   - Security Tools:
     - GenerateSecureKey
   - Database Metadata Tools:
     - GetTableMetadata
     - GetDatabaseObjectsMetadata
     - GetDatabaseObjectsByType

## Output Interpretation

The script provides a color-coded output:

- **Cyan**: Section headers
- **Green**: Successful calls
- **Red**: Failed calls
- **Yellow**: Response summaries
- **White**: Tool details and results

## Troubleshooting

### API Key Issues

If you see authentication errors:

- Verify your API key is set correctly
- Check that the API key matches the one expected by the server

### Connection Issues

If you cannot connect to the server:

- Ensure the server is running
- Check that the port in `$baseUrl` matches your server's configuration

### Tool Errors

If specific tool calls fail:

- For database tools, ensure there's at least one valid connection
- For metadata tools, make sure the specified connection exists and is valid

## Customizing the Tests

You can modify the script to test additional tools or customize the arguments for existing tool calls. The structure for calling a tool is:

```powershell
$payload = @{
    jsonrpc = "2.0"
    id = [unique_id]
    method = "tools/call"
    params = @{
        name = "mcp_mssqlmcpdloca_[ToolName]"
        arguments = @{
            # Tool-specific arguments here
        }
    }
}
```

## Related Documentation

- [MCP JSON-RPC Documentation](./McpJsonRpc.md)
- [Using MCP with C#](./UsingMcpWithCSharp.md)
