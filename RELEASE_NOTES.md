# mssqlMCP Release Notes

## Version 2.0.0 - Multi-Key API Release

**Release Date:** June 29, 2025

---

## Summary of Changes

This release introduces significant security and connectivity enhancements to the mssqlMCP (SQL Server Model Context Protocol) server. The major focus has been on implementing a robust multi-key API authentication system and adding stdio communication options for enhanced integration with VS Code Copilot Agent.

---

## New Features

### 🔐 Multi-Key API Authentication System

- **Master API Key Support**: Implemented a hierarchical API key system with master keys for administrative access
- **User-Specific API Keys**: Added support for creating and managing user-specific API keys with granular permissions
- **API Key Lifecycle Management**: Complete CRUD operations for API key management including creation, rotation, and revocation
- **Usage Analytics**: Built-in tracking and analytics for API key usage patterns and access logs

### 🔌 stdio Communication Option

- **stdio Protocol Support**: Added stdio communication protocol alongside existing HTTP endpoints
- **VS Code Integration**: Enhanced integration with VS Code Copilot Agent through stdio interface
- **Dual Protocol Support**: Server now supports both HTTP and stdio protocols simultaneously

### 🛡️ Enhanced Security Features

- **AES-256 Connection String Encryption**: All database connection strings are now encrypted using AES-256
- **Key Rotation Capability**: Secure key rotation mechanisms for both API keys and encryption keys
- **Connection Security Assessment**: Tools to assess and validate connection security configurations

---

## Enhancements

### 📊 Database Metadata Improvements

- **Enhanced Schema Information**: More detailed schema metadata including primary/foreign key relationships
- **Azure DevOps Integration**: Added support for Azure DevOps project, repository, and work item metadata
- **SQL Server Agent Jobs**: Comprehensive SQL Server Agent job information and execution history
- **SSIS Catalog Support**: Added support for SQL Server Integration Services catalog metadata

### 🏗️ Architecture Improvements

- **Clean Architecture**: Implemented separation of concerns with proper dependency injection
- **Async/Await Pattern**: All database operations now use async/await for better performance
- **Strongly-Typed Models**: Enhanced type safety with comprehensive model definitions
- **Robust Logging**: Integrated Serilog for comprehensive logging and diagnostics

### 📚 Documentation Updates

- **Comprehensive API Documentation**: Updated documentation covering all new features
- **Security Guidelines**: Added detailed security configuration and best practices
- **Integration Examples**: PowerShell and C# examples for various integration scenarios
- **Architecture Documentation**: Detailed architectural diagrams and process flows

---

## Technical Improvements

### 🔧 Configuration Management

- **Environment Variable Support**: Enhanced configuration through environment variables
- **Secure Configuration Storage**: Encrypted storage of sensitive configuration data
- **Connection String Provider**: Centralized connection string management with encryption

### 🧪 Testing and Quality

- **Unit Test Coverage**: Added comprehensive unit tests using xUnit and Moq
- **Integration Testing**: PowerShell-based integration testing scripts
- **Security Testing**: Automated security assessment tools and scripts

---

## Breaking Changes

⚠️ **API Authentication Required**: All API endpoints now require authentication via API keys. Existing integrations will need to be updated with proper API key configuration.

⚠️ **Connection String Format**: Database connection strings are now stored in encrypted format. Existing connections will need to be migrated using the provided migration scripts.

---

## Installation/Upgrade Instructions

### New Installation

1. **Prerequisites**: Ensure .NET 9.0 SDK is installed
2. **Clone Repository**: `git clone [repository-url]`
3. **Environment Setup**: Configure required environment variables:
   - `MSSQL_MCP_KEY`: Master encryption key
   - `MSSQL_MCP_API_KEY`: Master API key
   - `MSSQL_MCP_DATA`: Data directory path (optional)
4. **Build and Run**: `dotnet run --project mssqlMCP.csproj`

### Upgrade from Previous Version

1. **Backup Data**: Backup existing `connections.json` and any custom configurations
2. **Update Code**: Pull latest changes from repository
3. **Run Migration**: Execute `Scripts/Migrate-To-Encrypted.ps1` to migrate existing connections
4. **Configure API Keys**: Use `Scripts/Set-Api-Key.ps1` to set up API authentication
5. **Update Integrations**: Update client applications to use new API key authentication

---

## Known Issues

- **First-time Setup**: Initial encryption key generation may take a few seconds on first startup
- **Large Result Sets**: Very large query results may impact performance; consider implementing pagination for large datasets
- **Connection Pooling**: Connection pooling behavior may vary under high concurrent load scenarios

---

## Security Considerations

- **API Key Storage**: Store API keys securely and rotate them regularly
- **Network Security**: Use HTTPS for all HTTP-based communications
- **Database Permissions**: Follow principle of least privilege for database user accounts
- **Encryption Keys**: Protect encryption keys and implement proper key rotation policies

---

## Support and Documentation

- **Full Documentation**: See `Documentation/` folder for comprehensive guides
- **API Reference**: `Documentation/API_ENDPOINTS.md`
- **Security Guide**: `Documentation/Security.md`
- **Integration Examples**: `Examples/` folder contains sample implementations

---

## Contributors

- **7045kHz** - Lead Developer and Architect

---

_For technical support or questions, please refer to the project documentation or create an issue in the project repository._
