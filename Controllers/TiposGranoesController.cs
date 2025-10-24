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
    public class TiposGranoesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TiposGranoesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TiposGranoes
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<TiposGrano>>> GetTiposGranos()
        {
            try
            {
                var tiposGrano = await _context.TiposGranos
                    .Include(tg => tg.OrdenCompraTipoGranos)
                        .ThenInclude(octg => octg.OrdenCompra)
                    .ToListAsync();
                
                return Ok(tiposGrano);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los tipos de grano", error = ex.Message });
            }
        }

        // GET: api/TiposGranoes/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TiposGrano>> GetTipoGrano(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var tipoGrano = await _context.TiposGranos
                    .Include(tg => tg.OrdenCompraTipoGranos)
                        .ThenInclude(octg => octg.OrdenCompra)
                            .ThenInclude(oc => oc.Proveedor)
                    .FirstOrDefaultAsync(tg => tg.Id == id);

                if (tipoGrano == null)
                {
                    return NotFound(new { message = $"Tipo de grano con ID {id} no encontrado" });
                }

                return Ok(tipoGrano);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el tipo de grano", error = ex.Message });
            }
        }

        // PUT: api/TiposGranoes/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutTipoGrano(int id, TiposGrano tipoGrano)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                if (id != tipoGrano.Id)
                {
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID del tipo de grano" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el nombre no esté vacío
                if (string.IsNullOrWhiteSpace(tipoGrano.Nombre))
                {
                    return BadRequest(new { message = "El nombre del tipo de grano es requerido" });
                }

                // Validar valores válidos
                var nombresValidos = new[] { "Arábica", "Robusta", "Blends", "arábica", "robusta", "blends" };
                if (!nombresValidos.Contains(tipoGrano.Nombre))
                {
                    return BadRequest(new { message = "Nombre inválido. Debe ser: Arábica, Robusta o Blends" });
                }

                var tipoGranoExistente = await _context.TiposGranos.FindAsync(id);
                if (tipoGranoExistente == null)
                {
                    return NotFound(new { message = $"Tipo de grano con ID {id} no encontrado" });
                }

                // Actualizar propiedades
                tipoGranoExistente.Nombre = tipoGrano.Nombre;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Tipo de grano actualizado exitosamente", tipoGrano = tipoGranoExistente });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoGranoExists(id))
                {
                    return NotFound(new { message = $"Tipo de grano con ID {id} no encontrado" });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el tipo de grano", error = ex.Message });
            }
        }

        // POST: api/TiposGranoes
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TiposGrano>> PostTipoGrano(TiposGrano tipoGrano)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el nombre no esté vacío
                if (string.IsNullOrWhiteSpace(tipoGrano.Nombre))
                {
                    return BadRequest(new { message = "El nombre del tipo de grano es requerido" });
                }

                // Validar valores válidos
                var nombresValidos = new[] { "Arábica", "Robusta", "Blends", "arábica", "robusta", "blends" };
                if (!nombresValidos.Contains(tipoGrano.Nombre))
                {
                    return BadRequest(new { message = "Nombre inválido. Debe ser: Arábica, Robusta o Blends" });
                }

                // Validar que no exista un tipo de grano con el mismo nombre (case-insensitive)
                var nombreExiste = await _context.TiposGranos
                    .AnyAsync(tg => tg.Nombre.ToLower() == tipoGrano.Nombre.ToLower());
                if (nombreExiste)
                {
                    return BadRequest(new { message = "Ya existe un tipo de grano con ese nombre" });
                }

                _context.TiposGranos.Add(tipoGrano);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTipoGrano), new { id = tipoGrano.Id }, tipoGrano);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el tipo de grano", error = ex.Message });
            }
        }

        // DELETE: api/TiposGranoes/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTipoGrano(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var tipoGrano = await _context.TiposGranos
                    .Include(tg => tg.OrdenCompraTipoGranos)
                    .FirstOrDefaultAsync(tg => tg.Id == id);

                if (tipoGrano == null)
                {
                    return NotFound(new { message = $"Tipo de grano con ID {id} no encontrado" });
                }

                // Verificar si tiene órdenes de compra asociadas
                if (tipoGrano.OrdenCompraTipoGranos.Any())
                {
                    return BadRequest(new 
                    { 
                        message = "No se puede eliminar el tipo de grano porque tiene órdenes de compra asociadas",
                        ordenesCount = tipoGrano.OrdenCompraTipoGranos.Count
                    });
                }

                _context.TiposGranos.Remove(tipoGrano);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Tipo de grano eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el tipo de grano", error = ex.Message });
            }
        }

        private bool TipoGranoExists(int id)
        {
            return _context.TiposGranos.Any(e => e.Id == id);
        }
    }
}
