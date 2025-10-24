using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiEjemplo.Data;
using ApiEjemplo.Models;

namespace ApiEjemplo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenesComprasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdenesComprasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/OrdenesCompras
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<OrdenesCompra>>> GetOrdenesCompras()
        {
            try
            {
                var ordenes = await _context.OrdenesCompras
                    .Include(o => o.Proveedor)
                    .Include(o => o.OrdenCompraTipoGranos)
                        .ThenInclude(octg => octg.TipoGrano)
                    .ToListAsync();
                
                return Ok(ordenes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las órdenes de compra", error = ex.Message });
            }
        }

        // GET: api/OrdenesCompras/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrdenesCompra>> GetOrdenCompra(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var orden = await _context.OrdenesCompras
                    .Include(o => o.Proveedor)
                    .Include(o => o.OrdenCompraTipoGranos)
                        .ThenInclude(octg => octg.TipoGrano)
                    .Include(o => o.OrdenCompraTipoGranos)
                        .ThenInclude(octg => octg.OrdenCompraTipoGranoLotes)
                            .ThenInclude(octgl => octgl.Lote)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (orden == null)
                {
                    return NotFound(new { message = $"Orden de compra con ID {id} no encontrada" });
                }

                return Ok(orden);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la orden de compra", error = ex.Message });
            }
        }

        // GET: api/OrdenesCompras/PorProveedor/5
        [HttpGet("PorProveedor/{proveedorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrdenesCompra>>> GetOrdenesPorProveedor(int proveedorId)
        {
            try
            {
                var ordenes = await _context.OrdenesCompras
                    .Where(o => o.ProveedorId == proveedorId)
                    .Include(o => o.Proveedor)
                    .Include(o => o.OrdenCompraTipoGranos)
                    .ToListAsync();

                return Ok(ordenes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las órdenes del proveedor", error = ex.Message });
            }
        }

        // GET: api/OrdenesCompras/PorEstado/{estado}
        [HttpGet("PorEstado/{estado}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrdenesCompra>>> GetOrdenesPorEstado(string estado)
        {
            try
            {
                var ordenes = await _context.OrdenesCompras
                    .Where(o => o.Estado == estado)
                    .Include(o => o.Proveedor)
                    .ToListAsync();

                return Ok(ordenes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las órdenes por estado", error = ex.Message });
            }
        }

        // PUT: api/OrdenesCompras/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutOrdenCompra(int id, OrdenesCompra ordenCompra)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                if (id != ordenCompra.Id)
                {
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID de la orden" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el proveedor exista
                var proveedorExiste = await _context.Proveedores.AnyAsync(p => p.Id == ordenCompra.ProveedorId);
                if (!proveedorExiste)
                {
                    return BadRequest(new { message = $"El proveedor con ID {ordenCompra.ProveedorId} no existe" });
                }

                // Validar estados válidos
                var estadosValidos = new[] { "Pendiente", "Enviada", "En Tránsito", "Recibida", "Cancelada" };
                if (!string.IsNullOrWhiteSpace(ordenCompra.Estado) && !estadosValidos.Contains(ordenCompra.Estado))
                {
                    return BadRequest(new { message = $"Estado inválido. Debe ser uno de: {string.Join(", ", estadosValidos)}" });
                }

                // Validar fechas
                if (ordenCompra.FechaRecepcion.HasValue && ordenCompra.FechaRecepcion < ordenCompra.FechaEmision)
                {
                    return BadRequest(new { message = "La fecha de recepción no puede ser anterior a la fecha de emisión" });
                }

                var ordenExistente = await _context.OrdenesCompras.FindAsync(id);
                if (ordenExistente == null)
                {
                    return NotFound(new { message = $"Orden de compra con ID {id} no encontrada" });
                }

                // Actualizar propiedades
                ordenExistente.ProveedorId = ordenCompra.ProveedorId;
                ordenExistente.Estado = ordenCompra.Estado;
                ordenExistente.FechaEmision = ordenCompra.FechaEmision;
                ordenExistente.FechaRecepcion = ordenCompra.FechaRecepcion;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Orden de compra actualizada exitosamente", orden = ordenExistente });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdenCompraExists(id))
                {
                    return NotFound(new { message = $"Orden de compra con ID {id} no encontrada" });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar la orden de compra", error = ex.Message });
            }
        }

        // POST: api/OrdenesCompras
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrdenesCompra>> PostOrdenCompra(OrdenesCompra ordenCompra)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el proveedor exista
                var proveedorExiste = await _context.Proveedores.AnyAsync(p => p.Id == ordenCompra.ProveedorId);
                if (!proveedorExiste)
                {
                    return BadRequest(new { message = $"El proveedor con ID {ordenCompra.ProveedorId} no existe" });
                }

                // Validar estados válidos
                var estadosValidos = new[] { "Pendiente", "Enviada", "En Tránsito", "Recibida", "Cancelada" };
                if (!string.IsNullOrWhiteSpace(ordenCompra.Estado) && !estadosValidos.Contains(ordenCompra.Estado))
                {
                    return BadRequest(new { message = $"Estado inválido. Debe ser uno de: {string.Join(", ", estadosValidos)}" });
                }

                // Si no se especifica estado, establecer como Pendiente
                if (string.IsNullOrWhiteSpace(ordenCompra.Estado))
                {
                    ordenCompra.Estado = "Pendiente";
                }

                // Validar fechas
                if (ordenCompra.FechaRecepcion.HasValue && ordenCompra.FechaRecepcion < ordenCompra.FechaEmision)
                {
                    return BadRequest(new { message = "La fecha de recepción no puede ser anterior a la fecha de emisión" });
                }

                _context.OrdenesCompras.Add(ordenCompra);
                await _context.SaveChangesAsync();

                // Cargar el proveedor para la respuesta
                await _context.Entry(ordenCompra).Reference(o => o.Proveedor).LoadAsync();

                return CreatedAtAction(nameof(GetOrdenCompra), new { id = ordenCompra.Id }, ordenCompra);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear la orden de compra", error = ex.Message });
            }
        }

        // DELETE: api/OrdenesCompras/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteOrdenCompra(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var orden = await _context.OrdenesCompras
                    .Include(o => o.OrdenCompraTipoGranos)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (orden == null)
                {
                    return NotFound(new { message = $"Orden de compra con ID {id} no encontrada" });
                }

                // Verificar si tiene tipos de grano asociados
                if (orden.OrdenCompraTipoGranos.Any())
                {
                    return BadRequest(new 
                    { 
                        message = "No se puede eliminar la orden porque tiene tipos de grano asociados",
                        tiposGranoCount = orden.OrdenCompraTipoGranos.Count
                    });
                }

                _context.OrdenesCompras.Remove(orden);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Orden de compra eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar la orden de compra", error = ex.Message });
            }
        }

        private bool OrdenCompraExists(int id)
        {
            return _context.OrdenesCompras.Any(e => e.Id == id);
        }
    }
}
