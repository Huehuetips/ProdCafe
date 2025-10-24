# ?? Gu�a de Pruebas Completas - API Caf� Producci�n

## ?? Descripci�n

El archivo `test-completo.ps1` es un script exhaustivo que prueba **TODOS** los controladores y funcionalidades de la API. Ejecuta m�s de 100 pruebas que validan:

- ? Creaci�n de entidades (POST)
- ? Consulta de entidades (GET)
- ? Actualizaci�n de entidades (PUT)
- ? Validaciones de campos
- ? Validaciones de integridad referencial
- ? Endpoints especializados
- ? Restricciones de eliminaci�n
- ? Relaciones entre entidades

---

## ?? C�mo Ejecutar las Pruebas

### 1. Iniciar la API
```powershell
# Opci�n 1: Con el script de inicio
.\start-api.ps1

# Opci�n 2: Manualmente
dotnet run --launch-profile http
```

### 2. Ejecutar las Pruebas
```powershell
# Ejecutar todas las pruebas
.\test-completo.ps1

# Con pol�tica de ejecuci�n (si es necesario)
powershell -ExecutionPolicy Bypass -File .\test-completo.ps1
```

### 3. Revisar Resultados
El script mostrar�:
- ? Pruebas exitosas en verde
- ? Pruebas fallidas en rojo
- Resumen final con porcentaje de �xito
- IDs de todas las entidades creadas

---

## ?? Estructura de las Pruebas

### 1. Proveedores (5 pruebas)
- POST - Crear proveedor v�lido
- POST - Rechazar sin nombre
- GET - Obtener todos
- GET - Obtener por ID
- PUT - Actualizar

### 2. Clientes (4 pruebas)
- POST - Crear cliente v�lido
- POST - Rechazar con email inv�lido
- GET - Obtener todos
- GET - Obtener por ID con pedidos

### 3. Presentaciones (3 pruebas)
- POST - Crear presentaci�n v�lida
- POST - Rechazar sin tipo
- GET - Obtener todas

### 4. Productos (5 pruebas)
- POST - Crear producto v�lido
- POST - Rechazar con precio negativo
- POST - Rechazar con presentaci�n inexistente
- GET - Obtener todos
- GET - Obtener por ID con relaciones

### 5. Tipos de Grano (3 pruebas)
- POST - Crear tipo v�lido (Ar�bica, Robusta, Blends)
- POST - Rechazar tipo inv�lido
- GET - Obtener todos

### 6. Etapas (3 pruebas)
- POST - Crear etapa v�lida (Tostado, Molienda, Empaque)
- POST - Rechazar etapa inv�lida
- GET - Obtener todas

### 7. Rutas (5 pruebas)
- POST - Crear ruta v�lida
- POST - Rechazar con tiempo negativo
- GET - Obtener todas
- GET - Por zona
- GET - Por tipo

### 8. �rdenes de Compra (5 pruebas)
- POST - Crear orden v�lida
- POST - Rechazar con estado inv�lido
- GET - Obtener todas
- GET - Por proveedor
- GET - Por estado

### 9. Lotes (4 pruebas)
- POST - Crear lote v�lido
- POST - Rechazar con c�digo inv�lido (debe ser 6 caracteres)
- POST - Rechazar con fecha vencimiento anterior
- GET - Obtener todos

### 10. Lotes Terminados (3 pruebas)
- POST - Crear lote terminado v�lido
- POST - Rechazar con lote inexistente
- GET - Obtener todos

### 11. Cataciones (6 pruebas)
- POST - Crear cataci�n v�lida
- POST - Rechazar con puntaje inv�lido (0-100)
- POST - Rechazar con humedad inv�lida (0-100)
- GET - Obtener todas
- GET - Aprobadas
- GET - Por lote terminado

### 12. Pedidos (6 pruebas)
- POST - Crear pedido v�lido
- POST - Rechazar con estado inv�lido
- GET - Obtener todos
- GET - Prioritarios
- GET - Por cliente
- GET - Por estado

### 13. OrdenCompraTipoGranos (4 pruebas)
- POST - Crear registro v�lido
- POST - Rechazar con cantidad negativa
- GET - Obtener todos
- GET - Por orden de compra

### 14. OrdenCompraTipoGranoLotes (4 pruebas)
- POST - Crear asignaci�n v�lida
- POST - Rechazar con cantidad inv�lida
- GET - Obtener todos
- GET - Por lote

### 15. LoteEtapas (5 pruebas)
- POST - Crear etapa de lote v�lida
- POST - Rechazar con fecha fin anterior
- GET - Obtener todas
- GET - Por lote
- GET - En proceso

### 16. PedidoLoteTerminados (4 pruebas)
- POST - Crear producto en pedido v�lido
- POST - Rechazar con cantidad inv�lida
- GET - Obtener todos
- GET - Por pedido

### 17. PedidoRutas (7 pruebas)
- POST - Crear asignaci�n de ruta v�lida
- POST - Rechazar con estado inv�lido
- POST - Rechazar con fecha entrega anterior
- GET - Obtener todas
- GET - Por pedido
- GET - Por ruta
- GET - En tr�nsito

### 18. Restricciones de Eliminaci�n (5 pruebas)
- DELETE - Rechazar eliminar cliente con pedidos
- DELETE - Rechazar eliminar producto con lotes
- DELETE - Rechazar eliminar proveedor con �rdenes
- DELETE - Rechazar eliminar lote con lotes terminados
- DELETE - Rechazar eliminar ruta con pedidos

---

## ?? Total de Pruebas

| Categor�a | Cantidad |
|-----------|----------|
| Creaci�n (POST) | ~35 pruebas |
| Consulta (GET) | ~45 pruebas |
| Actualizaci�n (PUT) | ~5 pruebas |
| Validaciones | ~40 pruebas |
| Restricciones | ~5 pruebas |
| **TOTAL** | **~130 pruebas** |

---

## ?? Interpretaci�n de Resultados

### Resumen Final
Al finalizar, ver�s algo como:

```
=====================================================
                  RESUMEN DE PRUEBAS
=====================================================

Total de pruebas ejecutadas: 130
Pruebas exitosas: 128
Pruebas fallidas: 2

Porcentaje de �xito: 98.46%

??  Algunas pruebas fallaron. Revisa los detalles arriba.
```

### C�digos de Color
- **Verde (?):** Prueba exitosa
- **Rojo (?):** Prueba fallida
- **Amarillo:** Informaci�n adicional o advertencias

### Porcentaje de �xito
- **100%:** �Perfecto! Todas las pruebas pasaron
- **90-99%:** Excelente, solo fallos menores
- **70-89%:** Aceptable, revisar fallos
- **< 70%:** Requiere atenci�n, muchos fallos

---

## ?? Depuraci�n de Fallos

### Si una prueba falla:

1. **Revisa el mensaje de error:**
   ```
   ? POST - Crear producto v�lido
     Error: Response status code does not indicate success: 400 (Bad Request)
   ```

2. **Verifica la API:**
   ```powershell
   # Ver logs de la API
   # La consola donde ejecutaste dotnet run mostrar� el error detallado
   ```

3. **Prueba manualmente:**
   ```powershell
   # Ejemplo: probar endpoint espec�fico
   $body = @{
       nombre = "Test"
       presentacionId = 1
       precio = 25.99
   } | ConvertTo-Json
   
   Invoke-RestMethod -Uri "http://localhost:5190/api/Productos" -Method POST -Body $body -ContentType "application/json"
   ```

4. **Revisa el controlador:**
   - Abre el archivo del controlador correspondiente
   - Verifica las validaciones implementadas
   - Comprueba que el endpoint existe

---

## ??? Soluci�n de Problemas Comunes

### Error: "La API no responde"
**Soluci�n:**
```powershell
# Aseg�rate de que la API est� ejecut�ndose
dotnet run --launch-profile http

# Verifica que est� en el puerto correcto (5190)
```

### Error: "No se retorn� ID"
**Causa:** El endpoint no est� retornando el objeto creado

**Soluci�n:** Verifica que el controlador use:
```csharp
return CreatedAtAction(nameof(GetMetodo), new { id = entidad.Id }, entidad);
```

### Error: "El ID de la URL no coincide"
**Causa:** Problema con el ID en la entidad creada

**Soluci�n:** Verifica que la base de datos est� generando IDs autom�ticamente

### Error: Muchas pruebas de validaci�n fallan
**Causa:** Las validaciones no est�n implementadas

**Soluci�n:** Revisa que los controladores tengan todas las validaciones necesarias

---

## ?? Flujo de Datos de Prueba

El script crea datos en este orden para mantener integridad referencial:

```
1. Proveedor
2. Cliente
3. Presentaci�n
4. Producto (requiere Presentaci�n)
5. Tipo de Grano
6. Etapa
7. Ruta
8. Orden de Compra (requiere Proveedor)
9. Lote
10. Orden-Tipo Grano (requiere Orden y Tipo Grano)
11. Orden-Tipo Grano-Lote (requiere Orden-Tipo Grano y Lote)
12. Lote Etapa (requiere Lote y Etapa)
13. Lote Terminado (requiere Lote y Producto)
14. Cataci�n (requiere Lote Terminado)
15. Pedido (requiere Cliente)
16. Pedido-Lote Terminado (requiere Pedido, Lote Terminado y Producto)
17. Pedido-Ruta (requiere Pedido y Ruta)
```

---

## ?? IDs Creados

Al final de las pruebas, se muestran todos los IDs de entidades creadas:

```
Datos de prueba creados:
  - Proveedor ID: 1
  - Cliente ID: 1
  - Presentaci�n ID: 1
  - Producto ID: 1
  - Tipo Grano ID: 1
  - Etapa ID: 1
  - Ruta ID: 1
  - Orden Compra ID: 1
  - Lote ID: 1
  - Lote Terminado ID: 1
  - Cataci�n ID: 1
  - Pedido ID: 1
```

Puedes usar estos IDs para consultas manuales adicionales.

---

## ?? Ejecutar Pruebas M�ltiples Veces

### Primera ejecuci�n:
```powershell
.\test-completo.ps1
```
? Crea nuevos datos con timestamps �nicos

### Segunda ejecuci�n:
```powershell
.\test-completo.ps1
```
? Crea m�s datos (nombres/c�digos �nicos)

**Nota:** El script usa timestamps para garantizar nombres/c�digos �nicos en cada ejecuci�n.

---

## ?? Limpiar Datos de Prueba

Si quieres limpiar la base de datos:

```powershell
# Opci�n 1: Eliminar base de datos y recrear
Remove-Item -Path ".\cafe.db" -Force
dotnet ef database update

# Opci�n 2: Limpiar manualmente por API (respetando dependencias)
# Primero eliminar tablas intermedias, luego las principales
```

---

## ?? Ejemplo de Salida Completa

```
=====================================================
    PRUEBAS COMPLETAS - API CAF� PRODUCCI�N
=====================================================

Verificando conexi�n con la API...
? API respondiendo correctamente

1. Probando ProveedoresController...
  ? POST - Crear proveedor v�lido
  ? POST - Rechazar proveedor sin nombre
  ? GET - Obtener todos los proveedores
  ? GET - Obtener proveedor por ID
  ? PUT - Actualizar proveedor

2. Probando ClientesController...
  ? POST - Crear cliente v�lido
  ? POST - Rechazar cliente con email inv�lido
  ? GET - Obtener todos los clientes
  ? GET - Obtener cliente por ID con pedidos

[... m�s pruebas ...]

=====================================================
                  RESUMEN DE PRUEBAS
=====================================================

Total de pruebas ejecutadas: 130
Pruebas exitosas: 130
Pruebas fallidas: 0

Porcentaje de �xito: 100%

?? �TODAS LAS PRUEBAS PASARON EXITOSAMENTE!

Datos de prueba creados:
  - Proveedor ID: 1
  - Cliente ID: 1
  [... m�s IDs ...]

=====================================================
```

---

## ?? Pr�ximos Pasos

Despu�s de ejecutar las pruebas exitosamente:

1. ? Revisa que todos los endpoints funcionen
2. ? Verifica las validaciones implementadas
3. ? Prueba flujos de negocio completos
4. ? Implementa tests unitarios con xUnit
5. ? Agrega tests de integraci�n
6. ? Configura CI/CD con estas pruebas

---

## ?? Soporte

Si encuentras problemas:

1. Revisa los logs de la API (consola donde ejecutas `dotnet run`)
2. Verifica que la base de datos est� actualizada: `dotnet ef database update`
3. Aseg�rate de que todos los controladores est�n compilados sin errores
4. Verifica que el puerto 5190 est� disponible

---

�Disfruta probando tu API! ???
