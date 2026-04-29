# PowerShell skripta za kreiranje admin naloga
$body = @{
    firstName = "Admin"
    lastName = "User"
    username = "admin"
    email = "admin@posta.ba"
    password = "Admin123!"
    role = "Administrator"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5032/api/users" -Method POST -Body $body -ContentType "application/json"
    Write-Host "✅ Admin nalog kreiran!" -ForegroundColor Green
    Write-Host "Username: admin" -ForegroundColor Yellow
    Write-Host "Password: Admin123!" -ForegroundColor Yellow
    Write-Host "Email: admin@posta.ba" -ForegroundColor Yellow
} catch {
    Write-Host "❌ Greška: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Provjeri da li backend radi na http://localhost:5032" -ForegroundColor Yellow
}
