# ?? Guía de Inicialización Completa

## ?? Índice
1. [Requisitos Previos](#requisitos-previos)
2. [Inicialización Automática](#inicialización-automática)
3. [Inicialización Manual](#inicialización-manual)
4. [Verificación](#verificación)
5. [Solución de Problemas](#solución-de-problemas)

---

## ? Requisitos Previos

Antes de comenzar, asegúrate de tener:

- ? **.NET 9 SDK** instalado
- ? **SQL Server** instalado y ejecutándose
- ? **PowerShell** (viene con Windows)
- ? **Git** (opcional, para clonar el repositorio)

### Verificar instalaciones:

```powershell
# Verificar .NET
dotnet --version
# Debe mostrar: 9.0.x

# Verificar SQL Server (si usas instancia local)
# Abre SQL Server Configuration Manager y verifica que el servicio esté corriendo
```

---

## ?? Inicialización Automática (RECOMENDADO)

### Opción 1: Script Todo-en-Uno ?

```powershell
.\inicializar.ps1
```

Este script hará automáticamente:
1. ? Verificar .NET instalado
2. ? Eliminar base de datos existente (opcional)
3. ? Crear/aplicar migraciones
4. ? Compilar el proyecto
5. ? Iniciar la API
6. ? Cargar datos de prueba (opcional)

**Salida esperada:**
```
============================================
   INICIALIZACIÓN COMPLETA - API CAFÉ
============================================

Paso 1: Verificando .NET...
? .NET versión: 9.0.0

Paso 2: Base de Datos
¿Deseas eliminar la base de datos existente? (S/N): S
? Base de datos eliminada

Paso 3: Configurando Migraciones...
? Base de datos creada y migración aplicada

Paso 4: Compilando proyecto...
? Proyecto compilado exitosamente

Paso 5: Iniciando API...
? API iniciada (PID: 12345)

Paso 6: Verificando conexión con la API...
? API respondiendo correctamente

Paso 7: Cargando datos de prueba...
¿Deseas cargar datos de prueba? (S/N): S
? Todos los datos fueron creados exitosamente!

============================================
         INICIALIZACIÓN COMPLETADA
============================================

? Base de datos configurada
? Migraciones aplicadas
? API ejecutándose en http://localhost:5190
? Swagger disponible en http://localhost:5190
? Datos de prueba cargados
```

---

## ?? Inicialización Manual

Si prefieres hacerlo paso a paso:

### **Paso 1: Limpiar Base de Datos (Opcional)**

```powershell
# Eliminar base de datos existente
dotnet ef database drop --force
```

### **Paso 2: Crear Migraciones**

```powershell
# Si NO tienes carpeta Migrations, crea la migración inicial:
dotnet ef migrations add InitialCreate

# Si YA tienes migraciones, solo actualiza:
dotnet ef database update
```

**Salida esperada:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:05.23
Done.
```

### **Paso 3: Verificar Tablas Creadas**

Abre **SQL Server Management Studio (SSMS)** o **Azure Data Studio** y verifica que se crearon las tablas:

```sql
-- Ver todas las tablas
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
```

**Tablas esperadas (17):**
1. `catacion`
2. `clientes`
3. `etapas`
4. `lote_etapa`
5. `lotes`
6. `lotesTerminados`
7. `ordenCompra_tipoGrano`
8. `ordenCompra_tipoGrano_lote`
9. `ordenesCompra`
10. `pedido_loteTerminado`
11. `pedido_ruta`
12. `pedidos`
13. `presentaciones`
14. `productos`
15. `proveedores`
16. `rutas`
17. `tiposGrano`

### **Paso 4: Iniciar la API**

```powershell
# Opción A: Con script
.\start-api.ps1

# Opción B: Manualmente
dotnet run --launch-profile http
```

**Salida esperada:**
```
Compilando...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5190
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### **Paso 5: Verificar API Funcionando**

Abre el navegador en: **http://localhost:5190**

Deberías ver **Swagger UI** con todos los endpoints.

### **Paso 6: Cargar Datos de Prueba**

```powershell
# En otra terminal PowerShell
.\diagnostico-completo.ps1
```

**Salida esperada:**
```
1. Creando Proveedor...
   ? Proveedor creado: ID = 1

2. Creando Cliente...
   ? Cliente creado: ID = 1

3. Creando Presentación...
   ? Presentación creada: ID = 1

[... más creaciones ...]

? Todos los datos fueron creados exitosamente!
```

---

## ? Verificación

### 1. Verificar Base de Datos

```sql
-- Contar registros en cada tabla
SELECT 'Proveedores' AS Tabla, COUNT(*) AS Registros FROM proveedores
UNION ALL
SELECT 'Clientes', COUNT(*) FROM clientes
UNION ALL
SELECT 'Productos', COUNT(*) FROM productos
UNION ALL
SELECT 'Lotes', COUNT(*) FROM lotes
UNION ALL
SELECT 'Pedidos', COUNT(*) FROM pedidos;
```

### 2. Verificar API con PowerShell

```powershell
# Obtener todos los proveedores
Invoke-RestMethod -Uri "http://localhost:5190/api/Proveedores" -Method GET

# Obtener todos los clientes
Invoke-RestMethod -Uri "http://localhost:5190/api/Clientes" -Method GET

# Obtener todos los productos
Invoke-RestMethod -Uri "http://localhost:5190/api/Productos" -Method GET
```

### 3. Verificar en Swagger

1. Abre: **http://localhost:5190**
2. Expande cualquier endpoint GET
3. Haz clic en **"Try it out"**
4. Haz clic en **"Execute"**
5. Verifica que obtienes datos

---

## ?? Solución de Problemas

### **Error: "dotnet command not found"**

**Causa:** .NET SDK no está instalado.

**Solución:**
```powershell
# Descargar e instalar .NET 9 SDK desde:
# https://dotnet.microsoft.com/download/dotnet/9.0
```

---

### **Error: "Cannot connect to SQL Server"**

**Causa:** SQL Server no está ejecutándose o la cadena de conexión es incorrecta.

**Solución:**

1. Verifica que SQL Server esté corriendo:
   - Abre **SQL Server Configuration Manager**
   - Verifica que el servicio **SQL Server (MSSQLSERVER)** esté **Running**

2. Verifica la cadena de conexión en `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CafeDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

3. Si usas SQL Server con usuario/contraseña:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CafeDB;User Id=sa;Password=TuContraseña;TrustServerCertificate=True;"
  }
}
```

---

### **Error: "Migration already exists"**

**Causa:** Ya existe una migración con ese nombre.

**Solución:**
```powershell
# Opción 1: Aplicar migraciones existentes
dotnet ef database update

# Opción 2: Eliminar migraciones y empezar de nuevo
Remove-Item -Path ".\Migrations\*" -Recurse -Force
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

### **Error: "Port 5190 is already in use"**

**Causa:** Ya hay una instancia de la API ejecutándose.

**Solución:**
```powershell
# Detener todos los procesos de la API
Get-Process | Where-Object {$_.ProcessName -like "*ApiEjemplo*"} | Stop-Process -Force

# O cambiar el puerto en launchSettings.json
```

---

### **Error: "Database update failed"**

**Causa:** Error en la migración o base de datos bloqueada.

**Solución:**
```powershell
# 1. Eliminar base de datos
dotnet ef database drop --force

# 2. Recrear desde cero
dotnet ef database update

# 3. Si persiste, verificar logs de SQL Server
```

---

### **Error: "The API does not respond"**

**Causa:** La API no terminó de iniciar o hay un error en el código.

**Solución:**

1. Verifica los logs en la consola donde ejecutaste la API
2. Espera 10-15 segundos después de iniciar
3. Verifica que no haya errores de compilación:
```powershell
dotnet build
```

---

## ?? Flujo Visual de Inicialización

```
???????????????????????????????????????????????????
?  1. Verificar .NET y SQL Server                 ?
???????????????????????????????????????????????????
                  ?
                  ?
???????????????????????????????????????????????????
?  2. Eliminar Base de Datos (Opcional)           ?
?     dotnet ef database drop --force             ?
???????????????????????????????????????????????????
                  ?
                  ?
???????????????????????????????????????????????????
?  3. Crear/Aplicar Migraciones                   ?
?     dotnet ef migrations add InitialCreate      ?
?     dotnet ef database update                   ?
???????????????????????????????????????????????????
                  ?
                  ?
???????????????????????????????????????????????????
?  4. Compilar Proyecto                           ?
?     dotnet build                                ?
???????????????????????????????????????????????????
                  ?
                  ?
???????????????????????????????????????????????????
?  5. Iniciar API                                 ?
?     dotnet run --launch-profile http            ?
???????????????????????????????????????????????????
                  ?
                  ?
???????????????????????????????????????????????????
?  6. Cargar Datos de Prueba                      ?
?     .\diagnostico-completo.ps1                  ?
???????????????????????????????????????????????????
                  ?
                  ?
???????????????????????????????????????????????????
?  ? Sistema Listo                               ?
?     - Base de datos con 17 tablas               ?
?     - API en http://localhost:5190              ?
?     - Swagger disponible                        ?
?     - Datos de prueba cargados                  ?
???????????????????????????????????????????????????
```

---

## ?? Comandos de Referencia Rápida

### Inicialización Completa
```powershell
.\inicializar.ps1
```

### Resetear Base de Datos
```powershell
dotnet ef database drop --force
dotnet ef database update
```

### Iniciar API
```powershell
.\start-api.ps1
# o
dotnet run --launch-profile http
```

### Cargar Datos
```powershell
.\diagnostico-completo.ps1
```

### Ejecutar Pruebas
```powershell
.\test-completo.ps1
```

### Ver Swagger
```
http://localhost:5190
```

---

## ?? Checklist de Inicialización

- [ ] .NET 9 SDK instalado
- [ ] SQL Server ejecutándose
- [ ] Cadena de conexión configurada en `appsettings.json`
- [ ] Migraciones creadas (`dotnet ef migrations add InitialCreate`)
- [ ] Base de datos actualizada (`dotnet ef database update`)
- [ ] Proyecto compilado sin errores (`dotnet build`)
- [ ] API iniciada (`dotnet run --launch-profile http`)
- [ ] Swagger accesible en http://localhost:5190
- [ ] Datos de prueba cargados (`.\diagnostico-completo.ps1`)
- [ ] Endpoints funcionando correctamente

---

## ?? ¡Listo!

Tu API de Café está completamente configurada y lista para usar.

**Siguiente paso recomendado:**
```powershell
# Ejecutar pruebas completas
.\test-completo.ps1
```

---

**¿Necesitas ayuda?** Revisa la sección de [Solución de Problemas](#solución-de-problemas) o abre un issue en el repositorio.
