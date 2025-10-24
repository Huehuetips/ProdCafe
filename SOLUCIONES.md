# ?? Soluciones a Problemas Encontrados

## Problemas Identificados

### 1. Error 400 en POST de Productos, Órdenes, Lotes y Pedidos

**Causa:** Las entidades usan `DateOnly` pero el JSON está enviando strings.

**Solución:** .NET 9 ya maneja automáticamente la serialización de `DateOnly` desde strings en formato `yyyy-MM-dd`.

### 2. Propiedades de Navegación Requeridas

**Problema:** Las propiedades de navegación están marcadas como `= null!` lo que puede causar problemas con el model binding.

**Ejemplo del problema:**
```csharp
[ForeignKey(nameof(PresentacionId))]
public Presentaciones Presentacion { get; set; } = null!;
```

**Solución:** Estas propiedades NO deben ser enviadas en el JSON. Solo se deben enviar los IDs.

---

## ?? Comandos Para Diagnosticar

### 1. Ejecutar Diagnóstico
```powershell
.\diagnostico.ps1
```

Esto mostrará los errores exactos de cada endpoint.

### 2. Ver Logs de la API

En la consola donde ejecutas `dotnet run`, verás los errores completos con stack trace.

---

## ?? Errores Comunes y Soluciones

### Error: "The JSON value could not be converted to System.DateOnly"

**Causa:** Formato de fecha incorrecto

**Solución:** Asegúrate de enviar fechas en formato `yyyy-MM-dd`:
```json
{
  "fechaEmision": "2024-10-18"
}
```

### Error: "A required property was not supplied"

**Causa:** Falta un campo obligatorio

**Solución:** Revisa el modelo de la entidad y asegúrate de enviar todos los campos `required`.

### Error: "The ForeignKey attribute on property... is not valid"

**Causa:** Estás intentando enviar la propiedad de navegación en el JSON

**Solución:** Solo envía los IDs, no las entidades completas:
```json
// ? Incorrecto
{
  "presentacionId": 1,
  "presentacion": { ... }
}

// ? Correcto
{
  "presentacionId": 1
}
```

---

## ?? Formato Correcto para Cada Entidad

### Producto
```json
{
  "nombre": "Café Premium",
  "presentacionId": 1,
  "nivelTostado": "Medio",
  "tipoMolido": "Fino",
  "precio": 25.99
}
```

### Orden de Compra
```json
{
  "proveedorId": 1,
  "estado": "Pendiente",
  "fechaEmision": "2024-10-18",
  "fechaRecepcion": null
}
```

### Lote
```json
{
  "codigo": "LOT001",
  "fechaIngreso": "2024-10-18",
  "fechaLote": "2024-10-18",
  "fechaVencimiento": "2025-04-18",
  "estado": "Activo",
  "observaciones": null
}
```

### Pedido
```json
{
  "clienteId": 1,
  "fecha": "2024-10-18",
  "estado": "Pendiente",
  "tipo": "Delivery",
  "prioritaria": true
}
```

---

## ?? Próximos Pasos

1. ? Ejecuta `.\diagnostico.ps1`
2. ? Comparte la salida completa
3. ? Revisa los logs de la API en la consola
4. ? Ajustaremos los controladores si es necesario

---

**Nota:** La mayoría de los problemas se resuelven viendo los logs exactos de la API.
