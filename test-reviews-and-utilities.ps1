# Test Reviews and Utilities Script

$baseUrl = "http://localhost:5050/api"
$adminEmail = "MisanAdmin@codeenvision.com"
$adminPassword = "XYZ@890^"

function Assert-Success($response, $context) {
    if ($response) {
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

Write-Host "--- Starting Review & Utility Verification ---" -ForegroundColor Cyan

# 1. Admin Login & Product Setup
try {
    $adminBody = @{ email = $adminEmail; password = $adminPassword } | ConvertTo-Json
    $adminAuth = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method Post -Body $adminBody -ContentType "application/json"
    $adminToken = $adminAuth.accessToken
    $adminHeaders = @{ Authorization = "Bearer $adminToken" }
    
    # Create Product
    $product = @{
        name = "Review Test Product $(Get-Random)"
        description = "Test Description"
        price = 50
        stock = 100
        category = "Test"
        mizaj = 1
        imageUrl = "http://test.com/img.png"
        ingredients = @("Test")
    } | ConvertTo-Json
    
    $createdProduct = Invoke-RestMethod -Uri "$baseUrl/products" -Method Post -Body $product -Headers $adminHeaders -ContentType "application/json"
    if ($createdProduct.productId) { $productId = $createdProduct.productId } else { $productId = $createdProduct.id }
    
    Write-Host "✅ Created Product ($productId)" -ForegroundColor Green
} catch { Log-Error $_; exit }

# 2. Product Reviews
try {
    # Add Review
    $reviewBody = @{ rating = 5; comment = "Excellent product!" } | ConvertTo-Json
    Invoke-RestMethod -Uri "$baseUrl/products/$productId/reviews" -Method Post -Body $reviewBody -Headers $adminHeaders -ContentType "application/json"
    Write-Host "✅ Added Review" -ForegroundColor Green

    # Get Reviews
    $reviews = Invoke-RestMethod -Uri "$baseUrl/products/$productId/reviews"
    Assert-Success ($reviews.Count -ge 1) "Get Reviews"
} catch { Log-Error $_ }

# 3. Order Utilities (Using Fresh Shop User)
try {
    $shopEmail = "utilityUser$(Get-Random)@test.com"
    $shopPass = "Test@123"
    $phone = "0300$(Get-Random -Minimum 10000000 -Maximum 99999999)"
    $regBody = @{ email = $shopEmail; password = $shopPass; fullName = "Utility User"; phone = $phone } | ConvertTo-Json
    
    Invoke-RestMethod -Uri "$baseUrl/auth/register/patient" -Method Post -Body $regBody -ContentType "application/json"
    
    $loginBody = @{ email = $shopEmail; password = $shopPass } | ConvertTo-Json
    $shopAuth = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method Post -Body $loginBody -ContentType "application/json"
    $shopToken = $shopAuth.accessToken
    $shopHeaders = @{ Authorization = "Bearer $shopToken" }

    # Place Order
    $cartItem = @{ productId = $productId; quantity = 1 } | ConvertTo-Json
    Invoke-RestMethod -Uri "$baseUrl/cart/items" -Method Post -Body $cartItem -Headers $shopHeaders -ContentType "application/json"
    
    $checkoutBody = @{ shippingAddressId = [Guid]::NewGuid() } | ConvertTo-Json
    $orderRes = Invoke-RestMethod -Uri "$baseUrl/orders" -Method Post -Body $checkoutBody -Headers $shopHeaders -ContentType "application/json"
    $orderId = $orderRes.orderId
    Write-Host "✅ Created Order ($orderId)" -ForegroundColor Green

    # Get Order Detail
    $orderDetail = Invoke-RestMethod -Uri "$baseUrl/orders/$orderId" -Headers $shopHeaders
    Assert-Success ($orderDetail.id -eq $orderId) "Get Order Detail"

    # Get Invoice
    $invoice = Invoke-RestMethod -Uri "$baseUrl/orders/$orderId/invoice" -Headers $shopHeaders
    Assert-Success ($invoice.url -ne $null) "Get Invoice URL"

    # Track Shipment
    $track = Invoke-RestMethod -Uri "$baseUrl/orders/$orderId/track" -Headers $shopHeaders
    Assert-Success ($track.trackingNumber -ne $null) "Track Shipment"

    # Cancel Order
    try {
        Invoke-RestMethod -Uri "$baseUrl/orders/$orderId/cancel" -Method Put -Headers $shopHeaders
        Write-Host "✅ Cancel Order Success" -ForegroundColor Green
    } catch { Log-Error $_ }

} catch { Log-Error $_ }

# 4. Wishlist Check (Using Shop User)
try {
    # Add to Wishlist
    $wishBody = @{ itemType = "Product"; itemId = $productId } | ConvertTo-Json
    Invoke-RestMethod -Uri "$baseUrl/wishlist" -Method Post -Body $wishBody -Headers $shopHeaders -ContentType "application/json"
    
    # Check Status
    $check = Invoke-RestMethod -Uri "$baseUrl/wishlist/check/Product/$productId" -Headers $shopHeaders
    Assert-Success ($check.exists -eq $true) "Check Wishlist Status (Exists)"

} catch { Log-Error $_ }

Write-Host "--- Verification Complete ---" -ForegroundColor Cyan
