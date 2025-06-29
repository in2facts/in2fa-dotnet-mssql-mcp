# Multi-Key Instructions

## Overview

- Instead of authenticating via a single key extracted from `MSSQL_MCP_API_KEY`, create a SQLite database to store multiple keys and store the database in the same directory as the connection.db file.
- The `MSSQL_MCP_KEY` environment variable should be used as a master key to encrypt the keys stored in the SQLite database.
- The X-API-Key and Authentication header should be authenticated against the `MSSQL_MCP_API_KEY` and the keys stored in the SQLite database used to authenticate requests to the MCP server.
- The `MSSQL_MCP_KEY` environment variable should be used to encrypt the keys stored in the SQLite database.
- The `MSSQL_MCP_API_KEY` is the master key that is used to create, list, update and delete the keys in the SQLite database. Only the master key can perform these operations.
- Any additional MCP endpoints should follow the same authentication pattern.
- Any endpoints created for this API authentication method should follow the same pattern as the existing MCP endpoints. This includes the same tool/call structure, error handling, and response format.
- Example tool/call structure for the MCP endpoints:
  ```json
  {
  "jsonrpc": "2.0",
  "id": <unique-identifier>,
  "method": "tools/call",
  "params": {
    "name": "<tool-name>",
    "arguments": {
      // Tool-specific parameters
    }
  }
  }
  ```
