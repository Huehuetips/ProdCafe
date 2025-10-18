# ?? API de Gestión de Producción de Café

API REST desarrollada en .NET 9 para gestión integral de producción y distribución de café.

## ?? Requisitos Previos

- .NET 9 SDK
- SQL Server (Local o Express)
- Visual Studio 2022 / VS Code / Rider

## ??? Base de Datos

La aplicación utiliza SQL Server con la base de datos `CafeProd`.

**Cadena de conexión** (en `appsettings.json`):
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CafeProd;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

## ??? Estructura del Modelo de Datos

### Entidades Principales (12)
- **Proveedores** - Proveedores de granos de café
- **TiposGrano** - Tipos de grano (Arábica, Robusta, Blends)
- **OrdenesCompra** - Órdenes de compra a proveedores
- **Lotes** - Lotes de materia prima
- **Etapas** - Etapas del proceso (Tostado, Molienda, Empaque)
- **Presentaciones** - Tipos de presentación de productos
- **Productos** - Productos finales de café
- **LotesTerminados** - Lotes de productos terminados
- **Catacion** - Evaluación de calidad de lotes
- **Clientes** - Clientes del negocio
- **Pedidos** - Pedidos de clientes
- **Rutas** - Rutas de distribución

### Tablas de Relación (5)
- **OrdenCompraTipoGrano** - Relación entre órdenes y tipos de grano
- **OrdenCompraTipoGranoLote** - Asignación de granos a lotes
- **LoteEtapa** - Seguimiento de etapas de producción
- **PedidoLoteTerminado** - Productos en pedidos
- **PedidoRuta** - Asignación de rutas a pedidos

## ?? Inicio Rápido

### 1. Configurar la Base de Datos

```powershell
# Instalar herramientas EF Core (si no las tienes)
dotnet tool install --global dotnet-ef

# Crear migración
dotnet ef migrations add InitialCreate

# Crear base de datos
dotnet ef database update
```

### 2. Iniciar la Aplicación

**Opción A: Usando el script**
```powershell
.\start-api.ps1
```

**Opción B: Manualmente**
```powershell
dotnet run --launch-profile http
```

La API estará disponible en: **http://localhost:5190**

### 3. Probar la API

**Opción A: Swagger UI (Recomendado)**
- Abrir navegador en: http://localhost:5190
- Interfaz gráfica para probar todos los endpoints

**Opción B: Usando el script de prueba**
```powershell
.\test-api.ps1
```

## ?? Endpoints Principales

Todos los endpoints siguen el patrón REST estándar:

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/Proveedores` | Obtener todos los proveedores |
| GET | `/api/Proveedores/{id}` | Obtener proveedor por ID |
| POST | `/api/Proveedores` | Crear nuevo proveedor |
| PUT | `/api/Proveedores/{id}` | Actualizar proveedor |
| DELETE | `/api/Proveedores/{id}` | Eliminar proveedor |

Los mismos endpoints existen para:
- `/api/Clientes`
- `/api/Productos`
- `/api/Pedidos`
- `/api/OrdenesCompras`
- `/api/Lotes`
- `/api/Rutas`
- `/api/TiposGranos`
- `/api/Etapas`
- `/api/Presentaciones`
- `/api/LotesTerminados`
- `/api/Cataciones`
- Y todas las tablas intermedias...

## ?? Configuración

### CORS
Por defecto, la API permite peticiones desde cualquier origen (útil para desarrollo).

Para producción, modifica en `Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecific", policy =>
    {
        policy.WithOrigins("https://tu-frontend.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### HTTPS
Actualmente deshabilitado para facilitar desarrollo local. Para habilitarlo:

1. Descomentar en `Program.cs`:
   ```csharp
   app.UseHttpsRedirection();
   ```

2. Usar el perfil HTTPS:
   ```powershell
   dotnet run --launch-profile https
   ```

## ?? Ejemplos de Uso

### Crear un Proveedor
```powershell
$proveedor = @{
    nombre = "Café Colombiano S.A."
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5190/api/Proveedores" `
    -Method POST `
    -Body $proveedor `
    -ContentType "application/json"
```

### Obtener todos los Clientes
```powershell
Invoke-RestMethod -Uri "http://localhost:5190/api/Clientes" -Method GET
```

### Crear un Producto
```powershell
$producto = @{
    nombre = "Café Premium Molido"
    presentacionId = 1
    nivelTostado = "Medio"
    tipoMolido = "Fino"
    precio = 15.99
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5190/api/Productos" `
    -Method POST `
    -Body $producto `
    -ContentType "application/json"
```

## ??? Desarrollo

### Agregar una nueva migración
```powershell
dotnet ef migrations add NombreDeLaMigracion
dotnet ef database update
```

### Revertir una migración
```powershell
dotnet ef migrations remove
```

### Ver el estado de las migraciones
```powershell
dotnet ef migrations list
```

### Compilar el proyecto
```powershell
dotnet build
```

### Limpiar y recompilar
```powershell
dotnet clean
dotnet build
```

## ?? Solución de Problemas

### Error: "Failed to determine the https port for redirect"
- Usa el perfil HTTP: `dotnet run --launch-profile http`
- O descomenta `app.UseHttpsRedirection()` en `Program.cs`

### Error: "Cannot open database"
- Verifica que SQL Server esté corriendo
- Revisa la cadena de conexión en `appsettings.json`
- Ejecuta `dotnet ef database update`

### Error: "Port already in use"
- Cambia el puerto en `Properties/launchSettings.json`
- O detén la aplicación que está usando el puerto 5190

## ?? Tecnologías Utilizadas

- **.NET 9** - Framework principal
- **Entity Framework Core 9** - ORM
- **SQL Server** - Base de datos
- **Swashbuckle** - Documentación API (Swagger)
- **ASP.NET Core** - Web API

## ?? Notas

- La API está configurada para **desarrollo** con CORS abierto
- Los ciclos de referencia JSON están manejados automáticamente
- Las eliminaciones están configuradas con `Restrict` para evitar cascadas accidentales
- Swagger UI está en la raíz: http://localhost:5190

## ?? Soporte

Si tienes problemas:
1. Verifica que SQL Server esté corriendo
2. Comprueba la cadena de conexión
3. Asegúrate de haber ejecutado las migraciones
4. Revisa los logs en la consola

---

¡Happy Coding! ?
