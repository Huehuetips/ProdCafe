# Script de diagnóstico mejorado - Crea datos desde cero
$baseUrl = "http://localhost:5190/api"

Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "     DIAGNÓSTICO COMPLETO - API CAFÉ (Datos desde Cero)" -ForegroundColor Green
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""

$testData = @{
    ProveedorId = $null
    ClienteId = $null
    PresentacionId = $null
    ProductoId = $null
    TipoGranoId = $null
    EtapaId = $null
    RutaId = $null
    OrdenCompraId = $null
    LoteId = $null
    LoteTerminadoId = $null
    CatacionId = $null
    PedidoId = $null
}

$timestamp = Get-Date -Format "HHmmss"

# 1. Crear Proveedor
Write-Host "1. Creando Proveedor..." -ForegroundColor Yellow
try {
    $body = @{
        nombre = "Proveedor Test $timestamp"
    } | ConvertTo-Json
    
    $result = Invoke-RestMethod -Uri "$baseUrl/Proveedores" -Method POST -Body $body -ContentType "application/json"
    $testData.ProveedorId = $result.id
    Write-Host "   ? Proveedor creado: ID = $($result.id)" -ForegroundColor Green
} catch {
    Write-Host "   ? Error al crear proveedor" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "   Detalles: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
    }
    exit
}

# 2. Crear Cliente
Write-Host "2. Creando Cliente..." -ForegroundColor Yellow
try {
    $body = @{
        nombre = "Cliente Test $timestamp"
        tipo = "Minorista"
        email = "test$timestamp@test.com"
    } | ConvertTo-Json
    
    $result = Invoke-RestMethod -Uri "$baseUrl/Clientes" -Method POST -Body $body -ContentType "application/json"
    $testData.ClienteId = $result.id
    Write-Host "   ? Cliente creado: ID = $($result.id)" -ForegroundColor Green
} catch {
    Write-Host "   ? Error al crear cliente" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "   Detalles: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
    }
    exit
}

# 3. Crear Presentación
Write-Host "3. Creando Presentación..." -ForegroundColor Yellow
try {
    $body = @{
        tipo = "Bolsa 500g Test $timestamp"
    } | ConvertTo-Json
    
    $result = Invoke-RestMethod -Uri "$baseUrl/Presentaciones" -Method POST -Body $body -ContentType "application/json"
    $testData.PresentacionId = $result.id
    Write-Host "   ? Presentación creada: ID = $($result.id)" -ForegroundColor Green
} catch {
    Write-Host "   ? Error al crear presentación" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "   Detalles: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
    }
    exit
}

# 4. Crear Producto
Write-Host "4. Creando Producto..." -ForegroundColor Yellow
try {
    $body = @{
        nombre = "Producto Test $timestamp"
        presentacionId = $testData.PresentacionId
        nivelTostado = "Medio"
        tipoMolido = "Fino"
        precio = 25.99
    } | ConvertTo-Json
    
    $result = Invoke-RestMethod -Uri "$baseUrl/Productos" -Method POST -Body $body -ContentType "application/json"
    $testData.ProductoId = $result.id
    Write-Host "   ? Producto creado: ID = $($result.id)" -ForegroundColor Green
} catch {
    Write-Host "   ? Error al crear producto" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "   Detalles: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
    }
    exit
}

# 5. Crear Tipo de Grano
Write-Host "5. Creando Tipo de Grano..." -ForegroundColor Yellow
try {
    # Usar Blends si Arábica y Robusta ya existen
    $tiposGrano = @("Arábica", "Robusta", "Blends")
    $tipoGranoNombre = $null
    
    foreach ($tipo in $tiposGrano) {
        try {
            $body = @{ nombre = $tipo } | ConvertTo-Json
            $result = Invoke-RestMethod -Uri "$baseUrl/TiposGranoes" -Method POST -Body $body -ContentType "application/json"
            $testData.TipoGranoId = $result.id
            $tipoGranoNombre = $tipo
            break
        } catch {
            continue
        }
    }
    
    if ($testData.TipoGranoId) {
        Write-Host "   ? Tipo de Grano creado: $tipoGranoNombre (ID = $($testData.TipoGranoId))" -ForegroundColor Green
    } else {
        Write-Host "   ? No se pudo crear tipo de grano, usando existente" -ForegroundColor Yellow
        $existentes = Invoke-RestMethod -Uri "$baseUrl/TiposGranoes" -Method GET
        $testData.TipoGranoId = $existentes[0].id
        Write-Host "   ? Usando tipo existente: ID = $($testData.TipoGranoId)" -ForegroundColor Green
    }
} catch {
    Write-Host "   ? Error fatal con tipos de grano" -ForegroundColor Red
    exit
}

# 6. Crear Etapa
Write-Host "6. Creando Etapa..." -ForegroundColor Yellow
try {
    $etapas = @("Tostado", "Molienda", "Empaque")
    $etapaNombre = $null
    
    foreach ($etapa in $etapas) {
        try {
            $body = @{ nombre = $etapa } | ConvertTo-Json
            $result = Invoke-RestMethod -Uri "$baseUrl/Etapas" -Method POST -Body $body -ContentType "application/json"
            $testData.EtapaId = $result.id
            $etapaNombre = $etapa
            break
        } catch {
            continue
        }
    }
    
    if ($testData.EtapaId) {
        Write-Host "   ? Etapa creada: $etapaNombre (ID = $($testData.EtapaId))" -ForegroundColor Green
    } else {
        Write-Host "   ? No se pudo crear etapa, usando existente" -ForegroundColor Yellow
        $existentes = Invoke-RestMethod -Uri "$baseUrl/Etapas" -Method GET
        $testData.EtapaId = $existentes[0].id
        Write-Host "   ? Usando etapa existente: ID = $($testData.EtapaId)" -ForegroundColor Green
    }
} catch {
    Write-Host "   ? Error fatal con etapas" -ForegroundColor Red
    exit
}

# 7. Crear Ruta
Write-Host "7. Creando Ruta..." -ForegroundColor Yellow
try {
    $body = @{
        tipo = "Urbana"
        nombre = "Ruta Test $timestamp"
        zona = "Centro"
        tiempoEstimadoH = 2.5
    } | ConvertTo-Json
    
    $result = Invoke-RestMethod -Uri "$baseUrl/Rutas" -Method POST -Body $body -ContentType "application/json"
    $testData.RutaId = $result.id
    Write-Host "   ? Ruta creada: ID = $($result.id)" -ForegroundColor Green
} catch {
    Write-Host "   ? Error al crear ruta" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "   Detalles: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
    }
    exit
}

# 8. Crear Orden de Compra
Write-Host "8. Creando Orden de Compra..." -ForegroundColor Yellow
try {
    $body = @{
        proveedorId = $testData.ProveedorId
        estado = "Pendiente"
        fechaEmision = (Get-Date).ToString("yyyy-MM-dd")
        fechaRecepcion = $null
    } | ConvertTo-Json
    
    $result = Invoke-RestMethod -Uri "$baseUrl/OrdenesCompras" -Method POST -Body $body -ContentType "application/json"
    $testData.OrdenCompraId = $result.id
    Write-Host "   ? Orden de Compra creada: ID = $($result.id)" -ForegroundColor Green
} catch {
    Write-Host "   ? Error al crear orden de compra" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "   Detalles: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
    }
    exit
}

# 9. Crear Lote
Write-Host "9. Creando Lote..." -ForegroundColor Yellow
try {
    $codigo = "T$timestamp"
    if ($codigo.Length -lt 6) {
        $codigo = $codigo.PadRight(6, '0')
    }
    $codigo = $codigo.Substring(0, 6)
    
    $body = @{
        codigo = $codigo
        fechaIngreso = (Get-Date).ToString("yyyy-MM-dd")
        fechaLote = (Get-Date).ToString("yyyy-MM-dd")
        fechaVencimiento = (Get-Date).AddMonths(6).ToString("yyyy-MM-dd")
        estado = "Activo"
        observaciones = $null
    } | ConvertTo-Json
    
    $result = Invoke-RestMethod -Uri "$baseUrl/Lotes" -Method POST -Body $body -ContentType "application/json"
    $testData.LoteId = $result.id
    Write-Host "   ? Lote creado: Código = $codigo, ID = $($result.id)" -ForegroundColor Green
} catch {
    Write-Host "   ? Error al crear lote" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "   Detalles: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
    }
    exit
}

# 10. Crear Lote Terminado
Write-Host "10. Creando Lote Terminado..." -ForegroundColor Yellow
try {
    $body = @{
        loteId = $testData.LoteId
        productoId = $testData.ProductoId
        fechaEnvasado = (Get-Date).ToString("yyyy-MM-dd")
        fechaVencimiento = (Get-Date).AddMonths(6).ToString("yyyy-MM-dd")
    } | ConvertTo-Json
    
    $result = Invoke-RestMethod -Uri "$baseUrl/LotesTerminados" -Method POST -Body $body -ContentType "application/json"
    $testData.LoteTerminadoId = $result.id
    Write-Host "   ? Lote Terminado creado: ID = $($result.id)" -ForegroundColor Green
} catch {
    Write-Host "   ? Error al crear lote terminado" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "   Detalles: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
    }
    exit
}

# 11. Crear Catación
Write-Host "11. Creando Catación..." -ForegroundColor Yellow
try {
    $body = @{
        loteTerminadoId = $testData.LoteTerminadoId
        puntaje = 87.5
        humedad = 12.3
        notas = "Excelente calidad"
        aprobado = $true
        fecha = (Get-Date).ToString("yyyy-MM-dd")
    } | ConvertTo-Json
    
    $result = Invoke-RestMethod -Uri "$baseUrl/Catacions" -Method POST -Body $body -ContentType "application/json"
    $testData.CatacionId = $result.id
    Write-Host "   ? Catación creada: ID = $($result.id), Puntaje = 87.5" -ForegroundColor Green
} catch {
    Write-Host "   ? Error al crear catación" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "   Detalles: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
    }
    exit
}

# 12. Crear Pedido
Write-Host "12. Creando Pedido..." -ForegroundColor Yellow
try {
    $body = @{
        clienteId = $testData.ClienteId
        fecha = (Get-Date).ToString("yyyy-MM-dd")
        estado = "Pendiente"
        tipo = "Delivery"
        prioritaria = $true
    } | ConvertTo-Json
    
    $result = Invoke-RestMethod -Uri "$baseUrl/Pedidos" -Method POST -Body $body -ContentType "application/json"
    $testData.PedidoId = $result.id
    Write-Host "   ? Pedido creado: ID = $($result.id)" -ForegroundColor Green
} catch {
    Write-Host "   ? Error al crear pedido" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "   Detalles: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
    }
    exit
}

Write-Host ""
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "                     RESUMEN DE CREACIÓN" -ForegroundColor Green
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "? Todos los datos fueron creados exitosamente!" -ForegroundColor Green
Write-Host ""
Write-Host "IDs Creados:" -ForegroundColor Yellow
Write-Host "  - Proveedor:        $($testData.ProveedorId)" -ForegroundColor White
Write-Host "  - Cliente:          $($testData.ClienteId)" -ForegroundColor White
Write-Host "  - Presentación:     $($testData.PresentacionId)" -ForegroundColor White
Write-Host "  - Producto:         $($testData.ProductoId)" -ForegroundColor White
Write-Host "  - Tipo Grano:       $($testData.TipoGranoId)" -ForegroundColor White
Write-Host "  - Etapa:            $($testData.EtapaId)" -ForegroundColor White
Write-Host "  - Ruta:             $($testData.RutaId)" -ForegroundColor White
Write-Host "  - Orden Compra:     $($testData.OrdenCompraId)" -ForegroundColor White
Write-Host "  - Lote:             $($testData.LoteId)" -ForegroundColor White
Write-Host "  - Lote Terminado:   $($testData.LoteTerminadoId)" -ForegroundColor White
Write-Host "  - Catación:         $($testData.CatacionId)" -ForegroundColor White
Write-Host "  - Pedido:           $($testData.PedidoId)" -ForegroundColor White
Write-Host ""
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "¡Flujo completo de producción creado!" -ForegroundColor Green
Write-Host "Desde compra de materia prima hasta pedido del cliente." -ForegroundColor Green
Write-Host ""
Write-Host "Puedes verificar en Swagger: http://localhost:5190" -ForegroundColor Yellow
Write-Host "================================================================" -ForegroundColor Cyan
