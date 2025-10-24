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
    public class OrdenCompraTipoGranosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdenCompraTipoGranosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/OrdenCompraTipoGranos
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<OrdenCompraTipoGrano>>> GetOrdenCompraTipoGranos()
        {
            try
            {
                var items = await _context.OrdenCompraTipoGranos
                    .Include(octg => octg.OrdenCompra)
                        .ThenInclude(oc => oc.Proveedor)
                    .Include(octg => octg.TipoGrano)
                    .Include(octg => octg.OrdenCompraTipoGranoLotes)
                    .ToListAsync();
                
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los registros", error = ex.Message });
            }
        }

        // GET: api/OrdenCompraTipoGranos/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrdenCompraTipoGrano>> GetOrdenCompraTipoGrano(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var item = await _context.OrdenCompraTipoGranos
                    .Include(octg => octg.OrdenCompra)
                        .ThenInclude(oc => oc.Proveedor)
                    .Include(octg => octg.TipoGrano)
                    .Include(octg => octg.OrdenCompraTipoGranoLotes)
                        .ThenInclude(octgl => octgl.Lote)
                    .FirstOrDefaultAsync(octg => octg.Id == id);

                if (item == null)
                {
                    return NotFound(new { message = $"Registro con ID {id} no encontrado" });
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el registro", error = ex.Message });
            }
        }

        // GET: api/OrdenCompraTipoGranos/PorOrdenCompra/5
        [HttpGet("PorOrdenCompra/{ordenCompraId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrdenCompraTipoGrano>>> GetPorOrdenCompra(int ordenCompraId)
        {
            try
            {
                var items = await _context.OrdenCompraTipoGranos
                    .Where(octg => octg.OrdenCompraId == ordenCompraId)
                    .Include(octg => octg.TipoGrano)
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los registros", error = ex.Message });
            }
        }

        // PUT: api/OrdenCompraTipoGranos/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutOrdenCompraTipoGrano(int id, OrdenCompraTipoGrano item)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                if (id != item.Id)
                {
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID del registro" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que la orden de compra exista
                var ordenExiste = await _context.OrdenesCompras.AnyAsync(oc => oc.Id == item.OrdenCompraId);
                if (!ordenExiste)
                {
                    return BadRequest(new { message = $"La orden de compra con ID {item.OrdenCompraId} no existe" });
                }

                // Validar que el tipo de grano exista
                var tipoGranoExiste = await _context.TiposGranos.AnyAsync(tg => tg.Id == item.TipoGranoId);
                if (!tipoGranoExiste)
                {
                    return BadRequest(new { message = $"El tipo de grano con ID {item.TipoGranoId} no existe" });
                }

                // Validar cantidades y precios
                if (item.CantidadKg <= 0)
                {
                    return BadRequest(new { message = "La cantidad debe ser mayor que 0" });
                }

                if (item.PrecioUnitarioKg < 0)
                {
                    return BadRequest(new { message = "El precio unitario no puede ser negativo" });
                }

                if (item.PrecioTotal < 0)
                {
                    return BadRequest(new { message = "El precio total no puede ser negativo" });
                }

                var itemExistente = await _context.OrdenCompraTipoGranos.FindAsync(id);
                if (itemExistente == null)
                {
                    return NotFound(new { message = $"Registro con ID {id} no encontrado" });
                }

                // Actualizar propiedades
                itemExistente.OrdenCompraId = item.OrdenCompraId;
                itemExistente.TipoGranoId = item.TipoGranoId;
                itemExistente.CantidadKg = item.CantidadKg;
                itemExistente.PrecioUnitarioKg = item.PrecioUnitarioKg;
                itemExistente.PrecioTotal = item.PrecioTotal;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Registro actualizado exitosamente", item = itemExistente });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdenCompraTipoGranoExists(id))
                {
                    return NotFound(new { message = $"Registro con ID {id} no encontrado" });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el registro", error = ex.Message });
            }
        }

        // POST: api/OrdenCompraTipoGranos
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrdenCompraTipoGrano>> PostOrdenCompraTipoGrano(OrdenCompraTipoGrano item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que la orden de compra exista
                var ordenExiste = await _context.OrdenesCompras.AnyAsync(oc => oc.Id == item.OrdenCompraId);
                if (!ordenExiste)
                {
                    return BadRequest(new { message = $"La orden de compra con ID {item.OrdenCompraId} no existe" });
                }

                // Validar que el tipo de grano exista
                var tipoGranoExiste = await _context.TiposGranos.AnyAsync(tg => tg.Id == item.TipoGranoId);
                if (!tipoGranoExiste)
                {
                    return BadRequest(new { message = $"El tipo de grano con ID {item.TipoGranoId} no existe" });
                }

                // Validar cantidades y precios
                if (item.CantidadKg <= 0)
                {
                    return BadRequest(new { message = "La cantidad debe ser mayor que 0" });
                }

                if (item.PrecioUnitarioKg < 0)
                {
                    return BadRequest(new { message = "El precio unitario no puede ser negativo" });
                }

                if (item.PrecioTotal < 0)
                {
                    return BadRequest(new { message = "El precio total no puede ser negativo" });
                }

                _context.OrdenCompraTipoGranos.Add(item);
                await _context.SaveChangesAsync();

                // Cargar relaciones para la respuesta
                await _context.Entry(item).Reference(i => i.OrdenCompra).LoadAsync();
                await _context.Entry(item).Reference(i => i.TipoGrano).LoadAsync();

                return CreatedAtAction(nameof(GetOrdenCompraTipoGrano), new { id = item.Id }, item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el registro", error = ex.Message });
            }
        }

        // DELETE: api/OrdenCompraTipoGranos/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteOrdenCompraTipoGrano(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var item = await _context.OrdenCompraTipoGranos
                    .Include(octg => octg.OrdenCompraTipoGranoLotes)
                    .FirstOrDefaultAsync(octg => octg.Id == id);

                if (item == null)
                {
                    return NotFound(new { message = $"Registro con ID {id} no encontrado" });
                }

                // Verificar si tiene lotes asociados
                if (item.OrdenCompraTipoGranoLotes.Any())
                {
                    return BadRequest(new 
                    { 
                        message = "No se puede eliminar el registro porque tiene lotes asociados",
                        lotesCount = item.OrdenCompraTipoGranoLotes.Count
                    });
                }

                _context.OrdenCompraTipoGranos.Remove(item);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Registro eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el registro", error = ex.Message });
            }
        }

        private bool OrdenCompraTipoGranoExists(int id)
        {
            return _context.OrdenCompraTipoGranos.Any(e => e.Id == id);
        }
    }
}
