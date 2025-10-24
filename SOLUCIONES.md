# ?? Soluciones a Problemas Encontrados

## Problemas Identificados

### 1. Error 400 en POST de Productos, �rdenes, Lotes y Pedidos

**Causa:** Las entidades usan `DateOnly` pero el JSON est� enviando strings.

**Soluci�n:** .NET 9 ya maneja autom�ticamente la serializaci�n de `DateOnly` desde strings en formato `yyyy-MM-dd`.

### 2. Propiedades de Navegaci�n Requeridas

**Problema:** Las propiedades de navegaci�n est�n marcadas como `= null!` lo que puede causar problemas con el model binding.

**Ejemplo del problema:**
```csharp
[ForeignKey(nameof(PresentacionId))]
public Presentaciones Presentacion { get; set; } = null!;
```

**Soluci�n:** Estas propiedades NO deben ser enviadas en el JSON. Solo se deben enviar los IDs.

---

## ?? Comandos Para Diagnosticar

### 1. Ejecutar Diagn�stico
```powershell
.\diagnostico.ps1
```

Esto mostrar� los errores exactos de cada endpoint.

### 2. Ver Logs de la API

En la consola donde ejecutas `dotnet run`, ver�s los errores completos con stack trace.

---

## ?? Errores Comunes y Soluciones

### Error: "The JSON value could not be converted to System.DateOnly"

**Causa:** Formato de fecha incorrecto

**Soluci�n:** Aseg�rate de enviar fechas en formato `yyyy-MM-dd`:
```json
{
  "fechaEmision": "2024-10-18"
}
```

### Error: "A required property was not supplied"

**Causa:** Falta un campo obligatorio

**Soluci�n:** Revisa el modelo de la entidad y aseg�rate de enviar todos los campos `required`.

### Error: "The ForeignKey attribute on property... is not valid"

**Causa:** Est�s intentando enviar la propiedad de navegaci�n en el JSON

**Soluci�n:** Solo env�a los IDs, no las entidades completas:
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
  "nombre": "Caf� Premium",
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

## ?? Pr�ximos Pasos

1. ? Ejecuta `.\diagnostico.ps1`
2. ? Comparte la salida completa
3. ? Revisa los logs de la API en la consola
4. ? Ajustaremos los controladores si es necesario

---

**Nota:** La mayor�a de los problemas se resuelven viendo los logs exactos de la API.
