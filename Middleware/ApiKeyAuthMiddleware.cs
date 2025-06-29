using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using mssqlMCP.Services;
using System;
using System.Threading.Tasks;

namespace mssqlMCP.Middleware;

/// <summary>
/// Middleware that validates API key authentication for incoming requests
/// Supports both master key and user-specific API keys
/// </summary>
public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyAuthMiddleware> _logger;
    private readonly string _masterKey;
    private readonly IApiKeyManager _apiKeyManager;
    private readonly IApiKeyRepository _apiKeyRepository;

    public ApiKeyAuthMiddleware(
        RequestDelegate next,
        ILogger<ApiKeyAuthMiddleware> logger,
        IConfiguration configuration,
        IApiKeyManager apiKeyManager,
        IApiKeyRepository apiKeyRepository)
    {
        _next = next;
        _logger = logger;
        _apiKeyManager = apiKeyManager;
        _apiKeyRepository = apiKeyRepository;
        _masterKey = Environment.GetEnvironmentVariable("MSSQL_MCP_API_KEY") ??
            configuration["ApiSecurity:ApiKey"] ??
            "";
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip auth check if API key is not configured and we have no API key manager
        if (string.IsNullOrEmpty(_masterKey) && _apiKeyManager == null)
        {
            _logger.LogWarning("API key authentication is disabled. No API key configured and no API key manager available.");
            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers["Authorization"].ToString();
        var apiKeyHeader = context.Request.Headers["X-API-Key"].ToString();

        // Check for either Bearer token or X-API-Key
        if (string.IsNullOrEmpty(authHeader) && string.IsNullOrEmpty(apiKeyHeader))
        {
            _logger.LogWarning("No authentication provided - neither Bearer token nor X-API-Key present");
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Authentication required",
                message = "Please provide either a Bearer token in the Authorization header or an API key in the X-API-Key header"
            });
            return;
        }

        // Check Bearer token if present
        if (!string.IsNullOrEmpty(authHeader))
        {
            if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Invalid Authorization header format");
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Invalid Authorization format",
                    message = "Authorization header must use Bearer scheme"
                });
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            // First check the master key
            if (!string.IsNullOrEmpty(_masterKey) && string.Equals(token, _masterKey))
            {
                _logger.LogInformation("Successfully authenticated using master Bearer token");
                context.Items["IsAdmin"] = true; // Mark as admin/master key
                await _next(context);
                return;
            }

            // Then check the API key repository
            if (_apiKeyManager != null)
            {
                bool isValid = await _apiKeyManager.ValidateApiKeyAsync(token);
                if (isValid)
                {
                    _logger.LogInformation("Successfully authenticated using stored Bearer token");

                    // Get the API key using the token value, not ID
                    var apiKey = await _apiKeyRepository.GetApiKeyByValueAsync(token);
                    if (apiKey != null)
                    {
                        await _apiKeyManager.LogApiKeyUsageAsync(context, apiKey.Id, apiKey.UserId);
                        await _next(context);
                        return;
                    }
                    else
                    {
                        _logger.LogWarning("API key validated but could not be retrieved for usage logging");
                        // Still allow the request to proceed
                        await _next(context);
                        return;
                    }
                }
            }
        }

        // Check X-API-Key if present
        if (!string.IsNullOrEmpty(apiKeyHeader))
        {
            // First check the master key
            if (!string.IsNullOrEmpty(_masterKey) && string.Equals(apiKeyHeader, _masterKey))
            {
                _logger.LogInformation("Successfully authenticated using master X-API-Key");
                context.Items["IsAdmin"] = true; // Mark as admin/master key
                await _next(context);
                return;
            }

            // Then check the API key repository
            if (_apiKeyManager != null)
            {
                var isValid = await _apiKeyManager.ValidateApiKeyAsync(apiKeyHeader);
                if (isValid)
                {
                    _logger.LogInformation("Successfully authenticated using stored X-API-Key");

                    // Get the API key using the value, not ID
                    var apiKey = await _apiKeyRepository.GetApiKeyByValueAsync(apiKeyHeader);
                    if (apiKey != null)
                    {
                        await _apiKeyManager.LogApiKeyUsageAsync(context, apiKey.Id, apiKey.UserId);
                        await _next(context);
                        return;
                    }
                    else
                    {
                        _logger.LogWarning("API key validated but could not be retrieved for usage logging");
                        // Still allow the request to proceed
                        await _next(context);
                        return;
                    }
                }
            }
        }

        // If we get here, neither authentication method was successful
        _logger.LogWarning("Invalid authentication credentials provided");
        context.Response.StatusCode = 403; // Forbidden
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Invalid authentication",
            message = "The provided authentication credentials are not valid"
        });
    }
}