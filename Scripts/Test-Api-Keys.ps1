## Test-Api-Keys.ps1
## Script to test the multi-key API authentication functionality

# Get the master API key from environment or prompt for it
$masterApiKey = $env:MSSQL_MCP_API_KEY
if (-not $masterApiKey) {
    $masterApiKey = Read-Host -Prompt "Enter the master API key"
}

# Default server URL
$serverUrl = "http://localhost:3001/mcp"
if ($env:MSSQL_MCP_URL) {
    $serverUrl = $env:MSSQL_MCP_URL
}

# Common headers
$headers = @{
    "Authorization" = "Bearer $masterApiKey"
    "Content-Type"  = "application/json"
}

# Helper function to invoke MCP API
function Invoke-McpApi {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Method,
        
        [Parameter(Mandatory = $true)]
        [PSObject]$Body,
        
        [Parameter(Mandatory = $false)]
        [string]$ApiKey = $masterApiKey
    )
    
    $headers = @{
        "Authorization" = "Bearer $ApiKey"
        "Content-Type"  = "application/json"
    }
    
    $jsonBody = $Body | ConvertTo-Json -Depth 10
    
    Write-Host "Calling $Method..."
    $response = Invoke-RestMethod -Uri $serverUrl -Method Post -Headers $headers -Body $jsonBody
    return $response
}

# 1. Create a new API key
$userId = "test-user-" + (Get-Random -Minimum 1000 -Maximum 9999)
$createKeyRequest = @{
    jsonrpc = "2.0"
    id      = 1
    method  = "CreateApiKey"
    params  = @{
        request = @{
            name           = "Test API Key"
            userId         = $userId
            keyType        = "test"
            expirationDate = (Get-Date).AddDays(30).ToString("o")
        }
    }
}

Write-Host "`n--- Creating a new API key ---" -ForegroundColor Green
$createResult = Invoke-McpApi -Method "Create API Key" -Body $createKeyRequest
$createResult.result

# Save the generated API key for later tests
$apiKeyId = $createResult.result.id
$apiKeyValue = $createResult.result.key

Write-Host "`nAPI Key created:" -ForegroundColor Yellow
Write-Host "ID: $apiKeyId"
Write-Host "Value: $apiKeyValue"

# 2. List API keys for the user
$listKeysRequest = @{
    jsonrpc = "2.0"
    id      = 2
    method  = "ListUserApiKeys" 
    params  = @{
        userId = $userId
    }
}

Write-Host "`n--- Listing API keys for user $userId ---" -ForegroundColor Green
$listResult = Invoke-McpApi -Method "List User API Keys" -Body $listKeysRequest
$listResult.result

# 3. Test the new API key with a simple operation (list connections)
$testKeyRequest = @{
    jsonrpc = "2.0"
    id      = 3
    method  = "ListConnections"
    params  = @{}
}

Write-Host "`n--- Testing the new API key with ListConnections ---" -ForegroundColor Green
try {
    $testResult = Invoke-McpApi -Method "Test API Key" -Body $testKeyRequest -ApiKey $apiKeyValue
    Write-Host "API key works! Result:" -ForegroundColor Green
    $testResult.result
}
catch {
    Write-Host "API key test failed: $_" -ForegroundColor Red
}

# 4. Get usage logs for the API key
$getLogsRequest = @{
    jsonrpc = "2.0"
    id      = 4
    method  = "GetApiKeyUsageLogs"
    params  = @{
        apiKeyId = $apiKeyId
        limit    = 10
    }
}

Write-Host "`n--- Getting usage logs for the API key ---" -ForegroundColor Green
$logsResult = Invoke-McpApi -Method "Get API Key Usage Logs" -Body $getLogsRequest
$logsResult.result

# 5. Revoke the API key
$revokeRequest = @{
    jsonrpc = "2.0"
    id      = 5
    method  = "RevokeApiKey"
    params  = @{
        request = @{
            id = $apiKeyId
        }
    }
}

Write-Host "`n--- Revoking the API key ---" -ForegroundColor Green
$revokeResult = Invoke-McpApi -Method "Revoke API Key" -Body $revokeRequest
Write-Host "Revoke result: $($revokeResult.result)"

# 6. Try to use the revoked key
Write-Host "`n--- Testing the revoked API key ---" -ForegroundColor Green
try {
    $testAfterRevokeResult = Invoke-McpApi -Method "Test Revoked API Key" -Body $testKeyRequest -ApiKey $apiKeyValue
    Write-Host "WARNING: API key still works after revocation!" -ForegroundColor Yellow
    $testAfterRevokeResult.result
}
catch {
    Write-Host "API key correctly rejected after revocation: $_" -ForegroundColor Green
}

# 7. Delete the API key
$deleteRequest = @{
    jsonrpc = "2.0"
    id      = 6
    method  = "DeleteApiKey"
    params  = @{
        id = $apiKeyId
    }
}

Write-Host "`n--- Deleting the API key ---" -ForegroundColor Green
$deleteResult = Invoke-McpApi -Method "Delete API Key" -Body $deleteRequest
Write-Host "Delete result: $($deleteResult.result)"

Write-Host "`nAPI Key Test Completed!" -ForegroundColor Green
