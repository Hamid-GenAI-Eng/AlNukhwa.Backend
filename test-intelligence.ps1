# Test Intelligence Module Script

$baseUrl = "http://localhost:5050/api"
$userAEmail = "patientAi$(Get-Random)@test.com"
$userBEmail = "hakeemAi$(Get-Random)@test.com"
$phoneA = "0300$(Get-Random -Minimum 1000000 -Maximum 9999999)"
$phoneB = "0300$(Get-Random -Minimum 1000000 -Maximum 9999999)"
$password = "Test@123"

function Assert-Success($condition, $context) {
    if ($condition) {
        Write-Host "✅ $context Success" -ForegroundColor Green
    } else {
        Write-Host "❌ $context Failed" -ForegroundColor Red
    }
}

function Log-Error($ex) {
    Write-Host "❌ Error: $($ex.Exception.Message)" -ForegroundColor Red
    if ($ex.Exception.Response) {
        $stream = $ex.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        Write-Host "   Response: $($reader.ReadToEnd())" -ForegroundColor DarkRed
    }
}

Write-Host "--- Starting Intelligence Verification ---" -ForegroundColor Cyan

# 1. User Setup
try {
    # Register User A
    $regA = @{ email = $userAEmail; password = $password; fullName = "Patient A"; phone = $phoneA } | ConvertTo-Json
    Invoke-RestMethod -Uri "$baseUrl/auth/register/patient" -Method Post -Body $regA -ContentType "application/json"
    
    $loginA = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method Post -Body (@{ email = $userAEmail; password = $password } | ConvertTo-Json) -ContentType "application/json"
    $tokenA = $loginA.accessToken
    $headersA = @{ Authorization = "Bearer $tokenA" }
    $userIdA = $loginA.userId
    Write-Host "✅ User A Registered ($userAEmail)" -ForegroundColor Green

    # Register User B
    $regB = @{ email = $userBEmail; password = $password; fullName = "Hakeem B"; phone = $phoneB; licenseNumber = "LIC-$(Get-Random)" } | ConvertTo-Json
    Invoke-RestMethod -Uri "$baseUrl/auth/register/hakeem" -Method Post -Body $regB -ContentType "application/json"
    
    $loginB = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method Post -Body (@{ email = $userBEmail; password = $password } | ConvertTo-Json) -ContentType "application/json"
    $tokenB = $loginB.accessToken
    $headersB = @{ Authorization = "Bearer $tokenB" }
    $userIdB = $loginB.userId
    Write-Host "✅ User B Registered ($userBEmail)" -ForegroundColor Green

} catch { Log-Error $_; exit }

# 2. Messaging Flow
try {
    # A starts conversation with B
    $startBody = @{ recipientId = $userIdB; content = "Salaam Hakeem Sahib" } | ConvertTo-Json
    $msgRes = Invoke-RestMethod -Uri "$baseUrl/conversations" -Method Post -Body $startBody -Headers $headersA -ContentType "application/json"
    Assert-Success ($msgRes.messageId -ne $null) "Start Conversation"

    # B lists conversations
    $convsB = Invoke-RestMethod -Uri "$baseUrl/conversations" -Headers $headersB
    Assert-Success ($convsB.Count -ge 1) "List Conversations (B)"
    $convId = $convsB[0].id

    # B reads message
    Invoke-RestMethod -Uri "$baseUrl/conversations/$convId/read" -Method Put -Headers $headersB
    
    # B Replies
    $replyBody = @{ content = "Walaikum Assalam" } | ConvertTo-Json
    Invoke-RestMethod -Uri "$baseUrl/conversations/$convId/messages" -Method Post -Body $replyBody -Headers $headersB -ContentType "application/json"
    Write-Host "✅ Reply Sent" -ForegroundColor Green

    # A gets messages
    $msgsA = Invoke-RestMethod -Uri "$baseUrl/conversations/$convId" -Headers $headersA
    Assert-Success ($msgsA.Count -ge 2) "Get Messages (A)"
    Assert-Success ($msgsA[1].content -eq "Walaikum Assalam") "Verify Reply Content"

} catch { Log-Error $_ }

# 3. AI Chat Flow
try {
    # A sends message to AI
    $aiBody = @{ message = "I have a headache"; tibbMode = $true } | ConvertTo-Json
    $aiRes = Invoke-RestMethod -Uri "$baseUrl/ai/chat" -Method Post -Body $aiBody -Headers $headersA -ContentType "application/json"
    Assert-Success ($aiRes.sessionId -ne $null) "Send AI Message"
    $sessionId = $aiRes.sessionId
    $aiMsgId = $aiRes.aiMessageId
    
    Write-Host "   AI Response: $($aiRes.aiResponse)" -ForegroundColor Yellow

    # List Sessions
    $sessions = Invoke-RestMethod -Uri "$baseUrl/ai/sessions" -Headers $headersA
    Assert-Success ($sessions.Count -ge 1) "List AI Sessions"

    # Get Session Messages
    $aiMsgs = Invoke-RestMethod -Uri "$baseUrl/ai/sessions/$sessionId" -Headers $headersA
    Assert-Success ($aiMsgs.Count -ge 2) "Get Session Messages"

    # Submit Feedback
    $feedBody = @{ messageId = $aiMsgId; isPositive = $true } | ConvertTo-Json
    Invoke-RestMethod -Uri "$baseUrl/ai/feedback" -Method Post -Body $feedBody -Headers $headersA -ContentType "application/json"
    Write-Host "✅ Feedback Submitted" -ForegroundColor Green

} catch { Log-Error $_ }

Write-Host "--- Verification Complete ---" -ForegroundColor Cyan
