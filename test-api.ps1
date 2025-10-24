# Script para probar la API de Café Producción
# Asegúrate de que la API esté corriendo en http://localhost:5190

Write-Host "=== Probando API de Café Producción ===" -ForegroundColor Green
Write-Host ""

# URL base
$baseUrl = "http://localhost:5190/api"

# Probar conexión
Write-Host "1. Probando conexión con la API..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/Proveedores" -Method GET -UseBasicParsing
    Write-Host "? API respondiendo correctamente (Status: $($response.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "? Error: La API no responde. Asegúrate de ejecutar 'dotnet run --launch-profile http'" -ForegroundColor Red
    exit
}

Write-Host ""
Write-Host "2. Insertando datos de prueba..." -ForegroundColor Yellow

# Insertar Proveedor
Write-Host "   - Creando proveedor..." -ForegroundColor Cyan
$proveedor = @{
    nombre = "Café Colombiano S.A."
} | ConvertTo-Json

$proveedorResult = Invoke-RestMethod -Uri "$baseUrl/Proveedores" -Method POST -Body $proveedor -ContentType "application/json"
Write-Host "   ? Proveedor creado con ID: $($proveedorResult.id)" -ForegroundColor Green

# Insertar Cliente
Write-Host "   - Creando cliente..." -ForegroundColor Cyan
$cliente = @{
    nombre = "Café Express"
    tipo = "Mayorista"
    contacto = "Juan Pérez"
    telefono = "555-1234"
    email = "contacto@cafeexpress.com"
    direccion = "Calle Principal 123"
} | ConvertTo-Json

$clienteResult = Invoke-RestMethod -Uri "$baseUrl/Clientes" -Method POST -Body $cliente -ContentType "application/json"
Write-Host "   ? Cliente creado con ID: $($clienteResult.id)" -ForegroundColor Green

# Insertar Presentación
Write-Host "   - Creando presentación..." -ForegroundColor Cyan
$presentacion = @{
    tipo = "Bolsa 500g"
} | ConvertTo-Json

$presentacionResult = Invoke-RestMethod -Uri "$baseUrl/Presentaciones" -Method POST -Body $presentacion -ContentType "application/json"
Write-Host "   ? Presentación creada con ID: $($presentacionResult.id)" -ForegroundColor Green

# Insertar Tipo de Grano
Write-Host "   - Creando tipo de grano..." -ForegroundColor Cyan
$tipoGrano = @{
    "nombre(arábica|robusta|blends)" = "Arábica"
} | ConvertTo-Json

$tipoGranoResult = Invoke-RestMethod -Uri "$baseUrl/TiposGranos" -Method POST -Body $tipoGrano -ContentType "application/json"
Write-Host "   ? Tipo de grano creado con ID: $($tipoGranoResult.id)" -ForegroundColor Green

# Insertar Etapa
Write-Host "   - Creando etapa..." -ForegroundColor Cyan
$etapa = @{
    "nombre(Tostado|Molienda|Empaque)" = "Tostado"
} | ConvertTo-Json

$etapaResult = Invoke-RestMethod -Uri "$baseUrl/Etapas" -Method POST -Body $etapa -ContentType "application/json"
Write-Host "   ? Etapa creada con ID: $($etapaResult.id)" -ForegroundColor Green

# Insertar Ruta
Write-Host "   - Creando ruta..." -ForegroundColor Cyan
$ruta = @{
    nombre = "Ruta Centro"
    tipo = "Urbana"
    zona = "Centro"
    tiempoEstimadoH = 2.5
} | ConvertTo-Json

$rutaResult = Invoke-RestMethod -Uri "$baseUrl/Rutas" -Method POST -Body $ruta -ContentType "application/json"
Write-Host "   ? Ruta creada con ID: $($rutaResult.id)" -ForegroundColor Green

Write-Host ""
Write-Host "3. Consultando datos insertados..." -ForegroundColor Yellow

# Consultar todos los proveedores
Write-Host "   - Proveedores:" -ForegroundColor Cyan
$proveedores = Invoke-RestMethod -Uri "$baseUrl/Proveedores" -Method GET
$proveedores | ForEach-Object { Write-Host "     • ID: $($_.id) - $($_.nombre)" -ForegroundColor White }

# Consultar todos los clientes
Write-Host "   - Clientes:" -ForegroundColor Cyan
$clientes = Invoke-RestMethod -Uri "$baseUrl/Clientes" -Method GET
$clientes | ForEach-Object { Write-Host "     • ID: $($_.id) - $($_.nombre) ($($_.tipo))" -ForegroundColor White }

# Consultar todas las presentaciones
Write-Host "   - Presentaciones:" -ForegroundColor Cyan
$presentaciones = Invoke-RestMethod -Uri "$baseUrl/Presentaciones" -Method GET
$presentaciones | ForEach-Object { Write-Host "     • ID: $($_.id) - $($_.tipo)" -ForegroundColor White }

# Consultar todos los tipos de grano
Write-Host "   - Tipos de Grano:" -ForegroundColor Cyan
$tiposGrano = Invoke-RestMethod -Uri "$baseUrl/TiposGranos" -Method GET
$tiposGrano | ForEach-Object { Write-Host "     • ID: $($_.id) - $($_.'nombre(arábica|robusta|blends)')" -ForegroundColor White }

# Consultar todas las rutas
Write-Host "   - Rutas:" -ForegroundColor Cyan
$rutas = Invoke-RestMethod -Uri "$baseUrl/Rutas" -Method GET
$rutas | ForEach-Object { Write-Host "     • ID: $($_.id) - $($_.nombre) - Zona: $($_.zona) - Tiempo: $($_.tiempoEstimadoH)h" -ForegroundColor White }

Write-Host ""
Write-Host "=== ¡Prueba completada exitosamente! ===" -ForegroundColor Green
Write-Host ""
Write-Host "Puedes acceder a Swagger UI en: http://localhost:5190" -ForegroundColor Yellow
Write-Host ""
