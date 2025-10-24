# ?? Gu�a de Validaciones y Uso de la API

## ?? Resumen de Controladores Actualizados

Se han agregado validaciones completas, manejo de errores y funcionalidades mejoradas a los siguientes controladores:

### ? Controladores con Validaciones Completas

1. **ClientesController**
2. **ProductosController**
3. **CatacionsController**
4. **PedidosController**
5. **OrdenesComprasController**

---

## ?? 1. ClientesController (`/api/Clientes`)

### Validaciones Implementadas:

#### POST - Crear Cliente
```json
{
  "nombre": "Caf� Express",          // REQUERIDO - No puede estar vac�o
  "tipo": "Mayorista",               // Opcional
  "contacto": "Juan P�rez",          // Opcional
  "email": "contacto@cafe.com",      // Opcional - Validaci�n de formato
  "telefono": "555-1234",            // Opcional
  "direccion": "Calle Principal 123" // Opcional
}
```

**Validaciones:**
- ? `nombre` es requerido y no puede estar vac�o
- ? `email` debe tener formato v�lido
- ? No puede haber dos clientes con el mismo email
- ? ModelState v�lido

**Respuestas:**
- `201 Created` - Cliente creado exitosamente
- `400 Bad Request` - Validaciones fallidas
- `500 Internal Server Error` - Error del servidor

#### GET - Obtener Todos
```http
GET /api/Clientes
```
**Incluye:**
- Lista de clientes con sus pedidos asociados

#### GET - Obtener por ID
```http
GET /api/Clientes/5
```
**Incluye:**
- Cliente con sus pedidos asociados

**Validaciones:**
- ? ID debe ser mayor que 0
- ? Cliente debe existir

#### PUT - Actualizar Cliente
```json
{
  "id": 1,
  "nombre": "Caf� Express Actualizado",
  "tipo": "Mayorista",
  "email": "nuevo@email.com"
}
```

**Validaciones:**
- ? ID de URL debe coincidir con ID del body
- ? Nombre requerido
- ? Email con formato v�lido
- ? Cliente debe existir

#### DELETE - Eliminar Cliente
```http
DELETE /api/Clientes/5
```

**Validaciones:**
- ? ID debe ser mayor que 0
- ? Cliente debe existir
- ? No puede tener pedidos asociados
- ?? Si tiene pedidos, retorna error 400 con cantidad de pedidos

---

## ?? 2. ProductosController (`/api/Productos`)

### Validaciones Implementadas:

#### POST - Crear Producto
```json
{
  "nombre": "Caf� Premium Molido",    // REQUERIDO
  "presentacionId": 1,                // REQUERIDO - Debe existir
  "nivelTostado": "Medio",            // Opcional
  "tipoMolido": "Fino",               // Opcional
  "precio": 15.99                     // REQUERIDO - No puede ser negativo
}
```

**Validaciones:**
- ? `nombre` es requerido
- ? `presentacionId` debe existir en la tabla Presentaciones
- ? `precio` no puede ser negativo
- ? No puede existir producto con mismo nombre y presentaci�n
- ? ModelState v�lido

#### GET - Obtener Todos
```http
GET /api/Productos
```
**Incluye:**
- Productos con su presentaci�n asociada

#### GET - Obtener por ID
```http
GET /api/Productos/5
```
**Incluye:**
- Producto con presentaci�n y lotes terminados

#### PUT - Actualizar Producto
**Validaciones:**
- ? ID de URL debe coincidir
- ? Nombre requerido
- ? Precio no negativo
- ? Presentaci�n debe existir
- ? Producto debe existir

#### DELETE - Eliminar Producto
**Validaciones:**
- ? ID debe ser mayor que 0
- ? Producto debe existir
- ? No puede tener lotes terminados asociados
- ? No puede tener pedidos asociados
- ?? Retorna cantidad de lotes y pedidos si hay restricciones

---

## ?? 3. CatacionsController (`/api/Catacions`)

### Endpoints Adicionales:

#### GET - Todas las Cataciones
```http
GET /api/Catacions
```
**Incluye:**
- Cataciones con lote terminado, producto y lote

#### GET - Por ID
```http
GET /api/Catacions/5
```
**Incluye:**
- Cataci�n completa con todas las relaciones

#### GET - Por Lote Terminado
```http
GET /api/Catacions/PorLoteTerminado/5
```
**Retorna:**
- Todas las cataciones de un lote espec�fico

#### GET - Cataciones Aprobadas
```http
GET /api/Catacions/Aprobadas
```
**Retorna:**
- Solo cataciones con `aprobado = true`

### POST - Crear Cataci�n
```json
{
  "loteTerminadoId": 1,         // REQUERIDO - Debe existir
  "puntaje": 85.5,              // REQUERIDO - Entre 0 y 100
  "humedad": 12.3,              // REQUERIDO - Entre 0 y 100
  "notas": "Excelente aroma",   // Opcional
  "aprobado": true,             // REQUERIDO
  "fecha": "2024-10-18"         // REQUERIDO - No puede ser futura
}
```

**Validaciones:**
- ? `loteTerminadoId` debe existir
- ? `puntaje` entre 0 y 100
- ? `humedad` entre 0 y 100
- ? `fecha` no puede ser futura
- ? ModelState v�lido

---

## ?? 4. PedidosController (`/api/Pedidos`)

### Endpoints Adicionales:

#### GET - Todos los Pedidos
```http
GET /api/Pedidos
```
**Incluye:**
- Cliente, productos y rutas asociadas

#### GET - Por Cliente
```http
GET /api/Pedidos/PorCliente/5
```
**Retorna:**
- Todos los pedidos de un cliente espec�fico

#### GET - Prioritarios
```http
GET /api/Pedidos/Prioritarios
```
**Retorna:**
- Solo pedidos con `prioritaria = true`, ordenados por fecha

#### GET - Por Estado
```http
GET /api/Pedidos/PorEstado/Pendiente
```
**Retorna:**
- Pedidos filtrados por estado
**Estados v�lidos:** Pendiente, En Proceso, Listo, Entregado, Cancelado

### POST - Crear Pedido
```json
{
  "clienteId": 1,               // REQUERIDO - Debe existir
  "fecha": "2024-10-18",        // REQUERIDO
  "estado": "Pendiente",        // Opcional - Valores v�lidos
  "tipo": "Delivery",           // Opcional
  "prioritaria": false          // REQUERIDO
}
```

**Validaciones:**
- ? `clienteId` debe existir
- ? `estado` debe ser uno de: Pendiente, En Proceso, Listo, Entregado, Cancelado
- ? Si no se especifica estado, se establece como "Pendiente"
- ? ModelState v�lido

#### DELETE - Eliminar Pedido
**Validaciones:**
- ? No puede tener productos asociados
- ? No puede tener rutas asociadas
- ?? Retorna cantidad de productos y rutas si hay restricciones

---

## ?? 5. OrdenesComprasController (`/api/OrdenesCompras`)

### Endpoints Adicionales:

#### GET - Todas las �rdenes
```http
GET /api/OrdenesCompras
```
**Incluye:**
- Proveedor y tipos de grano asociados

#### GET - Por Proveedor
```http
GET /api/OrdenesCompras/PorProveedor/5
```
**Retorna:**
- Todas las �rdenes de un proveedor espec�fico

#### GET - Por Estado
```http
GET /api/OrdenesCompras/PorEstado/Pendiente
```
**Retorna:**
- �rdenes filtradas por estado
**Estados v�lidos:** Pendiente, Enviada, En Tr�nsito, Recibida, Cancelada

### POST - Crear Orden de Compra
```json
{
  "proveedorId": 1,             // REQUERIDO - Debe existir
  "estado": "Pendiente",        // Opcional - Valores v�lidos
  "fechaEmision": "2024-10-18", // REQUERIDO
  "fechaRecepcion": null        // Opcional - No puede ser < fechaEmision
}
```

**Validaciones:**
- ? `proveedorId` debe existir
- ? `estado` debe ser uno de: Pendiente, Enviada, En Tr�nsito, Recibida, Cancelada
- ? Si no se especifica estado, se establece como "Pendiente"
- ? `fechaRecepcion` no puede ser anterior a `fechaEmision`
- ? ModelState v�lido

#### DELETE - Eliminar Orden
**Validaciones:**
- ? No puede tener tipos de grano asociados
- ?? Retorna cantidad de tipos de grano si hay restricciones

---

## ?? Caracter�sticas Comunes en Todos los Controladores

### 1. C�digos de Respuesta HTTP Apropiados
- `200 OK` - Operaci�n exitosa
- `201 Created` - Recurso creado (POST)
- `400 Bad Request` - Validaci�n fallida
- `404 Not Found` - Recurso no encontrado
- `500 Internal Server Error` - Error del servidor

### 2. Mensajes de Error Descriptivos
```json
{
  "message": "El nombre del cliente es requerido"
}
```

```json
{
  "message": "No se puede eliminar el cliente porque tiene pedidos asociados",
  "pedidosCount": 5
}
```

### 3. Inclusi�n de Entidades Relacionadas
Todos los GET incluyen las entidades relacionadas mediante `.Include()` y `.ThenInclude()`

### 4. Validaci�n de IDs
- IDs deben ser mayores que 0
- IDs en URL deben coincidir con body en PUT

### 5. Manejo de Excepciones
- Try-Catch en todos los m�todos
- Mensajes de error descriptivos
- Log del error original

---

## ?? Ejemplos de Uso con PowerShell

### Crear un Cliente
```powershell
$cliente = @{
    nombre = "Caf� Express"
    tipo = "Mayorista"
    email = "contacto@cafeexpress.com"
    telefono = "555-1234"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5190/api/Clientes" `
    -Method POST `
    -Body $cliente `
    -ContentType "application/json"
```

### Obtener Pedidos Prioritarios
```powershell
Invoke-RestMethod -Uri "http://localhost:5190/api/Pedidos/Prioritarios" -Method GET
```

### Actualizar Producto
```powershell
$producto = @{
    id = 1
    nombre = "Caf� Premium Actualizado"
    presentacionId = 1
    precio = 19.99
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5190/api/Productos/1" `
    -Method PUT `
    -Body $producto `
    -ContentType "application/json"
```

### Obtener Cataciones de un Lote
```powershell
Invoke-RestMethod -Uri "http://localhost:5190/api/Catacions/PorLoteTerminado/1" -Method GET
```

---

## ?? Errores Comunes y Soluciones

### Error: "El ID debe ser mayor que 0"
**Soluci�n:** Verifica que est�s enviando un ID v�lido en la URL

### Error: "Ya existe un cliente con ese email"
**Soluci�n:** Usa un email diferente o actualiza el cliente existente

### Error: "La presentaci�n con ID X no existe"
**Soluci�n:** Crea primero la presentaci�n o usa un ID v�lido

### Error: "No se puede eliminar porque tiene X asociados"
**Soluci�n:** Elimina primero las entidades relacionadas o usa soft delete

### Error: "El puntaje debe estar entre 0 y 100"
**Soluci�n:** Verifica que los valores num�ricos est�n en el rango correcto

---

## ?? Pr�ximos Pasos

Para completar las validaciones en TODOS los controladores, se recomienda:

1. ? Aplicar el mismo patr�n a los controladores restantes:
   - ProveedoresController
   - PresentacionesController
   - RutasController
   - LotesController
   - EtapasController
   - TiposGranosController
   - Y tablas intermedias

2. ? Agregar endpoints adicionales seg�n necesidad de negocio

3. ? Implementar paginaci�n para listas grandes

4. ? Agregar filtros de b�squeda y ordenamiento

5. ? Implementar autenticaci�n y autorizaci�n (JWT)

6. ? Agregar logging con ILogger

7. ? Implementar DTOs para separar modelos de base de datos de API

---

�Necesitas ayuda con alg�n controlador espec�fico o quieres que contin�e con los dem�s?
