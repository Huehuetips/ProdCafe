# ?? API de Gesti�n de Producci�n de Caf�

API REST desarrollada en .NET 9 para gesti�n integral de producci�n y distribuci�n de caf�.

## ?? Requisitos Previos

- .NET 9 SDK
- SQL Server (Local o Express)
- Visual Studio 2022 / VS Code / Rider

## ??? Base de Datos

La aplicaci�n utiliza SQL Server con la base de datos `CafeProd`.

**Cadena de conexi�n** (en `appsettings.json`):
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CafeProd;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

## ??? Estructura del Modelo de Datos

### Entidades Principales (12)
- **Proveedores** - Proveedores de granos de caf�
- **TiposGrano** - Tipos de grano (Ar�bica, Robusta, Blends)
- **OrdenesCompra** - �rdenes de compra a proveedores
- **Lotes** - Lotes de materia prima
- **Etapas** - Etapas del proceso (Tostado, Molienda, Empaque)
- **Presentaciones** - Tipos de presentaci�n de productos
- **Productos** - Productos finales de caf�
- **LotesTerminados** - Lotes de productos terminados
- **Catacion** - Evaluaci�n de calidad de lotes
- **Clientes** - Clientes del negocio
- **Pedidos** - Pedidos de clientes
- **Rutas** - Rutas de distribuci�n

### Tablas de Relaci�n (5)
- **OrdenCompraTipoGrano** - Relaci�n entre �rdenes y tipos de grano
- **OrdenCompraTipoGranoLote** - Asignaci�n de granos a lotes
- **LoteEtapa** - Seguimiento de etapas de producci�n
- **PedidoLoteTerminado** - Productos en pedidos
- **PedidoRuta** - Asignaci�n de rutas a pedidos

## ?? Inicio R�pido

### 1. Configurar la Base de Datos

```powershell
# Instalar herramientas EF Core (si no las tienes)
dotnet tool install --global dotnet-ef

# Crear migraci�n
dotnet ef migrations add InitialCreate

# Crear base de datos
dotnet ef database update
```

### 2. Iniciar la Aplicaci�n

**Opci�n A: Usando el script**
```powershell
.\start-api.ps1
```

**Opci�n B: Manualmente**
```powershell
dotnet run --launch-profile http
```

La API estar� disponible en: **http://localhost:5190**

### 3. Probar la API

**Opci�n A: Swagger UI (Recomendado)**
- Abrir navegador en: http://localhost:5190
- Interfaz gr�fica para probar todos los endpoints

**Opci�n B: Usando el script de prueba**
```powershell
.\test-api.ps1
```

## ?? Endpoints Principales

Todos los endpoints siguen el patr�n REST est�ndar:

| M�todo | Endpoint | Descripci�n |
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

## ?? Configuraci�n

### CORS
Por defecto, la API permite peticiones desde cualquier origen (�til para desarrollo).

Para producci�n, modifica en `Program.cs`:
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
    nombre = "Caf� Colombiano S.A."
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
    nombre = "Caf� Premium Molido"
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

### Agregar una nueva migraci�n
```powershell
dotnet ef migrations add NombreDeLaMigracion
dotnet ef database update
```

### Revertir una migraci�n
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

## ?? Soluci�n de Problemas

### Error: "Failed to determine the https port for redirect"
- Usa el perfil HTTP: `dotnet run --launch-profile http`
- O descomenta `app.UseHttpsRedirection()` en `Program.cs`

### Error: "Cannot open database"
- Verifica que SQL Server est� corriendo
- Revisa la cadena de conexi�n en `appsettings.json`
- Ejecuta `dotnet ef database update`

### Error: "Port already in use"
- Cambia el puerto en `Properties/launchSettings.json`
- O det�n la aplicaci�n que est� usando el puerto 5190

## ?? Tecnolog�as Utilizadas

- **.NET 9** - Framework principal
- **Entity Framework Core 9** - ORM
- **SQL Server** - Base de datos
- **Swashbuckle** - Documentaci�n API (Swagger)
- **ASP.NET Core** - Web API

## ?? Notas

- La API est� configurada para **desarrollo** con CORS abierto
- Los ciclos de referencia JSON est�n manejados autom�ticamente
- Las eliminaciones est�n configuradas con `Restrict` para evitar cascadas accidentales
- Swagger UI est� en la ra�z: http://localhost:5190

## ?? Soporte

Si tienes problemas:
1. Verifica que SQL Server est� corriendo
2. Comprueba la cadena de conexi�n
3. Aseg�rate de haber ejecutado las migraciones
4. Revisa los logs en la consola

---

�Happy Coding! ?
