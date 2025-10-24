# Script de diagnóstico - Ver errores detallados
$baseUrl = "http://localhost:5190/api"

Write-Host "DIAGNÓSTICO DE ERRORES - API CAFÉ" -ForegroundColor Cyan
Write-Host ""

# Función para convertir fecha a formato DateOnly
function Get-DateOnlyFormat {
    param([DateTime]$date)
    return $date.ToString("yyyy-MM-dd")
}

# Test Producto
Write-Host "1. Probando crear Producto..." -ForegroundColor Yellow
try {
    $body = @{
        nombre = "Test Product"
        presentacionId = 2
        nivelTostado = "Medio"
        tipoMolido = "Fino"
        precio = 25.99
    } | ConvertTo-Json -Depth 10
    
    Write-Host "Enviando: $body" -ForegroundColor Gray
    $result = Invoke-RestMethod -Uri "$baseUrl/Productos" -Method POST -Body $body -ContentType "application/json"
    Write-Host "? Producto creado exitosamente: ID = $($result.id)" -ForegroundColor Green
} catch {
    Write-Host "? Error al crear producto:" -ForegroundColor Red
    Write-Host "Status: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "Detalles: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
    }
}

Write-Host ""

# Test TipoGrano
Write-Host "2. Probando crear Tipo de Grano..." -ForegroundColor Yellow
try {
    $body = @{
        nombre = "Robusta"
    } | ConvertTo-Json -Depth 10
    
    Write-Host "Enviando: $body" -ForegroundColor Gray
    $result = Invoke-RestMethod -Uri "$baseUrl/TiposGranoes" -Method POST -Body $body -ContentType "application/json"
    Write-Host "? Tipo de Grano creado exitosamente: ID = $($result.id)" -ForegroundColor Green
} catch {
    Write-Host "? Error al crear tipo de grano:" -ForegroundColor Red
    Write-Host "Status: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "Detalles: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
    }
}

Write-Host ""

# Test Etapa
Write-Host "3. Probando crear Etapa..." -ForegroundColor Yellow
try {
    $body = @{
        nombre = "Molienda"
    } | ConvertTo-Json -Depth 10
    
    Write-Host "Enviando: $body" -ForegroundColor Gray
    $result = Invoke-RestMethod -Uri "$baseUrl/Etapas" -Method POST -Body $body -ContentType "application/json"
    Write-Host "? Etapa creada exitosamente: ID = $($result.id)" -ForegroundColor Green
} catch {
    Write-Host "? Error al crear etapa:" -ForegroundColor Red
    Write-Host "Status: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "Detalles: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
    }
}

Write-Host ""

# Test Orden de Compra - Con DateOnly
Write-Host "4. Probando crear Orden de Compra..." -ForegroundColor Yellow
try {
    $fechaEmision = Get-DateOnlyFormat (Get-Date)
    $body = @{
        proveedorId = 1
        estado = "Pendiente"
        fechaEmision = $fechaEmision
        fechaRecepcion = $null
    } | ConvertTo-Json -Depth 10
    
    Write-Host "Enviando: $body" -ForegroundColor Gray
    $result = Invoke-RestMethod -Uri "$baseUrl/OrdenesCompras" -Method POST -Body $body -ContentType "application/json"
    Write-Host "? Orden creada exitosamente: ID = $($result.id)" -ForegroundColor Green
} catch {
    Write-Host "? Error al crear orden:" -ForegroundColor Red
    Write-Host "Status: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "Detalles: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
    }
}

Write-Host ""

# Test Lote - Con DateOnly
Write-Host "5. Probando crear Lote..." -ForegroundColor Yellow
try {
    $fechaIngreso = Get-DateOnlyFormat (Get-Date)
    $fechaLote = Get-DateOnlyFormat (Get-Date)
    $fechaVencimiento = Get-DateOnlyFormat ((Get-Date).AddMonths(6))
    
    $body = @{
        codigo = "TST001"
        fechaIngreso = $fechaIngreso
        fechaLote = $fechaLote
        fechaVencimiento = $fechaVencimiento
        estado = "Activo"
        observaciones = $null
    } | ConvertTo-Json -Depth 10
    
    Write-Host "Enviando: $body" -ForegroundColor Gray
    $result = Invoke-RestMethod -Uri "$baseUrl/Lotes" -Method POST -Body $body -ContentType "application/json"
    Write-Host "? Lote creado exitosamente: ID = $($result.id)" -ForegroundColor Green
} catch {
    Write-Host "? Error al crear lote:" -ForegroundColor Red
    Write-Host "Status: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "Detalles: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
    }
}

Write-Host ""

# Test Pedido - Con DateOnly
Write-Host "6. Probando crear Pedido..." -ForegroundColor Yellow
try {
    $fecha = Get-DateOnlyFormat (Get-Date)
    $body = @{
        clienteId = 2
        fecha = $fecha
        estado = "Pendiente"
        tipo = "Delivery"
        prioritaria = $true
    } | ConvertTo-Json -Depth 10
    
    Write-Host "Enviando: $body" -ForegroundColor Gray
    $result = Invoke-RestMethod -Uri "$baseUrl/Pedidos" -Method POST -Body $body -ContentType "application/json"
    Write-Host "? Pedido creado exitosamente: ID = $($result.id)" -ForegroundColor Green
} catch {
    Write-Host "? Error al crear pedido:" -ForegroundColor Red
    Write-Host "Status: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "Detalles: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "DIAGNÓSTICO COMPLETADO" -ForegroundColor Cyan
Write-Host ""
Write-Host "NOTA: Si aún hay errores 400, revisa los logs de la API." -ForegroundColor Yellow
Write-Host "Ejecuta: dotnet run --launch-profile http" -ForegroundColor Yellow
