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
    public class PresentacionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PresentacionesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Presentaciones
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Presentaciones>>> GetPresentaciones()
        {
            try
            {
                var presentaciones = await _context.Presentaciones
                    .Include(p => p.Productos)
                    .ToListAsync();
                
                return Ok(presentaciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las presentaciones", error = ex.Message });
            }
        }

        // GET: api/Presentaciones/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Presentaciones>> GetPresentacion(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var presentacion = await _context.Presentaciones
                    .Include(p => p.Productos)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (presentacion == null)
                {
                    return NotFound(new { message = $"Presentación con ID {id} no encontrada" });
                }

                return Ok(presentacion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la presentación", error = ex.Message });
            }
        }

        // PUT: api/Presentaciones/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutPresentacion(int id, Presentaciones presentacion)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                if (id != presentacion.Id)
                {
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID de la presentación" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el tipo no esté vacío
                if (string.IsNullOrWhiteSpace(presentacion.Tipo))
                {
                    return BadRequest(new { message = "El tipo de presentación es requerido" });
                }

                var presentacionExistente = await _context.Presentaciones.FindAsync(id);
                if (presentacionExistente == null)
                {
                    return NotFound(new { message = $"Presentación con ID {id} no encontrada" });
                }

                // Actualizar propiedades
                presentacionExistente.Tipo = presentacion.Tipo;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Presentación actualizada exitosamente", presentacion = presentacionExistente });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PresentacionExists(id))
                {
                    return NotFound(new { message = $"Presentación con ID {id} no encontrada" });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar la presentación", error = ex.Message });
            }
        }

        // POST: api/Presentaciones
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Presentaciones>> PostPresentacion(Presentaciones presentacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el tipo no esté vacío
                if (string.IsNullOrWhiteSpace(presentacion.Tipo))
                {
                    return BadRequest(new { message = "El tipo de presentación es requerido" });
                }

                // Validar que no exista una presentación con el mismo tipo
                var tipoExiste = await _context.Presentaciones.AnyAsync(p => p.Tipo == presentacion.Tipo);
                if (tipoExiste)
                {
                    return BadRequest(new { message = "Ya existe una presentación con ese tipo" });
                }

                _context.Presentaciones.Add(presentacion);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPresentacion), new { id = presentacion.Id }, presentacion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear la presentación", error = ex.Message });
            }
        }

        // DELETE: api/Presentaciones/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePresentacion(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var presentacion = await _context.Presentaciones
                    .Include(p => p.Productos)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (presentacion == null)
                {
                    return NotFound(new { message = $"Presentación con ID {id} no encontrada" });
                }

                // Verificar si tiene productos asociados
                if (presentacion.Productos.Any())
                {
                    return BadRequest(new 
                    { 
                        message = "No se puede eliminar la presentación porque tiene productos asociados",
                        productosCount = presentacion.Productos.Count
                    });
                }

                _context.Presentaciones.Remove(presentacion);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Presentación eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar la presentación", error = ex.Message });
            }
        }

        private bool PresentacionExists(int id)
        {
            return _context.Presentaciones.Any(e => e.Id == id);
        }
    }
}
