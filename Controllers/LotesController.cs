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
    public class LotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LotesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Lotes
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Lotes>>> GetLotes()
        {
            try
            {
                var lotes = await _context.Lotes
                    .Include(l => l.LotesTerminados)
                    .Include(l => l.LoteEtapas)
                    .ToListAsync();
                
                return Ok(lotes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los lotes", error = ex.Message });
            }
        }

        // GET: api/Lotes/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Lotes>> GetLote(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var lote = await _context.Lotes
                    .Include(l => l.LotesTerminados)
                    .Include(l => l.LoteEtapas)
                        .ThenInclude(le => le.Etapa)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (lote == null)
                {
                    return NotFound(new { message = $"Lote con ID {id} no encontrado" });
                }

                return Ok(lote);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el lote", error = ex.Message });
            }
        }

        // PUT: api/Lotes/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutLote(int id, Lotes lote)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                if (id != lote.Id)
                {
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID del lote" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar campos requeridos
                if (string.IsNullOrWhiteSpace(lote.Codigo))
                {
                    return BadRequest(new { message = "El código del lote es requerido" });
                }

                if (lote.Codigo.Length != 6)
                {
                    return BadRequest(new { message = "El código del lote debe tener exactamente 6 caracteres" });
                }

                // Validar fechas
                if (lote.FechaVencimiento < lote.FechaLote)
                {
                    return BadRequest(new { message = "La fecha de vencimiento no puede ser anterior a la fecha del lote" });
                }

                var loteExistente = await _context.Lotes.FindAsync(id);
                if (loteExistente == null)
                {
                    return NotFound(new { message = $"Lote con ID {id} no encontrado" });
                }

                // Actualizar propiedades
                loteExistente.Codigo = lote.Codigo;
                loteExistente.FechaIngreso = lote.FechaIngreso;
                loteExistente.FechaLote = lote.FechaLote;
                loteExistente.FechaVencimiento = lote.FechaVencimiento;
                loteExistente.Estado = lote.Estado;
                loteExistente.Observaciones = lote.Observaciones;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Lote actualizado exitosamente", lote = loteExistente });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoteExists(id))
                {
                    return NotFound(new { message = $"Lote con ID {id} no encontrado" });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el lote", error = ex.Message });
            }
        }

        // POST: api/Lotes
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Lotes>> PostLote(Lotes lote)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar campos requeridos
                if (string.IsNullOrWhiteSpace(lote.Codigo))
                {
                    return BadRequest(new { message = "El código del lote es requerido" });
                }

                if (lote.Codigo.Length != 6)
                {
                    return BadRequest(new { message = "El código del lote debe tener exactamente 6 caracteres" });
                }

                // Validar fechas
                if (lote.FechaVencimiento < lote.FechaLote)
                {
                    return BadRequest(new { message = "La fecha de vencimiento no puede ser anterior a la fecha del lote" });
                }

                // Validar que no exista un lote con el mismo código
                var codigoExiste = await _context.Lotes.AnyAsync(l => l.Codigo == lote.Codigo);
                if (codigoExiste)
                {
                    return BadRequest(new { message = "Ya existe un lote con ese código" });
                }

                _context.Lotes.Add(lote);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetLote), new { id = lote.Id }, lote);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el lote", error = ex.Message });
            }
        }

        // DELETE: api/Lotes/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteLote(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var lote = await _context.Lotes
                    .Include(l => l.LotesTerminados)
                    .Include(l => l.LoteEtapas)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (lote == null)
                {
                    return NotFound(new { message = $"Lote con ID {id} no encontrado" });
                }

                // Verificar si tiene lotes terminados o etapas asociadas
                if (lote.LotesTerminados.Any() || lote.LoteEtapas.Any())
                {
                    return BadRequest(new 
                    { 
                        message = "No se puede eliminar el lote porque tiene lotes terminados o etapas asociadas",
                        lotesTerminadosCount = lote.LotesTerminados.Count,
                        etapasCount = lote.LoteEtapas.Count
                    });
                }

                _context.Lotes.Remove(lote);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Lote eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el lote", error = ex.Message });
            }
        }

        private bool LoteExists(int id)
        {
            return _context.Lotes.Any(e => e.Id == id);
        }
    }
}
