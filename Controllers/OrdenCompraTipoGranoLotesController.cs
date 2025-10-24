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
    public class OrdenCompraTipoGranoLotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdenCompraTipoGranoLotesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/OrdenCompraTipoGranoLotes
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<OrdenCompraTipoGranoLote>>> GetOrdenCompraTipoGranoLotes()
        {
            try
            {
                var items = await _context.OrdenCompraTipoGranoLotes
                    .Include(octgl => octgl.OrdenCompraTipoGrano)
                        .ThenInclude(octg => octg.OrdenCompra)
                    .Include(octgl => octgl.OrdenCompraTipoGrano)
                        .ThenInclude(octg => octg.TipoGrano)
                    .Include(octgl => octgl.Lote)
                    .ToListAsync();
                
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los registros", error = ex.Message });
            }
        }

        // GET: api/OrdenCompraTipoGranoLotes/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrdenCompraTipoGranoLote>> GetOrdenCompraTipoGranoLote(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var item = await _context.OrdenCompraTipoGranoLotes
                    .Include(octgl => octgl.OrdenCompraTipoGrano)
                        .ThenInclude(octg => octg.OrdenCompra)
                            .ThenInclude(oc => oc.Proveedor)
                    .Include(octgl => octgl.OrdenCompraTipoGrano)
                        .ThenInclude(octg => octg.TipoGrano)
                    .Include(octgl => octgl.Lote)
                    .FirstOrDefaultAsync(octgl => octgl.Id == id);

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

        // GET: api/OrdenCompraTipoGranoLotes/PorLote/5
        [HttpGet("PorLote/{loteId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrdenCompraTipoGranoLote>>> GetPorLote(int loteId)
        {
            try
            {
                var items = await _context.OrdenCompraTipoGranoLotes
                    .Where(octgl => octgl.LoteId == loteId)
                    .Include(octgl => octgl.OrdenCompraTipoGrano)
                        .ThenInclude(octg => octg.TipoGrano)
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los registros", error = ex.Message });
            }
        }

        // PUT: api/OrdenCompraTipoGranoLotes/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutOrdenCompraTipoGranoLote(int id, OrdenCompraTipoGranoLote item)
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

                // Validar que OrdenCompraTipoGrano exista
                var octgExiste = await _context.OrdenCompraTipoGranos.AnyAsync(octg => octg.Id == item.OrdenCompraTipoGranoId);
                if (!octgExiste)
                {
                    return BadRequest(new { message = $"El registro de orden-tipo grano con ID {item.OrdenCompraTipoGranoId} no existe" });
                }

                // Validar que el lote exista
                var loteExiste = await _context.Lotes.AnyAsync(l => l.Id == item.LoteId);
                if (!loteExiste)
                {
                    return BadRequest(new { message = $"El lote con ID {item.LoteId} no existe" });
                }

                // Validar cantidad
                if (item.CantidadKg <= 0)
                {
                    return BadRequest(new { message = "La cantidad debe ser mayor que 0" });
                }

                var itemExistente = await _context.OrdenCompraTipoGranoLotes.FindAsync(id);
                if (itemExistente == null)
                {
                    return NotFound(new { message = $"Registro con ID {id} no encontrado" });
                }

                // Actualizar propiedades
                itemExistente.OrdenCompraTipoGranoId = item.OrdenCompraTipoGranoId;
                itemExistente.LoteId = item.LoteId;
                itemExistente.CantidadKg = item.CantidadKg;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Registro actualizado exitosamente", item = itemExistente });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdenCompraTipoGranoLoteExists(id))
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

        // POST: api/OrdenCompraTipoGranoLotes
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrdenCompraTipoGranoLote>> PostOrdenCompraTipoGranoLote(OrdenCompraTipoGranoLote item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que OrdenCompraTipoGrano exista
                var octgExiste = await _context.OrdenCompraTipoGranos.AnyAsync(octg => octg.Id == item.OrdenCompraTipoGranoId);
                if (!octgExiste)
                {
                    return BadRequest(new { message = $"El registro de orden-tipo grano con ID {item.OrdenCompraTipoGranoId} no existe" });
                }

                // Validar que el lote exista
                var loteExiste = await _context.Lotes.AnyAsync(l => l.Id == item.LoteId);
                if (!loteExiste)
                {
                    return BadRequest(new { message = $"El lote con ID {item.LoteId} no existe" });
                }

                // Validar cantidad
                if (item.CantidadKg <= 0)
                {
                    return BadRequest(new { message = "La cantidad debe ser mayor que 0" });
                }

                _context.OrdenCompraTipoGranoLotes.Add(item);
                await _context.SaveChangesAsync();

                // Cargar relaciones para la respuesta
                await _context.Entry(item).Reference(i => i.OrdenCompraTipoGrano).LoadAsync();
                await _context.Entry(item).Reference(i => i.Lote).LoadAsync();

                return CreatedAtAction(nameof(GetOrdenCompraTipoGranoLote), new { id = item.Id }, item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el registro", error = ex.Message });
            }
        }

        // DELETE: api/OrdenCompraTipoGranoLotes/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteOrdenCompraTipoGranoLote(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var item = await _context.OrdenCompraTipoGranoLotes.FindAsync(id);
                if (item == null)
                {
                    return NotFound(new { message = $"Registro con ID {id} no encontrado" });
                }

                _context.OrdenCompraTipoGranoLotes.Remove(item);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Registro eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el registro", error = ex.Message });
            }
        }

        private bool OrdenCompraTipoGranoLoteExists(int id)
        {
            return _context.OrdenCompraTipoGranoLotes.Any(e => e.Id == id);
        }
    }
}
