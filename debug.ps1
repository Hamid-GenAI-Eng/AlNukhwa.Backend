try {
    $response = Invoke-WebRequest -Uri "http://localhost:5050/api/hakeems?Page=1" -UseBasicParsing
    Write-Output $response.Content
} catch {
    $e = $_.Exception
    if ($e.Response) {
        $stream = $e.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        Write-Output $reader.ReadToEnd()
    } else {
        Write-Output "Error: $($e.Message)"
    }
}
