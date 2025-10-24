# ============================================
# SCRIPT DE INICIALIZACIÓN COMPLETA
# ============================================
# Este script configura la base de datos desde cero
# y carga datos de prueba

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "   INICIALIZACIÓN COMPLETA - API CAFÉ" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Paso 1: Verificar .NET
Write-Host "Paso 1: Verificando .NET..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
Write-Host "? .NET versión: $dotnetVersion" -ForegroundColor Green
Write-Host ""

# Paso 2: Preguntar si eliminar base de datos existente
Write-Host "Paso 2: Base de Datos" -ForegroundColor Yellow
$respuesta = Read-Host "¿Deseas eliminar la base de datos existente? (S/N)"
if ($respuesta -eq "S" -or $respuesta -eq "s") {
    Write-Host "Eliminando base de datos..." -ForegroundColor Yellow
    try {
        dotnet ef database drop --force
        Write-Host "? Base de datos eliminada" -ForegroundColor Green
    } catch {
        Write-Host "? No se pudo eliminar la base de datos o no existe" -ForegroundColor Yellow
    }
}
Write-Host ""

# Paso 3: Crear/Actualizar Migraciones
Write-Host "Paso 3: Configurando Migraciones..." -ForegroundColor Yellow
$migrationExists = Test-Path ".\Migrations"

if ($migrationExists) {
    Write-Host "Migraciones existentes encontradas" -ForegroundColor Gray
    Write-Host "Aplicando migraciones a la base de datos..." -ForegroundColor Yellow
    dotnet ef database update
    Write-Host "? Base de datos actualizada" -ForegroundColor Green
} else {
    Write-Host "No se encontraron migraciones" -ForegroundColor Gray
    Write-Host "Creando migración inicial..." -ForegroundColor Yellow
    dotnet ef migrations add InitialCreate
    Write-Host "Aplicando migración a la base de datos..." -ForegroundColor Yellow
    dotnet ef database update
    Write-Host "? Base de datos creada y migración aplicada" -ForegroundColor Green
}
Write-Host ""

# Paso 4: Compilar proyecto
Write-Host "Paso 4: Compilando proyecto..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -eq 0) {
    Write-Host "? Proyecto compilado exitosamente" -ForegroundColor Green
} else {
    Write-Host "? Error al compilar el proyecto" -ForegroundColor Red
    exit
}
Write-Host ""

# Paso 5: Iniciar API en segundo plano
Write-Host "Paso 5: Iniciando API..." -ForegroundColor Yellow
Write-Host "La API se iniciará en http://localhost:5190" -ForegroundColor Gray

# Iniciar API en segundo plano
$apiProcess = Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run --launch-profile http" -PassThru

Write-Host "? API iniciada (PID: $($apiProcess.Id))" -ForegroundColor Green
Write-Host "Esperando 10 segundos a que la API esté lista..." -ForegroundColor Yellow
Start-Sleep -Seconds 10
Write-Host ""

# Paso 6: Verificar conexión con la API
Write-Host "Paso 6: Verificando conexión con la API..." -ForegroundColor Yellow
try {
    $testConnection = Invoke-WebRequest -Uri "http://localhost:5190/api/Proveedores" -Method GET -UseBasicParsing -TimeoutSec 5
    Write-Host "? API respondiendo correctamente" -ForegroundColor Green
} catch {
    Write-Host "? La API no responde. Verifica que esté ejecutándose." -ForegroundColor Red
    Write-Host "Puedes revisar la ventana de la API que se abrió." -ForegroundColor Yellow
    exit
}
Write-Host ""

# Paso 7: Cargar datos de prueba
Write-Host "Paso 7: Cargando datos de prueba..." -ForegroundColor Yellow
$cargarDatos = Read-Host "¿Deseas cargar datos de prueba? (S/N)"
if ($cargarDatos -eq "S" -or $cargarDatos -eq "s") {
    Write-Host "Ejecutando script de carga de datos..." -ForegroundColor Yellow
    Write-Host ""
    
    # Ejecutar script de diagnóstico completo
    & .\diagnostico-completo.ps1
}
Write-Host ""

# Resumen final
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "         INICIALIZACIÓN COMPLETADA" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "? Base de datos configurada" -ForegroundColor Green
Write-Host "? Migraciones aplicadas" -ForegroundColor Green
Write-Host "? API ejecutándose en http://localhost:5190" -ForegroundColor Green
Write-Host "? Swagger disponible en http://localhost:5190" -ForegroundColor Green
if ($cargarDatos -eq "S" -or $cargarDatos -eq "s") {
    Write-Host "? Datos de prueba cargados" -ForegroundColor Green
}
Write-Host ""
Write-Host "Comandos útiles:" -ForegroundColor Yellow
Write-Host "  - Ver datos en Swagger: http://localhost:5190" -ForegroundColor White
Write-Host "  - Detener API: Cierra la ventana de PowerShell de la API" -ForegroundColor White
Write-Host "  - Cargar más datos: .\diagnostico-completo.ps1" -ForegroundColor White
Write-Host "  - Ejecutar pruebas: .\test-completo.ps1" -ForegroundColor White
Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Presiona cualquier tecla para continuar..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
