# Script para iniciar la API de Caf� Producci�n
Write-Host "=== Iniciando API de Caf� Producci�n ===" -ForegroundColor Green
Write-Host ""
Write-Host "La API se abrir� en: http://localhost:5190" -ForegroundColor Yellow
Write-Host "Swagger UI disponible en la URL ra�z" -ForegroundColor Yellow
Write-Host ""
Write-Host "Presiona Ctrl+C para detener el servidor" -ForegroundColor Cyan
Write-Host ""

# Abrir navegador autom�ticamente
Start-Process "http://localhost:5190"

# Iniciar la API
dotnet run --launch-profile http
