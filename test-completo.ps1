# ============================================
# PRUEBAS COMPLETAS DE LA API - CAFÉ PRODUCCIÓN
# ============================================
# Este script prueba TODOS los controladores y endpoints
# de la API de forma exhaustiva

Write-Host "=====================================================" -ForegroundColor Cyan
Write-Host "    PRUEBAS COMPLETAS - API CAFÉ PRODUCCIÓN" -ForegroundColor Green
Write-Host "=====================================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5190/api"
$testResults = @{
    Passed = 0
    Failed = 0
    Total = 0
}

# Función para mostrar resultado de prueba
function Test-Endpoint {
    param(
        [string]$Nombre,
        [scriptblock]$Test
    )
    
    $testResults.Total++
    try {
        & $Test
        Write-Host "  ? $Nombre" -ForegroundColor Green
        $testResults.Passed++
        return $true
    }
    catch {
        Write-Host "  ? $Nombre" -ForegroundColor Red
        Write-Host "    Error: $($_.Exception.Message)" -ForegroundColor Yellow
        $testResults.Failed++
        return $false
    }
}

# Variables para almacenar IDs creados
$Global:TestData = @{
    ProveedorId = $null
    ClienteId = $null
    PresentacionId = $null
    ProductoId = $null
    TipoGranoId = $null
    EtapaId = $null
    OrdenCompraId = $null
    LoteId = $null
    LoteTerminadoId = $null
    CatacionId = $null
    PedidoId = $null
    RutaId = $null
    OrdenCompraTipoGranoId = $null
    OrdenCompraTipoGranoLoteId = $null
    LoteEtapaId = $null
    PedidoLoteTerminadoId = $null
    PedidoRutaId = $null
}

Write-Host "Verificando conexión con la API..." -ForegroundColor Yellow
try {
    $testConnection = Invoke-WebRequest -Uri "$baseUrl/Proveedores" -Method GET -UseBasicParsing -TimeoutSec 5
    Write-Host "? API respondiendo correctamente" -ForegroundColor Green
    Write-Host ""
} catch {
    Write-Host "? Error: La API no responde. Asegúrate de ejecutar 'dotnet run --launch-profile http'" -ForegroundColor Red
    exit
}

# ============================================
# 1. PROVEEDORES CONTROLLER
# ============================================
Write-Host "1. Probando ProveedoresController..." -ForegroundColor Cyan

Test-Endpoint "POST - Crear proveedor válido" {
    $body = @{
        nombre = "Proveedor Test $(Get-Date -Format 'HHmmss')"
    } | ConvertTo-Json
    $result = Invoke-RestMethod -Uri "$baseUrl/Proveedores" -Method POST -Body $body -ContentType "application/json"
    $Global:TestData.ProveedorId = $result.id
    if (-not $result.id) { throw "No se retornó ID" }
}

Test-Endpoint "POST - Rechazar proveedor sin nombre" {
    $body = @{ nombre = "" } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/Proveedores" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "GET - Obtener todos los proveedores" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Proveedores" -Method GET
    if (-not $result) { throw "No se obtuvieron proveedores" }
}

Test-Endpoint "GET - Obtener proveedor por ID" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Proveedores/$($Global:TestData.ProveedorId)" -Method GET
    if ($result.id -ne $Global:TestData.ProveedorId) { throw "ID no coincide" }
}

Test-Endpoint "PUT - Actualizar proveedor" {
    $body = @{
        id = $Global:TestData.ProveedorId
        nombre = "Proveedor Actualizado"
    } | ConvertTo-Json
    $result = Invoke-RestMethod -Uri "$baseUrl/Proveedores/$($Global:TestData.ProveedorId)" -Method PUT -Body $body -ContentType "application/json"
}

# ============================================
# 2. CLIENTES CONTROLLER
# ============================================
Write-Host ""
Write-Host "2. Probando ClientesController..." -ForegroundColor Cyan

Test-Endpoint "POST - Crear cliente válido" {
    $body = @{
        nombre = "Cliente Test $(Get-Date -Format 'HHmmss')"
        tipo = "Minorista"
        email = "test$(Get-Date -Format 'HHmmss')@test.com"
        telefono = "123-456-789"
        direccion = "Calle Test 123"
    } | ConvertTo-Json
    $result = Invoke-RestMethod -Uri "$baseUrl/Clientes" -Method POST -Body $body -ContentType "application/json"
    $Global:TestData.ClienteId = $result.id
    if (-not $result.id) { throw "No se retornó ID" }
}

Test-Endpoint "POST - Rechazar cliente con email inválido" {
    $body = @{
        nombre = "Test"
        email = "email-invalido"
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/Clientes" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "GET - Obtener todos los clientes" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Clientes" -Method GET
    if (-not $result) { throw "No se obtuvieron clientes" }
}

Test-Endpoint "GET - Obtener cliente por ID con pedidos" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Clientes/$($Global:TestData.ClienteId)" -Method GET
    if ($result.id -ne $Global:TestData.ClienteId) { throw "ID no coincide" }
    # Pedidos pueden estar vacíos al inicio
}

# ============================================
# 3. PRESENTACIONES CONTROLLER
# ============================================
Write-Host ""
Write-Host "3. Probando PresentacionesController..." -ForegroundColor Cyan

Test-Endpoint "POST - Crear presentación válida" {
    $body = @{
        tipo = "Bolsa 500g Test $(Get-Date -Format 'HHmmss')"
    } | ConvertTo-Json
    $result = Invoke-RestMethod -Uri "$baseUrl/Presentaciones" -Method POST -Body $body -ContentType "application/json"
    $Global:TestData.PresentacionId = $result.id
    if (-not $result.id) { throw "No se retornó ID" }
}

Test-Endpoint "POST - Rechazar presentación sin tipo" {
    $body = @{ tipo = "" } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/Presentaciones" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "GET - Obtener todas las presentaciones" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Presentaciones" -Method GET
    if (-not $result) { throw "No se obtuvieron presentaciones" }
}

# ============================================
# 4. PRODUCTOS CONTROLLER
# ============================================
Write-Host ""
Write-Host "4. Probando ProductosController..." -ForegroundColor Cyan

Test-Endpoint "POST - Crear producto válido" {
    $body = @{
        nombre = "Producto Test $(Get-Date -Format 'HHmmss')"
        presentacionId = $Global:TestData.PresentacionId
        nivelTostado = "Medio"
        tipoMolido = "Fino"
        precio = 25.99
    } | ConvertTo-Json
    $result = Invoke-RestMethod -Uri "$baseUrl/Productos" -Method POST -Body $body -ContentType "application/json"
    $Global:TestData.ProductoId = $result.id
    if (-not $result.id) { throw "No se retornó ID" }
}

Test-Endpoint "POST - Rechazar producto con precio negativo" {
    $body = @{
        nombre = "Test"
        presentacionId = $Global:TestData.PresentacionId
        precio = -10
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/Productos" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "POST - Rechazar producto con presentación inexistente" {
    $body = @{
        nombre = "Test"
        presentacionId = 99999
        precio = 10
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/Productos" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "GET - Obtener todos los productos" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Productos" -Method GET
    if (-not $result) { throw "No se obtuvieron productos" }
}

Test-Endpoint "GET - Obtener producto por ID con relaciones" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Productos/$($Global:TestData.ProductoId)" -Method GET
    if ($result.id -ne $Global:TestData.ProductoId) { throw "ID no coincide" }
    # Presentación puede ser null si no se incluye
}

# ============================================
# 5. TIPOS DE GRANO CONTROLLER
# ============================================
Write-Host ""
Write-Host "5. Probando TiposGranosController..." -ForegroundColor Cyan

Test-Endpoint "POST - Crear tipo de grano válido" {
    # Usar el nombre exacto de la propiedad de la entidad
    $body = @{
        nombre = "Arábica"
    } | ConvertTo-Json
    $result = Invoke-RestMethod -Uri "$baseUrl/TiposGranoes" -Method POST -Body $body -ContentType "application/json"
    $Global:TestData.TipoGranoId = $result.id
    if (-not $result.id) { throw "No se retornó ID" }
}

Test-Endpoint "POST - Rechazar tipo de grano inválido" {
    $body = @{
        nombre = "TipoInvalido"
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/TiposGranoes" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "GET - Obtener todos los tipos de grano" {
    $result = Invoke-RestMethod -Uri "$baseUrl/TiposGranoes" -Method GET
    if (-not $result) { throw "No se obtuvieron tipos de grano" }
}

# ============================================
# 6. ETAPAS CONTROLLER
# ============================================
Write-Host ""
Write-Host "6. Probando EtapasController..." -ForegroundColor Cyan

Test-Endpoint "POST - Crear etapa válida" {
    # Usar el nombre exacto de la propiedad de la entidad
    $body = @{
        nombre = "Tostado"
    } | ConvertTo-Json
    $result = Invoke-RestMethod -Uri "$baseUrl/Etapas" -Method POST -Body $body -ContentType "application/json"
    $Global:TestData.EtapaId = $result.id
    if (-not $result.id) { throw "No se retornó ID" }
}

Test-Endpoint "POST - Rechazar etapa inválida" {
    $body = @{
        nombre = "EtapaInvalida"
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/Etapas" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "GET - Obtener todas las etapas" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Etapas" -Method GET
    if (-not $result) { throw "No se obtuvieron etapas" }
}

# ============================================
# 7. RUTAS CONTROLLER
# ============================================
Write-Host ""
Write-Host "7. Probando RutasController..." -ForegroundColor Cyan

Test-Endpoint "POST - Crear ruta válida" {
    $body = @{
        tipo = "Urbana"
        nombre = "Ruta Test $(Get-Date -Format 'HHmmss')"
        zona = "Centro"
        tiempoEstimadoH = 2.5
    } | ConvertTo-Json
    $result = Invoke-RestMethod -Uri "$baseUrl/Rutas" -Method POST -Body $body -ContentType "application/json"
    $Global:TestData.RutaId = $result.id
    if (-not $result.id) { throw "No se retornó ID" }
}

Test-Endpoint "POST - Rechazar ruta con tiempo negativo" {
    $body = @{
        nombre = "Test"
        tiempoEstimadoH = -1
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/Rutas" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "GET - Obtener todas las rutas" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Rutas" -Method GET
    if (-not $result) { throw "No se obtuvieron rutas" }
}

Test-Endpoint "GET - Obtener rutas por zona" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Rutas/PorZona/Centro" -Method GET
    if ($null -eq $result) { throw "Error en consulta por zona" }
}

Test-Endpoint "GET - Obtener rutas por tipo" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Rutas/PorTipo/Urbana" -Method GET
    if ($null -eq $result) { throw "Error en consulta por tipo" }
}

# ============================================
# 8. ÓRDENES DE COMPRA CONTROLLER
# ============================================
Write-Host ""
Write-Host "8. Probando OrdenesComprasController..." -ForegroundColor Cyan

Test-Endpoint "POST - Crear orden de compra válida" {
    $body = @{
        proveedorId = $Global:TestData.ProveedorId
        estado = "Pendiente"
        fechaEmision = (Get-Date).ToString("yyyy-MM-dd")
        fechaRecepcion = $null
    } | ConvertTo-Json
    $result = Invoke-RestMethod -Uri "$baseUrl/OrdenesCompras" -Method POST -Body $body -ContentType "application/json"
    $Global:TestData.OrdenCompraId = $result.id
    if (-not $result.id) { throw "No se retornó ID" }
}

Test-Endpoint "POST - Rechazar orden con estado inválido" {
    $body = @{
        proveedorId = $Global:TestData.ProveedorId
        estado = "EstadoInvalido"
        fechaEmision = (Get-Date).ToString("yyyy-MM-dd")
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/OrdenesCompras" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "GET - Obtener todas las órdenes" {
    $result = Invoke-RestMethod -Uri "$baseUrl/OrdenesCompras" -Method GET
    if (-not $result) { throw "No se obtuvieron órdenes" }
}

Test-Endpoint "GET - Obtener órdenes por proveedor" {
    $result = Invoke-RestMethod -Uri "$baseUrl/OrdenesCompras/PorProveedor/$($Global:TestData.ProveedorId)" -Method GET
    if ($null -eq $result) { throw "Error en consulta por proveedor" }
}

Test-Endpoint "GET - Obtener órdenes por estado" {
    $result = Invoke-RestMethod -Uri "$baseUrl/OrdenesCompras/PorEstado/Pendiente" -Method GET
    if ($null -eq $result) { throw "Error en consulta por estado" }
}

# ============================================
# 9. LOTES CONTROLLER
# ============================================
Write-Host ""
Write-Host "9. Probando LotesController..." -ForegroundColor Cyan

Test-Endpoint "POST - Crear lote válido" {
    $codigo = "T$(Get-Date -Format 'HHmmss')"
    # Asegurar 6 caracteres
    if ($codigo.Length -lt 6) {
        $codigo = $codigo.PadRight(6, '0')
    }
    $body = @{
        codigo = $codigo.Substring(0, 6)
        fechaIngreso = (Get-Date).ToString("yyyy-MM-dd")
        fechaLote = (Get-Date).ToString("yyyy-MM-dd")
        fechaVencimiento = (Get-Date).AddMonths(6).ToString("yyyy-MM-dd")
        estado = "Activo"
        observaciones = $null
    } | ConvertTo-Json
    $result = Invoke-RestMethod -Uri "$baseUrl/Lotes" -Method POST -Body $body -ContentType "application/json"
    $Global:TestData.LoteId = $result.id
    if (-not $result.id) { throw "No se retornó ID" }
}

Test-Endpoint "POST - Rechazar lote con código inválido" {
    $body = @{
        codigo = "TST"
        fechaIngreso = (Get-Date).ToString("yyyy-MM-dd")
        fechaLote = (Get-Date).ToString("yyyy-MM-dd")
        fechaVencimiento = (Get-Date).AddMonths(6).ToString("yyyy-MM-dd")
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/Lotes" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "POST - Rechazar lote con fecha vencimiento anterior" {
    $body = @{
        codigo = "TST002"
        fechaIngreso = (Get-Date).ToString("yyyy-MM-dd")
        fechaLote = (Get-Date).ToString("yyyy-MM-dd")
        fechaVencimiento = (Get-Date).AddDays(-1).ToString("yyyy-MM-dd")
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/Lotes" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "GET - Obtener todos los lotes" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Lotes" -Method GET
    if (-not $result) { throw "No se obtuvieron lotes" }
}

# ============================================
# 10. LOTES TERMINADOS CONTROLLER
# ============================================
Write-Host ""
Write-Host "10. Probando LotesTerminadosController..." -ForegroundColor Cyan

Test-Endpoint "POST - Crear lote terminado válido" {
    $body = @{
        loteId = $Global:TestData.LoteId
        productoId = $Global:TestData.ProductoId
        fechaEnvasado = (Get-Date).ToString("yyyy-MM-dd")
        fechaVencimiento = (Get-Date).AddMonths(6).ToString("yyyy-MM-dd")
    } | ConvertTo-Json
    $result = Invoke-RestMethod -Uri "$baseUrl/LotesTerminados" -Method POST -Body $body -ContentType "application/json"
    $Global:TestData.LoteTerminadoId = $result.id
    if (-not $result.id) { throw "No se retornó ID" }
}

Test-Endpoint "POST - Rechazar lote terminado con lote inexistente" {
    $body = @{
        loteId = 99999
        productoId = $Global:TestData.ProductoId
        fechaEnvasado = (Get-Date).ToString("yyyy-MM-dd")
        fechaVencimiento = (Get-Date).AddMonths(6).ToString("yyyy-MM-dd")
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/LotesTerminados" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "GET - Obtener todos los lotes terminados" {
    $result = Invoke-RestMethod -Uri "$baseUrl/LotesTerminados" -Method GET
    if (-not $result) { throw "No se obtuvieron lotes terminados" }
}

# ============================================
# 11. CATACIONES CONTROLLER
# ============================================
Write-Host ""
Write-Host "11. Probando CatacionsController..." -ForegroundColor Cyan

Test-Endpoint "POST - Crear catación válida" {
    $body = @{
        loteTerminadoId = $Global:TestData.LoteTerminadoId
        puntaje = 87.5
        humedad = 12.3
        notas = "Excelente calidad"
        aprobado = $true
        fecha = (Get-Date).ToString("yyyy-MM-dd")
    } | ConvertTo-Json
    $result = Invoke-RestMethod -Uri "$baseUrl/Catacions" -Method POST -Body $body -ContentType "application/json"
    $Global:TestData.CatacionId = $result.id
    if (-not $result.id) { throw "No se retornó ID" }
}

Test-Endpoint "POST - Rechazar catación con puntaje inválido" {
    $body = @{
        loteTerminadoId = $Global:TestData.LoteTerminadoId
        puntaje = 150
        humedad = 12.3
        aprobado = $true
        fecha = (Get-Date).ToString("yyyy-MM-dd")
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/Catacions" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "POST - Rechazar catación con humedad inválida" {
    $body = @{
        loteTerminadoId = $Global:TestData.LoteTerminadoId
        puntaje = 85
        humedad = 150
        aprobado = $true
        fecha = (Get-Date).ToString("yyyy-MM-dd")
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/Catacions" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "GET - Obtener todas las cataciones" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Catacions" -Method GET
    if (-not $result) { throw "No se obtuvieron cataciones" }
}

Test-Endpoint "GET - Obtener cataciones aprobadas" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Catacions/Aprobadas" -Method GET
    if ($null -eq $result) { throw "Error en consulta de aprobadas" }
}

Test-Endpoint "GET - Obtener cataciones por lote terminado" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Catacions/PorLoteTerminado/$($Global:TestData.LoteTerminadoId)" -Method GET
    if ($null -eq $result) { throw "Error en consulta por lote" }
}

# ============================================
# 12. PEDIDOS CONTROLLER
# ============================================
Write-Host ""
Write-Host "12. Probando PedidosController..." -ForegroundColor Cyan

Test-Endpoint "POST - Crear pedido válido" {
    $body = @{
        clienteId = $Global:TestData.ClienteId
        fecha = (Get-Date).ToString("yyyy-MM-dd")
        estado = "Pendiente"
        tipo = "Delivery"
        prioritaria = $true
    } | ConvertTo-Json
    $result = Invoke-RestMethod -Uri "$baseUrl/Pedidos" -Method POST -Body $body -ContentType "application/json"
    $Global:TestData.PedidoId = $result.id
    if (-not $result.id) { throw "No se retornó ID" }
}

Test-Endpoint "POST - Rechazar pedido con estado inválido" {
    $body = @{
        clienteId = $Global:TestData.ClienteId
        fecha = (Get-Date).ToString("yyyy-MM-dd")
        estado = "EstadoInvalido"
        prioritaria = $false
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/Pedidos" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "GET - Obtener todos los pedidos" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Pedidos" -Method GET
    if (-not $result) { throw "No se obtuvieron pedidos" }
}

Test-Endpoint "GET - Obtener pedidos prioritarios" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Pedidos/Prioritarios" -Method GET
    if ($null -eq $result) { throw "Error en consulta de prioritarios" }
}

Test-Endpoint "GET - Obtener pedidos por cliente" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Pedidos/PorCliente/$($Global:TestData.ClienteId)" -Method GET
    if ($null -eq $result) { throw "Error en consulta por cliente" }
}

Test-Endpoint "GET - Obtener pedidos por estado" {
    $result = Invoke-RestMethod -Uri "$baseUrl/Pedidos/PorEstado/Pendiente" -Method GET
    if ($null -eq $result) { throw "Error en consulta por estado" }
}

# ============================================
# 13. ORDEN COMPRA TIPO GRANO CONTROLLER
# ============================================
Write-Host ""
Write-Host "13. Probando OrdenCompraTipoGranosController..." -ForegroundColor Cyan

Test-Endpoint "POST - Crear orden-tipo grano válido" {
    $body = @{
        ordenCompraId = $Global:TestData.OrdenCompraId
        tipoGranoId = $Global:TestData.TipoGranoId
        cantidadKg = 500
        precioUnitarioKg = 15.50
        precioTotal = 7750.00
    } | ConvertTo-Json
    $result = Invoke-RestMethod -Uri "$baseUrl/OrdenCompraTipoGranos" -Method POST -Body $body -ContentType "application/json"
    $Global:TestData.OrdenCompraTipoGranoId = $result.id
    if (-not $result.id) { throw "No se retornó ID" }
}

Test-Endpoint "POST - Rechazar con cantidad negativa" {
    $body = @{
        ordenCompraId = $Global:TestData.OrdenCompraId
        tipoGranoId = $Global:TestData.TipoGranoId
        cantidadKg = -10
        precioUnitarioKg = 15.50
        precioTotal = 0
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/OrdenCompraTipoGranos" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "GET - Obtener todos los registros" {
    $result = Invoke-RestMethod -Uri "$baseUrl/OrdenCompraTipoGranos" -Method GET
    if (-not $result) { throw "No se obtuvieron registros" }
}

Test-Endpoint "GET - Obtener por orden de compra" {
    $result = Invoke-RestMethod -Uri "$baseUrl/OrdenCompraTipoGranos/PorOrdenCompra/$($Global:TestData.OrdenCompraId)" -Method GET
    if ($null -eq $result) { throw "Error en consulta por orden" }
}

# ============================================
# 14. ORDEN COMPRA TIPO GRANO LOTE CONTROLLER
# ============================================
Write-Host ""
Write-Host "14. Probando OrdenCompraTipoGranoLotesController..." -ForegroundColor Cyan

Test-Endpoint "POST - Crear asignación a lote válida" {
    $body = @{
        ordenCompraTipoGranoId = $Global:TestData.OrdenCompraTipoGranoId
        loteId = $Global:TestData.LoteId
        cantidadKg = 250
    } | ConvertTo-Json
    $result = Invoke-RestMethod -Uri "$baseUrl/OrdenCompraTipoGranoLotes" -Method POST -Body $body -ContentType "application/json"
    $Global:TestData.OrdenCompraTipoGranoLoteId = $result.id
    if (-not $result.id) { throw "No se retornó ID" }
}

Test-Endpoint "POST - Rechazar con cantidad inválida" {
    $body = @{
        ordenCompraTipoGranoId = $Global:TestData.OrdenCompraTipoGranoId
        loteId = $Global:TestData.LoteId
        cantidadKg = 0
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/OrdenCompraTipoGranoLotes" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "GET - Obtener todos los registros" {
    $result = Invoke-RestMethod -Uri "$baseUrl/OrdenCompraTipoGranoLotes" -Method GET
    if (-not $result) { throw "No se obtuvieron registros" }
}

Test-Endpoint "GET - Obtener por lote" {
    $result = Invoke-RestMethod -Uri "$baseUrl/OrdenCompraTipoGranoLotes/PorLote/$($Global:TestData.LoteId)" -Method GET
    if ($null -eq $result) { throw "Error en consulta por lote" }
}

# ============================================
# 15. LOTE ETAPAS CONTROLLER
# ============================================
Write-Host ""
Write-Host "15. Probando LoteEtapasController..." -ForegroundColor Cyan

Test-Endpoint "POST - Crear etapa de lote válida" {
    $body = @{
        loteId = $Global:TestData.LoteId
        etapaId = $Global:TestData.EtapaId
        fechaInicio = (Get-Date).ToString("yyyy-MM-dd")
        fechaFin = $null
    } | ConvertTo-Json
    $result = Invoke-RestMethod -Uri "$baseUrl/LoteEtapas" -Method POST -Body $body -ContentType "application/json"
    $Global:TestData.LoteEtapaId = $result.id
    if (-not $result.id) { throw "No se retornó ID" }
}

Test-Endpoint "POST - Rechazar con fecha fin anterior" {
    $body = @{
        loteId = $Global:TestData.LoteId
        etapaId = $Global:TestData.EtapaId
        fechaInicio = (Get-Date).ToString("yyyy-MM-dd")
        fechaFin = (Get-Date).AddDays(-1).ToString("yyyy-MM-dd")
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/LoteEtapas" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "GET - Obtener todas las etapas de lotes" {
    $result = Invoke-RestMethod -Uri "$baseUrl/LoteEtapas" -Method GET
    if (-not $result) { throw "No se obtuvieron registros" }
}

Test-Endpoint "GET - Obtener etapas por lote" {
    $result = Invoke-RestMethod -Uri "$baseUrl/LoteEtapas/PorLote/$($Global:TestData.LoteId)" -Method GET
    if ($null -eq $result) { throw "Error en consulta por lote" }
}

Test-Endpoint "GET - Obtener etapas en proceso" {
    $result = Invoke-RestMethod -Uri "$baseUrl/LoteEtapas/EnProceso" -Method GET
    if ($null -eq $result) { throw "Error en consulta en proceso" }
}

# ============================================
# 16. PEDIDO LOTE TERMINADO CONTROLLER
# ============================================
Write-Host ""
Write-Host "16. Probando PedidoLoteTerminadosController..." -ForegroundColor Cyan

Test-Endpoint "POST - Crear producto en pedido válido" {
    $body = @{
        pedidoId = $Global:TestData.PedidoId
        loteTerminadoId = $Global:TestData.LoteTerminadoId
        productoId = $Global:TestData.ProductoId
        cantidad = 50
    } | ConvertTo-Json
    $result = Invoke-RestMethod -Uri "$baseUrl/PedidoLoteTerminados" -Method POST -Body $body -ContentType "application/json"
    $Global:TestData.PedidoLoteTerminadoId = $result.id
    if (-not $result.id) { throw "No se retornó ID" }
}

Test-Endpoint "POST - Rechazar con cantidad inválida" {
    $body = @{
        pedidoId = $Global:TestData.PedidoId
        loteTerminadoId = $Global:TestData.LoteTerminadoId
        productoId = $Global:TestData.ProductoId
        cantidad = 0
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/PedidoLoteTerminados" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "GET - Obtener todos los registros" {
    $result = Invoke-RestMethod -Uri "$baseUrl/PedidoLoteTerminados" -Method GET
    if (-not $result) { throw "No se obtuvieron registros" }
}

Test-Endpoint "GET - Obtener por pedido" {
    $result = Invoke-RestMethod -Uri "$baseUrl/PedidoLoteTerminados/PorPedido/$($Global:TestData.PedidoId)" -Method GET
    if ($null -eq $result) { throw "Error en consulta por pedido" }
}

# ============================================
# 17. PEDIDO RUTAS CONTROLLER
# ============================================
Write-Host ""
Write-Host "17. Probando PedidoRutasController..." -ForegroundColor Cyan

Test-Endpoint "POST - Crear asignación de ruta válida" {
    $body = @{
        pedidoId = $Global:TestData.PedidoId
        rutaId = $Global:TestData.RutaId
        fechaSalida = (Get-Date).ToString("yyyy-MM-dd")
        fechaEntrega = $null
        estado = "En Tránsito"
    } | ConvertTo-Json
    $result = Invoke-RestMethod -Uri "$baseUrl/PedidoRutas" -Method POST -Body $body -ContentType "application/json"
    $Global:TestData.PedidoRutaId = $result.id
    if (-not $result.id) { throw "No se retornó ID" }
}

Test-Endpoint "POST - Rechazar con estado inválido" {
    $body = @{
        pedidoId = $Global:TestData.PedidoId
        rutaId = $Global:TestData.RutaId
        fechaSalida = (Get-Date).ToString("yyyy-MM-dd")
        estado = "EstadoInvalido"
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/PedidoRutas" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "POST - Rechazar con fecha entrega anterior" {
    $body = @{
        pedidoId = $Global:TestData.PedidoId
        rutaId = $Global:TestData.RutaId
        fechaSalida = (Get-Date).ToString("yyyy-MM-dd")
        fechaEntrega = (Get-Date).AddDays(-1).ToString("yyyy-MM-dd")
        estado = "Pendiente"
    } | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "$baseUrl/PedidoRutas" -Method POST -Body $body -ContentType "application/json"
        throw "Debería haber fallado"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 400) { throw }
    }
}

Test-Endpoint "GET - Obtener todas las asignaciones" {
    $result = Invoke-RestMethod -Uri "$baseUrl/PedidoRutas" -Method GET
    if (-not $result) { throw "No se obtuvieron registros" }
}

Test-Endpoint "GET - Obtener por pedido" {
    $result = Invoke-RestMethod -Uri "$baseUrl/PedidoRutas/PorPedido/$($Global:TestData.PedidoId)" -Method GET
    if ($null -eq $result) { throw "Error en consulta por pedido" }
}

Test-Endpoint "GET - Obtener por ruta" {
    $result = Invoke-RestMethod -Uri "$baseUrl/PedidoRutas/PorRuta/$($Global:TestData.RutaId)" -Method GET
    if ($null -eq $result) { throw "Error en consulta por ruta" }
}

Test-Endpoint "GET - Obtener en tránsito" {
    $result = Invoke-RestMethod -Uri "$baseUrl/PedidoRutas/EnTransito" -Method GET
    if ($null -eq $result) { throw "Error en consulta en tránsito" }
}

# ============================================
# PRUEBAS DE ELIMINACIÓN (Restricciones)
# ============================================
Write-Host ""
Write-Host "18. Probando Restricciones de Eliminación..." -ForegroundColor Cyan

Test-Endpoint "DELETE - Rechazar eliminar cliente con pedidos" {
    try {
        Invoke-RestMethod -Uri "$baseUrl/Clientes/$($Global:TestData.ClienteId)" -Method DELETE
        throw "Debería haber fallado (cliente tiene pedidos)"
    } catch {
        # Debe retornar 400 porque tiene pedidos
        if ($_.Exception.Response.StatusCode -ne 400) { throw "Código de estado incorrecto: $($_.Exception.Response.StatusCode)" }
    }
}

Test-Endpoint "DELETE - Rechazar eliminar producto con lotes" {
    try {
        Invoke-RestMethod -Uri "$baseUrl/Productos/$($Global:TestData.ProductoId)" -Method DELETE
        throw "Debería haber fallado (producto tiene lotes)"
    } catch {
        # Debe retornar 400 porque tiene lotes terminados
        if ($_.Exception.Response.StatusCode -ne 400) { throw "Código de estado incorrecto: $($_.Exception.Response.StatusCode)" }
    }
}

Test-Endpoint "DELETE - Rechazar eliminar proveedor con órdenes" {
    try {
        Invoke-RestMethod -Uri "$baseUrl/Proveedores/$($Global:TestData.ProveedorId)" -Method DELETE
        throw "Debería haber fallado (proveedor tiene órdenes)"
    } catch {
        # Debe retornar 400 porque tiene órdenes de compra
        if ($_.Exception.Response.StatusCode -ne 400) { throw "Código de estado incorrecto: $($_.Exception.Response.StatusCode)" }
    }
}

Test-Endpoint "DELETE - Rechazar eliminar lote con lotes terminados" {
    try {
        Invoke-RestMethod -Uri "$baseUrl/Lotes/$($Global:TestData.LoteId)" -Method DELETE
        throw "Debería haber fallado (lote tiene lotes terminados)"
    } catch {
        # Debe retornar 400 porque tiene lotes terminados
        if ($_.Exception.Response.StatusCode -ne 400) { throw "Código de estado incorrecto: $($_.Exception.Response.StatusCode)" }
    }
}

Test-Endpoint "DELETE - Rechazar eliminar ruta con pedidos" {
    try {
        Invoke-RestMethod -Uri "$baseUrl/Rutas/$($Global:TestData.RutaId)" -Method DELETE
        throw "Debería haber fallado (ruta tiene pedidos)"
    } catch {
        # Debe retornar 400 porque tiene pedidos asociados
        if ($_.Exception.Response.StatusCode -ne 400) { throw "Código de estado incorrecto: $($_.Exception.Response.StatusCode)" }
    }
}

# ============================================
# RESUMEN FINAL
# ============================================
Write-Host ""
Write-Host "=====================================================" -ForegroundColor Cyan
Write-Host "                  RESUMEN DE PRUEBAS" -ForegroundColor Green
Write-Host "=====================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Total de pruebas ejecutadas: $($testResults.Total)" -ForegroundColor White
Write-Host "Pruebas exitosas: $($testResults.Passed)" -ForegroundColor Green
Write-Host "Pruebas fallidas: $($testResults.Failed)" -ForegroundColor Red
Write-Host ""

$percentage = [math]::Round(($testResults.Passed / $testResults.Total) * 100, 2)
Write-Host "Porcentaje de éxito: $percentage%" -ForegroundColor $(if ($percentage -ge 90) { "Green" } elseif ($percentage -ge 70) { "Yellow" } else { "Red" })
Write-Host ""

if ($testResults.Failed -eq 0) {
    Write-Host "?? ¡TODAS LAS PRUEBAS PASARON EXITOSAMENTE!" -ForegroundColor Green
} else {
    Write-Host "??  Algunas pruebas fallaron. Revisa los detalles arriba." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Datos de prueba creados:" -ForegroundColor Yellow
Write-Host "  - Proveedor ID: $($Global:TestData.ProveedorId)" -ForegroundColor White
Write-Host "  - Cliente ID: $($Global:TestData.ClienteId)" -ForegroundColor White
Write-Host "  - Presentación ID: $($Global:TestData.PresentacionId)" -ForegroundColor White
Write-Host "  - Producto ID: $($Global:TestData.ProductoId)" -ForegroundColor White
Write-Host "  - Tipo Grano ID: $($Global:TestData.TipoGranoId)" -ForegroundColor White
Write-Host "  - Etapa ID: $($Global:TestData.EtapaId)" -ForegroundColor White
Write-Host "  - Ruta ID: $($Global:TestData.RutaId)" -ForegroundColor White
Write-Host "  - Orden Compra ID: $($Global:TestData.OrdenCompraId)" -ForegroundColor White
Write-Host "  - Lote ID: $($Global:TestData.LoteId)" -ForegroundColor White
Write-Host "  - Lote Terminado ID: $($Global:TestData.LoteTerminadoId)" -ForegroundColor White
Write-Host "  - Catación ID: $($Global:TestData.CatacionId)" -ForegroundColor White
Write-Host "  - Pedido ID: $($Global:TestData.PedidoId)" -ForegroundColor White
Write-Host ""
Write-Host "=====================================================" -ForegroundColor Cyan
