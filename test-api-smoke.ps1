$ErrorActionPreference = "Stop"
$baseUrl = "http://localhost:5050/api"

Write-Host "1. Registering new patient..."
$email = "testUser_$(Get-Random)@example.com"
$registerBody = @{
    email = $email
    password = "Password123!"
    phone = "+92300$(Get-Random -Minimum 1000000 -Maximum 9999999)"
} | ConvertTo-Json

try {
    $regResponse = Invoke-RestMethod -Uri "$baseUrl/auth/register/patient" -Method Post -Body $registerBody -ContentType "application/json"
    Write-Host "   Success: Registered $email"
} catch {
    Write-Error "   Failed to register: $_"
}

Write-Host "`n2. Logging in..."
$loginBody = @{
    email = $email
    password = "Password123!"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method Post -Body $loginBody -ContentType "application/json"
    $token = $loginResponse.accessToken
    if (-not $token) { $token = $loginResponse.AccessToken } # Fallback
    Write-Host "   Success: Got Token ($($token.Substring(0, 10))...)"
} catch {
    Write-Error "   Failed to login: $_"
}

$headers = @{
    Authorization = "Bearer $token"
}

Write-Host "`n3. Getting 'Me'..."
try {
    $meResponse = Invoke-RestMethod -Uri "$baseUrl/auth/me" -Method Get -Headers $headers
    Write-Host "   Success: User ID is $($meResponse.id)"
} catch {
    Write-Error "   Failed to get Me: $_"
}

Write-Host "`n4. Getting Hakeems (Public)..."
try {
    $hakeems = Invoke-RestMethod -Uri "$baseUrl/hakeems?Page=1&PageSize=5" -Method Get
    Write-Host "   Success: Fetched $(($hakeems | Measure-Object).Count) Hakeems"
} catch {
    Write-Error "   Failed to get Hakeems: $_"
}
