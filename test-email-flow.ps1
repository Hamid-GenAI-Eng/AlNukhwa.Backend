$baseUrl = "http://localhost:5050" # Updated port from launchSettings
$email = "testuser@example.com" # Ensure this user exists or register one

# 1. Register User (if not exists)
Write-Host "Registering User..."
$registerBody = @{
    FirstName = "Test";
    LastName = "User";
    Email = $email;
    Password = "Password123!";
    Role = "Patient"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/register/patient" -Method Post -Body $registerBody -ContentType "application/json" -ErrorAction Stop
    Write-Host "Registration Successful."
} catch {
    Write-Host "User might already exist or error: $($_.Exception.Message)"
}

# 2. Request Forgot Password
Write-Host "Requesting Password Reset..."
$forgotBody = @{
    Email = $email
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/forgot-password" -Method Post -Body $forgotBody -ContentType "application/json" -ErrorAction Stop
    Write-Host "Forgot Password Request Successful."
    Write-Host "Status: OK"
} catch {
    Write-Host "Forgot Password Request Failed: $($_.Exception.Message)"
    # Start-Sleep -Seconds 5
    # exit
}

# Note: We cannot automate retrieving the link from email without access to the inbox.
# But a 200 OK above means the email service (Resend) accepted the request.

Write-Host "Done. Check email for the link."
