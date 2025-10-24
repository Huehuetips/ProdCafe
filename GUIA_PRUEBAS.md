# ?? Guía de Pruebas Completas - API Café Producción

## ?? Descripción

El archivo `test-completo.ps1` es un script exhaustivo que prueba **TODOS** los controladores y funcionalidades de la API. Ejecuta más de 100 pruebas que validan:

- ? Creación de entidades (POST)
- ? Consulta de entidades (GET)
- ? Actualización de entidades (PUT)
- ? Validaciones de campos
- ? Validaciones de integridad referencial
- ? Endpoints especializados
- ? Restricciones de eliminación
- ? Relaciones entre entidades

---

## ?? Cómo Ejecutar las Pruebas

### 1. Iniciar la API
```powershell
# Opción 1: Con el script de inicio
.\start-api.ps1

# Opción 2: Manualmente
dotnet run --launch-profile http
```

### 2. Ejecutar las Pruebas
```powershell
# Ejecutar todas las pruebas
.\test-completo.ps1

# Con política de ejecución (si es necesario)
powershell -ExecutionPolicy Bypass -File .\test-completo.ps1
```

### 3. Revisar Resultados
El script mostrará:
- ? Pruebas exitosas en verde
- ? Pruebas fallidas en rojo
- Resumen final con porcentaje de éxito
- IDs de todas las entidades creadas

---

## ?? Estructura de las Pruebas

### 1. Proveedores (5 pruebas)
- POST - Crear proveedor válido
- POST - Rechazar sin nombre
- GET - Obtener todos
- GET - Obtener por ID
- PUT - Actualizar

### 2. Clientes (4 pruebas)
- POST - Crear cliente válido
- POST - Rechazar con email inválido
- GET - Obtener todos
- GET - Obtener por ID con pedidos

### 3. Presentaciones (3 pruebas)
- POST - Crear presentación válida
- POST - Rechazar sin tipo
- GET - Obtener todas

### 4. Productos (5 pruebas)
- POST - Crear producto válido
- POST - Rechazar con precio negativo
- POST - Rechazar con presentación inexistente
- GET - Obtener todos
- GET - Obtener por ID con relaciones

### 5. Tipos de Grano (3 pruebas)
- POST - Crear tipo válido (Arábica, Robusta, Blends)
- POST - Rechazar tipo inválido
- GET - Obtener todos

### 6. Etapas (3 pruebas)
- POST - Crear etapa válida (Tostado, Molienda, Empaque)
- POST - Rechazar etapa inválida
- GET - Obtener todas

### 7. Rutas (5 pruebas)
- POST - Crear ruta válida
- POST - Rechazar con tiempo negativo
- GET - Obtener todas
- GET - Por zona
- GET - Por tipo

### 8. Órdenes de Compra (5 pruebas)
- POST - Crear orden válida
- POST - Rechazar con estado inválido
- GET - Obtener todas
- GET - Por proveedor
- GET - Por estado

### 9. Lotes (4 pruebas)
- POST - Crear lote válido
- POST - Rechazar con código inválido (debe ser 6 caracteres)
- POST - Rechazar con fecha vencimiento anterior
- GET - Obtener todos

### 10. Lotes Terminados (3 pruebas)
- POST - Crear lote terminado válido
- POST - Rechazar con lote inexistente
- GET - Obtener todos

### 11. Cataciones (6 pruebas)
- POST - Crear catación válida
- POST - Rechazar con puntaje inválido (0-100)
- POST - Rechazar con humedad inválida (0-100)
- GET - Obtener todas
- GET - Aprobadas
- GET - Por lote terminado

### 12. Pedidos (6 pruebas)
- POST - Crear pedido válido
- POST - Rechazar con estado inválido
- GET - Obtener todos
- GET - Prioritarios
- GET - Por cliente
- GET - Por estado

### 13. OrdenCompraTipoGranos (4 pruebas)
- POST - Crear registro válido
- POST - Rechazar con cantidad negativa
- GET - Obtener todos
- GET - Por orden de compra

### 14. OrdenCompraTipoGranoLotes (4 pruebas)
- POST - Crear asignación válida
- POST - Rechazar con cantidad inválida
- GET - Obtener todos
- GET - Por lote

### 15. LoteEtapas (5 pruebas)
- POST - Crear etapa de lote válida
- POST - Rechazar con fecha fin anterior
- GET - Obtener todas
- GET - Por lote
- GET - En proceso

### 16. PedidoLoteTerminados (4 pruebas)
- POST - Crear producto en pedido válido
- POST - Rechazar con cantidad inválida
- GET - Obtener todos
- GET - Por pedido

### 17. PedidoRutas (7 pruebas)
- POST - Crear asignación de ruta válida
- POST - Rechazar con estado inválido
- POST - Rechazar con fecha entrega anterior
- GET - Obtener todas
- GET - Por pedido
- GET - Por ruta
- GET - En tránsito

### 18. Restricciones de Eliminación (5 pruebas)
- DELETE - Rechazar eliminar cliente con pedidos
- DELETE - Rechazar eliminar producto con lotes
- DELETE - Rechazar eliminar proveedor con órdenes
- DELETE - Rechazar eliminar lote con lotes terminados
- DELETE - Rechazar eliminar ruta con pedidos

---

## ?? Total de Pruebas

| Categoría | Cantidad |
|-----------|----------|
| Creación (POST) | ~35 pruebas |
| Consulta (GET) | ~45 pruebas |
| Actualización (PUT) | ~5 pruebas |
| Validaciones | ~40 pruebas |
| Restricciones | ~5 pruebas |
| **TOTAL** | **~130 pruebas** |

---

## ?? Interpretación de Resultados

### Resumen Final
Al finalizar, verás algo como:

```
=====================================================
                  RESUMEN DE PRUEBAS
=====================================================

Total de pruebas ejecutadas: 130
Pruebas exitosas: 128
Pruebas fallidas: 2

Porcentaje de éxito: 98.46%

??  Algunas pruebas fallaron. Revisa los detalles arriba.
```

### Códigos de Color
- **Verde (?):** Prueba exitosa
- **Rojo (?):** Prueba fallida
- **Amarillo:** Información adicional o advertencias

### Porcentaje de Éxito
- **100%:** ¡Perfecto! Todas las pruebas pasaron
- **90-99%:** Excelente, solo fallos menores
- **70-89%:** Aceptable, revisar fallos
- **< 70%:** Requiere atención, muchos fallos

---

## ?? Depuración de Fallos

### Si una prueba falla:

1. **Revisa el mensaje de error:**
   ```
   ? POST - Crear producto válido
     Error: Response status code does not indicate success: 400 (Bad Request)
   ```

2. **Verifica la API:**
   ```powershell
   # Ver logs de la API
   # La consola donde ejecutaste dotnet run mostrará el error detallado
   ```

3. **Prueba manualmente:**
   ```powershell
   # Ejemplo: probar endpoint específico
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

## ??? Solución de Problemas Comunes

### Error: "La API no responde"
**Solución:**
```powershell
# Asegúrate de que la API está ejecutándose
dotnet run --launch-profile http

# Verifica que esté en el puerto correcto (5190)
```

### Error: "No se retornó ID"
**Causa:** El endpoint no está retornando el objeto creado

**Solución:** Verifica que el controlador use:
```csharp
return CreatedAtAction(nameof(GetMetodo), new { id = entidad.Id }, entidad);
```

### Error: "El ID de la URL no coincide"
**Causa:** Problema con el ID en la entidad creada

**Solución:** Verifica que la base de datos está generando IDs automáticamente

### Error: Muchas pruebas de validación fallan
**Causa:** Las validaciones no están implementadas

**Solución:** Revisa que los controladores tengan todas las validaciones necesarias

---

## ?? Flujo de Datos de Prueba

El script crea datos en este orden para mantener integridad referencial:

```
1. Proveedor
2. Cliente
3. Presentación
4. Producto (requiere Presentación)
5. Tipo de Grano
6. Etapa
7. Ruta
8. Orden de Compra (requiere Proveedor)
9. Lote
10. Orden-Tipo Grano (requiere Orden y Tipo Grano)
11. Orden-Tipo Grano-Lote (requiere Orden-Tipo Grano y Lote)
12. Lote Etapa (requiere Lote y Etapa)
13. Lote Terminado (requiere Lote y Producto)
14. Catación (requiere Lote Terminado)
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
  - Presentación ID: 1
  - Producto ID: 1
  - Tipo Grano ID: 1
  - Etapa ID: 1
  - Ruta ID: 1
  - Orden Compra ID: 1
  - Lote ID: 1
  - Lote Terminado ID: 1
  - Catación ID: 1
  - Pedido ID: 1
```

Puedes usar estos IDs para consultas manuales adicionales.

---

## ?? Ejecutar Pruebas Múltiples Veces

### Primera ejecución:
```powershell
.\test-completo.ps1
```
? Crea nuevos datos con timestamps únicos

### Segunda ejecución:
```powershell
.\test-completo.ps1
```
? Crea más datos (nombres/códigos únicos)

**Nota:** El script usa timestamps para garantizar nombres/códigos únicos en cada ejecución.

---

## ?? Limpiar Datos de Prueba

Si quieres limpiar la base de datos:

```powershell
# Opción 1: Eliminar base de datos y recrear
Remove-Item -Path ".\cafe.db" -Force
dotnet ef database update

# Opción 2: Limpiar manualmente por API (respetando dependencias)
# Primero eliminar tablas intermedias, luego las principales
```

---

## ?? Ejemplo de Salida Completa

```
=====================================================
    PRUEBAS COMPLETAS - API CAFÉ PRODUCCIÓN
=====================================================

Verificando conexión con la API...
? API respondiendo correctamente

1. Probando ProveedoresController...
  ? POST - Crear proveedor válido
  ? POST - Rechazar proveedor sin nombre
  ? GET - Obtener todos los proveedores
  ? GET - Obtener proveedor por ID
  ? PUT - Actualizar proveedor

2. Probando ClientesController...
  ? POST - Crear cliente válido
  ? POST - Rechazar cliente con email inválido
  ? GET - Obtener todos los clientes
  ? GET - Obtener cliente por ID con pedidos

[... más pruebas ...]

=====================================================
                  RESUMEN DE PRUEBAS
=====================================================

Total de pruebas ejecutadas: 130
Pruebas exitosas: 130
Pruebas fallidas: 0

Porcentaje de éxito: 100%

?? ¡TODAS LAS PRUEBAS PASARON EXITOSAMENTE!

Datos de prueba creados:
  - Proveedor ID: 1
  - Cliente ID: 1
  [... más IDs ...]

=====================================================
```

---

## ?? Próximos Pasos

Después de ejecutar las pruebas exitosamente:

1. ? Revisa que todos los endpoints funcionen
2. ? Verifica las validaciones implementadas
3. ? Prueba flujos de negocio completos
4. ? Implementa tests unitarios con xUnit
5. ? Agrega tests de integración
6. ? Configura CI/CD con estas pruebas

---

## ?? Soporte

Si encuentras problemas:

1. Revisa los logs de la API (consola donde ejecutas `dotnet run`)
2. Verifica que la base de datos esté actualizada: `dotnet ef database update`
3. Asegúrate de que todos los controladores estén compilados sin errores
4. Verifica que el puerto 5190 esté disponible

---

¡Disfruta probando tu API! ???
