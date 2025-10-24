# Script para iniciar la API de Café Producción
Write-Host "=== Iniciando API de Café Producción ===" -ForegroundColor Green
Write-Host ""
Write-Host "La API se abrirá en: http://localhost:5190" -ForegroundColor Yellow
Write-Host "Swagger UI disponible en la URL raíz" -ForegroundColor Yellow
Write-Host ""
Write-Host "Presiona Ctrl+C para detener el servidor" -ForegroundColor Cyan
Write-Host ""

# Abrir navegador automáticamente
Start-Process "http://localhost:5190"

# Iniciar la API
dotnet run --launch-profile http
