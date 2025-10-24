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
    public class CatacionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CatacionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Catacions
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Catacion>>> GetCataciones()
        {
            try
            {
                var cataciones = await _context.Cataciones
                    .Include(c => c.LoteTerminado)
                        .ThenInclude(lt => lt.Producto)
                    .Include(c => c.LoteTerminado)
                        .ThenInclude(lt => lt.Lote)
                    .ToListAsync();
                
                return Ok(cataciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las cataciones", error = ex.Message });
            }
        }

        // GET: api/Catacions/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Catacion>> GetCatacion(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var catacion = await _context.Cataciones
                    .Include(c => c.LoteTerminado)
                        .ThenInclude(lt => lt.Producto)
                    .Include(c => c.LoteTerminado)
                        .ThenInclude(lt => lt.Lote)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (catacion == null)
                {
                    return NotFound(new { message = $"Catación con ID {id} no encontrada" });
                }

                return Ok(catacion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la catación", error = ex.Message });
            }
        }

        // GET: api/Catacions/PorLoteTerminado/5
        [HttpGet("PorLoteTerminado/{loteTerminadoId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Catacion>>> GetCatacionesPorLote(int loteTerminadoId)
        {
            try
            {
                var cataciones = await _context.Cataciones
                    .Where(c => c.LoteTerminadoId == loteTerminadoId)
                    .Include(c => c.LoteTerminado)
                    .ToListAsync();

                return Ok(cataciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las cataciones del lote", error = ex.Message });
            }
        }

        // GET: api/Catacions/Aprobadas
        [HttpGet("Aprobadas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Catacion>>> GetCatacionesAprobadas()
        {
            try
            {
                var cataciones = await _context.Cataciones
                    .Where(c => c.Aprobado)
                    .Include(c => c.LoteTerminado)
                        .ThenInclude(lt => lt.Producto)
                    .ToListAsync();

                return Ok(cataciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las cataciones aprobadas", error = ex.Message });
            }
        }

        // PUT: api/Catacions/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutCatacion(int id, Catacion catacion)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                if (id != catacion.Id)
                {
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID de la catación" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validaciones de negocio
                if (catacion.Puntaje < 0 || catacion.Puntaje > 100)
                {
                    return BadRequest(new { message = "El puntaje debe estar entre 0 y 100" });
                }

                if (catacion.Humedad < 0 || catacion.Humedad > 100)
                {
                    return BadRequest(new { message = "La humedad debe estar entre 0 y 100" });
                }

                // Validar que el lote terminado exista
                var loteExiste = await _context.LotesTerminados.AnyAsync(lt => lt.Id == catacion.LoteTerminadoId);
                if (!loteExiste)
                {
                    return BadRequest(new { message = $"El lote terminado con ID {catacion.LoteTerminadoId} no existe" });
                }

                var catacionExistente = await _context.Cataciones.FindAsync(id);
                if (catacionExistente == null)
                {
                    return NotFound(new { message = $"Catación con ID {id} no encontrada" });
                }

                // Actualizar propiedades
                catacionExistente.LoteTerminadoId = catacion.LoteTerminadoId;
                catacionExistente.Puntaje = catacion.Puntaje;
                catacionExistente.Humedad = catacion.Humedad;
                catacionExistente.Notas = catacion.Notas;
                catacionExistente.Aprobado = catacion.Aprobado;
                catacionExistente.Fecha = catacion.Fecha;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Catación actualizada exitosamente", catacion = catacionExistente });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CatacionExists(id))
                {
                    return NotFound(new { message = $"Catación con ID {id} no encontrada" });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar la catación", error = ex.Message });
            }
        }

        // POST: api/Catacions
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Catacion>> PostCatacion(Catacion catacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validaciones de negocio
                if (catacion.Puntaje < 0 || catacion.Puntaje > 100)
                {
                    return BadRequest(new { message = "El puntaje debe estar entre 0 y 100" });
                }

                if (catacion.Humedad < 0 || catacion.Humedad > 100)
                {
                    return BadRequest(new { message = "La humedad debe estar entre 0 y 100" });
                }

                // Validar que el lote terminado exista
                var loteExiste = await _context.LotesTerminados.AnyAsync(lt => lt.Id == catacion.LoteTerminadoId);
                if (!loteExiste)
                {
                    return BadRequest(new { message = $"El lote terminado con ID {catacion.LoteTerminadoId} no existe" });
                }

                // Validar que la fecha no sea futura
                if (catacion.Fecha > DateOnly.FromDateTime(DateTime.Now))
                {
                    return BadRequest(new { message = "La fecha de catación no puede ser futura" });
                }

                _context.Cataciones.Add(catacion);
                await _context.SaveChangesAsync();

                // Cargar el lote terminado para la respuesta
                await _context.Entry(catacion).Reference(c => c.LoteTerminado).LoadAsync();

                return CreatedAtAction(nameof(GetCatacion), new { id = catacion.Id }, catacion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear la catación", error = ex.Message });
            }
        }

        // DELETE: api/Catacions/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCatacion(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var catacion = await _context.Cataciones.FindAsync(id);
                if (catacion == null)
                {
                    return NotFound(new { message = $"Catación con ID {id} no encontrada" });
                }

                _context.Cataciones.Remove(catacion);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Catación eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar la catación", error = ex.Message });
            }
        }

        private bool CatacionExists(int id)
        {
            return _context.Cataciones.Any(e => e.Id == id);
        }
    }
}
