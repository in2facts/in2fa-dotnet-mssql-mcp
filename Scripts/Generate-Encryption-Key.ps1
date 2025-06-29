## Generate-Encryption-Key.ps1
## Script to generate a secure encryption key for the multi-key authentication system

# Function to generate a secure key
function Generate-SecureKey {
    param (
        [int]$length = 32
    )
    
    $bytes = New-Object byte[] $length
    $rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
    $rng.GetBytes($bytes)
    $key = [Convert]::ToBase64String($bytes)
    return $key
}

# Generate a key for MSSQL_MCP_KEY
$encryptionKey = Generate-SecureKey -length 32
Write-Host "MSSQL_MCP_KEY encryption key (use this to encrypt API keys):" -ForegroundColor Green
Write-Host $encryptionKey -ForegroundColor Cyan
Write-Host "`nAdd this to your environment variables or .env file:" -ForegroundColor Yellow
Write-Host "MSSQL_MCP_KEY=$encryptionKey" -ForegroundColor White

# Generate a key for MSSQL_MCP_API_KEY (master key)
$masterApiKey = Generate-SecureKey -length 32
Write-Host "`nMSSQL_MCP_API_KEY master API key:" -ForegroundColor Green
Write-Host $masterApiKey -ForegroundColor Cyan
Write-Host "`nAdd this to your environment variables or .env file:" -ForegroundColor Yellow
Write-Host "MSSQL_MCP_API_KEY=$masterApiKey" -ForegroundColor White

Write-Host "`nEnvironment variable commands for PowerShell:" -ForegroundColor Green
Write-Host "`$env:MSSQL_MCP_KEY = `"$encryptionKey`"" -ForegroundColor White
Write-Host "`$env:MSSQL_MCP_API_KEY = `"$masterApiKey`"" -ForegroundColor White

Write-Host "`nEnvironment variable commands for CMD:" -ForegroundColor Green
Write-Host "set MSSQL_MCP_KEY=$encryptionKey" -ForegroundColor White
Write-Host "set MSSQL_MCP_API_KEY=$masterApiKey" -ForegroundColor White

Write-Host "`nEnvironment variable commands for Linux/macOS:" -ForegroundColor Green
Write-Host "export MSSQL_MCP_KEY='$encryptionKey'" -ForegroundColor White
Write-Host "export MSSQL_MCP_API_KEY='$masterApiKey'" -ForegroundColor White
