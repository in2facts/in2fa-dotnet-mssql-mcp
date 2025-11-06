# Merge Update Documentation - November 2025

## Overview

This document summarizes the significant changes introduced through the recent merge from the `api-key-auth` branch (PR #19) into the main codebase on November 5, 2025. The merge brings substantial security enhancements and bug fixes to the mssqlMCP project.

## Major Changes Summary

### 🔐 Security Enhancements

#### 1. **Enhanced API Key Authorization System**

The merge introduces a comprehensive API key-based authorization system with the following features:

- **Multi-tier API Key Support**: Support for both master keys and user-specific API keys
- **Role-based Access Control**: Different access levels for admin and user keys
- **Connection-level Restrictions**: API keys can be restricted to specific database connections
- **Encrypted Key Storage**: User API keys are stored encrypted in SQLite database

#### 2. **New Authentication Middleware**

Enhanced `ApiKeyAuthMiddleware` with:

- Support for Bearer token and X-API-Key header authentication
- Case-insensitive JSON property handling
- Connection name validation for restricted API keys
- Comprehensive authorization checks for different key types

#### 3. **Comprehensive Test Coverage**

Added extensive unit tests (`ApiKeyAuthMiddlewareTests.cs`) covering:

- User key access to allowed/forbidden endpoints
- Admin key privileges
- Master key unrestricted access
- Connection-based restrictions
- Various authentication scenarios

### 🛠️ Technical Improvements

#### 1. **JsonHelper Utility**

New `JsonHelper.cs` class providing:

- Case-insensitive JSON property access
- Support for nested property paths
- Improved JSON handling for API requests

#### 2. **Updated Documentation**

- Enhanced `API_ENDPOINTS.md` with new authentication examples
- Updated `tools_list.json` with current tool definitions
- Comprehensive security documentation updates

#### 3. **License Addition**

Added proper MIT license file to the project

### 🔧 Bug Fixes

#### 1. **API Key Authorization Bug Fix**

- Fixed case sensitivity issues in API key handling
- Improved error handling in authentication middleware
- Better validation of connection names in requests

#### 2. **Development Environment Improvements**

- Updated `.gitignore` with JetBrains Rider support
- Better build configuration management

## Detailed Changes by Component

### Authentication & Authorization

#### Enhanced Middleware (`ApiKeyAuthMiddleware.cs`)

```csharp
// Key features:
- Support for master key from environment variables
- User API key validation through IApiKeyManager
- Role-based endpoint access control
- Connection-level authorization
```

#### API Key Models (`ApiKeyModels.cs`)

- Added `AllowedConnectionNames` field for connection restrictions
- Enhanced API key response models
- Improved validation attributes

#### API Key Repository (`ApiKeyRepository.cs`)

- Added support for `AllowedConnectionNames` field in database schema
- Enhanced encryption/decryption for stored API keys
- Improved query methods for key management

### Security Features

#### Multi-Key Authentication

The system now supports three types of authentication:

1. **Master Key**: Environment variable `MSSQL_MCP_API_KEY`

   - Full system access
   - Can manage other API keys
   - Unrestricted endpoint access

2. **Admin Keys**: Database-stored with "admin" role

   - Can access management endpoints
   - Can create/manage other API keys
   - Full database access

3. **User Keys**: Database-stored with "user" role
   - Limited to data access endpoints
   - Can be restricted to specific connections
   - Cannot access management functions

#### Connection-Level Security

- API keys can be restricted to specific database connections
- Validation occurs at request time
- Prevents unauthorized access to restricted databases

### Testing Improvements

#### Comprehensive Test Suite

The new test suite covers:

- **Authentication Scenarios**: Valid/invalid keys, missing keys
- **Authorization Levels**: User, admin, and master key permissions
- **Connection Restrictions**: Allowed/disallowed connection access
- **Endpoint Security**: Management vs. data access endpoints
- **Edge Cases**: Null values, malformed requests, case sensitivity

### Documentation Updates

#### API Endpoints Documentation

- Updated authentication examples
- Added multi-key authentication guide
- Enhanced security configuration instructions
- Improved endpoint access documentation

#### Security Documentation

- Updated security guidelines
- Added new authentication methods
- Enhanced configuration examples
- Improved troubleshooting guides

## Migration Guide

### For Existing Installations

1. **Environment Variables**

   - Ensure `MSSQL_MCP_API_KEY` is set for master key access
   - Set `MSSQL_MCP_KEY` for API key encryption

2. **Database Updates**

   - The `AllowedConnectionNames` field will be automatically added to existing installations
   - Existing API keys will continue to work without connection restrictions

3. **Configuration Updates**
   - Review `appsettings.json` for new security configurations
   - Update client authentication headers as needed

### For New Installations

1. **Setup Master Key**

   ```bash
   $env:MSSQL_MCP_API_KEY = "your-master-key-here"
   $env:MSSQL_MCP_KEY = "your-encryption-key-here"
   ```

2. **Create User API Keys**
   Use the master key to create restricted user keys via API endpoints

3. **Configure Client Applications**
   Update clients to use appropriate authentication headers

## Breaking Changes

### ⚠️ Authentication Required

- If previously running without authentication, you must now configure at least a master key
- All API requests must include authentication headers

### API Changes

- Some endpoints now require admin privileges
- Connection-based restrictions may affect existing API key usage

## Security Improvements

### Enhanced Protection

- All API keys are now encrypted in storage
- Connection-level access control prevents unauthorized database access
- Role-based permissions limit endpoint access
- Comprehensive audit logging for security events

### Best Practices

- Use user keys instead of master keys for client applications
- Implement connection restrictions for enhanced security
- Regularly rotate API keys
- Monitor authentication logs for suspicious activity

## Future Considerations

### Planned Enhancements

- API key expiration dates
- Rate limiting per API key
- Enhanced audit logging
- OAuth integration options

### Recommended Actions

1. **Review Security Configuration**: Ensure proper API key setup
2. **Update Client Applications**: Implement proper authentication
3. **Monitor Logs**: Watch for authentication failures
4. **Regular Key Rotation**: Implement key rotation policies

## Conclusion

This merge significantly enhances the security posture of the mssqlMCP project while maintaining backward compatibility where possible. The new authentication system provides fine-grained access control and improved security for production deployments.

For questions or issues related to these changes, please refer to the updated documentation or create an issue in the project repository.

---

**Generated on**: November 5, 2025  
**Merge Commit**: `0ec0da00770ddc1473c559126eec658736379d8b`  
**Branch**: `documentation.2`
