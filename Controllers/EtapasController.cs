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
    public class EtapasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EtapasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Etapas
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Etapas>>> GetEtapas()
        {
            try
            {
                var etapas = await _context.Etapas
                    .Include(e => e.LoteEtapas)
                        .ThenInclude(le => le.Lote)
                    .ToListAsync();
                
                return Ok(etapas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las etapas", error = ex.Message });
            }
        }

        // GET: api/Etapas/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Etapas>> GetEtapa(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var etapa = await _context.Etapas
                    .Include(e => e.LoteEtapas)
                        .ThenInclude(le => le.Lote)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (etapa == null)
                {
                    return NotFound(new { message = $"Etapa con ID {id} no encontrada" });
                }

                return Ok(etapa);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la etapa", error = ex.Message });
            }
        }

        // PUT: api/Etapas/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutEtapa(int id, Etapas etapa)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                if (id != etapa.Id)
                {
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID de la etapa" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el nombre no esté vacío
                if (string.IsNullOrWhiteSpace(etapa.Nombre))
                {
                    return BadRequest(new { message = "El nombre de la etapa es requerido" });
                }

                // Validar valores válidos
                var nombresValidos = new[] { "Tostado", "Molienda", "Empaque" };
                if (!nombresValidos.Contains(etapa.Nombre))
                {
                    return BadRequest(new { message = $"Nombre inválido. Debe ser uno de: {string.Join(", ", nombresValidos)}" });
                }

                var etapaExistente = await _context.Etapas.FindAsync(id);
                if (etapaExistente == null)
                {
                    return NotFound(new { message = $"Etapa con ID {id} no encontrada" });
                }

                // Actualizar propiedades
                etapaExistente.Nombre = etapa.Nombre;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Etapa actualizada exitosamente", etapa = etapaExistente });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EtapaExists(id))
                {
                    return NotFound(new { message = $"Etapa con ID {id} no encontrada" });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar la etapa", error = ex.Message });
            }
        }

        // POST: api/Etapas
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Etapas>> PostEtapa(Etapas etapa)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el nombre no esté vacío
                if (string.IsNullOrWhiteSpace(etapa.Nombre))
                {
                    return BadRequest(new { message = "El nombre de la etapa es requerido" });
                }

                // Validar valores válidos
                var nombresValidos = new[] { "Tostado", "Molienda", "Empaque" };
                if (!nombresValidos.Contains(etapa.Nombre))
                {
                    return BadRequest(new { message = $"Nombre inválido. Debe ser uno de: {string.Join(", ", nombresValidos)}" });
                }

                // Validar que no exista una etapa con el mismo nombre
                var nombreExiste = await _context.Etapas.AnyAsync(e => e.Nombre == etapa.Nombre);
                if (nombreExiste)
                {
                    return BadRequest(new { message = "Ya existe una etapa con ese nombre" });
                }

                _context.Etapas.Add(etapa);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEtapa), new { id = etapa.Id }, etapa);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear la etapa", error = ex.Message });
            }
        }

        // DELETE: api/Etapas/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEtapa(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var etapa = await _context.Etapas
                    .Include(e => e.LoteEtapas)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (etapa == null)
                {
                    return NotFound(new { message = $"Etapa con ID {id} no encontrada" });
                }

                // Verificar si tiene lotes asociados
                if (etapa.LoteEtapas.Any())
                {
                    return BadRequest(new 
                    { 
                        message = "No se puede eliminar la etapa porque tiene lotes asociados",
                        lotesCount = etapa.LoteEtapas.Count
                    });
                }

                _context.Etapas.Remove(etapa);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Etapa eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar la etapa", error = ex.Message });
            }
        }

        private bool EtapaExists(int id)
        {
            return _context.Etapas.Any(e => e.Id == id);
        }
    }
}
