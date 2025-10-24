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
    public class LoteEtapasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoteEtapasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/LoteEtapas
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<LoteEtapa>>> GetLoteEtapas()
        {
            try
            {
                var items = await _context.LoteEtapas
                    .Include(le => le.Lote)
                    .Include(le => le.Etapa)
                    .ToListAsync();
                
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los registros", error = ex.Message });
            }
        }

        // GET: api/LoteEtapas/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoteEtapa>> GetLoteEtapa(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var item = await _context.LoteEtapas
                    .Include(le => le.Lote)
                    .Include(le => le.Etapa)
                    .FirstOrDefaultAsync(le => le.Id == id);

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

        // GET: api/LoteEtapas/PorLote/5
        [HttpGet("PorLote/{loteId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LoteEtapa>>> GetPorLote(int loteId)
        {
            try
            {
                var items = await _context.LoteEtapas
                    .Where(le => le.LoteId == loteId)
                    .Include(le => le.Etapa)
                    .OrderBy(le => le.FechaInicio)
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los registros", error = ex.Message });
            }
        }

        // GET: api/LoteEtapas/EnProceso
        [HttpGet("EnProceso")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LoteEtapa>>> GetEnProceso()
        {
            try
            {
                var items = await _context.LoteEtapas
                    .Where(le => le.FechaFin == null)
                    .Include(le => le.Lote)
                    .Include(le => le.Etapa)
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los registros", error = ex.Message });
            }
        }

        // PUT: api/LoteEtapas/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutLoteEtapa(int id, LoteEtapa item)
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

                // Validar que el lote exista
                var loteExiste = await _context.Lotes.AnyAsync(l => l.Id == item.LoteId);
                if (!loteExiste)
                {
                    return BadRequest(new { message = $"El lote con ID {item.LoteId} no existe" });
                }

                // Validar que la etapa exista
                var etapaExiste = await _context.Etapas.AnyAsync(e => e.Id == item.EtapaId);
                if (!etapaExiste)
                {
                    return BadRequest(new { message = $"La etapa con ID {item.EtapaId} no existe" });
                }

                // Validar fechas
                if (item.FechaFin.HasValue && item.FechaFin < item.FechaInicio)
                {
                    return BadRequest(new { message = "La fecha de fin no puede ser anterior a la fecha de inicio" });
                }

                var itemExistente = await _context.LoteEtapas.FindAsync(id);
                if (itemExistente == null)
                {
                    return NotFound(new { message = $"Registro con ID {id} no encontrado" });
                }

                // Actualizar propiedades
                itemExistente.LoteId = item.LoteId;
                itemExistente.EtapaId = item.EtapaId;
                itemExistente.FechaInicio = item.FechaInicio;
                itemExistente.FechaFin = item.FechaFin;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Registro actualizado exitosamente", item = itemExistente });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoteEtapaExists(id))
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

        // POST: api/LoteEtapas
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoteEtapa>> PostLoteEtapa(LoteEtapa item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el lote exista
                var loteExiste = await _context.Lotes.AnyAsync(l => l.Id == item.LoteId);
                if (!loteExiste)
                {
                    return BadRequest(new { message = $"El lote con ID {item.LoteId} no existe" });
                }

                // Validar que la etapa exista
                var etapaExiste = await _context.Etapas.AnyAsync(e => e.Id == item.EtapaId);
                if (!etapaExiste)
                {
                    return BadRequest(new { message = $"La etapa con ID {item.EtapaId} no existe" });
                }

                // Validar fechas
                if (item.FechaFin.HasValue && item.FechaFin < item.FechaInicio)
                {
                    return BadRequest(new { message = "La fecha de fin no puede ser anterior a la fecha de inicio" });
                }

                _context.LoteEtapas.Add(item);
                await _context.SaveChangesAsync();

                // Cargar relaciones para la respuesta
                await _context.Entry(item).Reference(i => i.Lote).LoadAsync();
                await _context.Entry(item).Reference(i => i.Etapa).LoadAsync();

                return CreatedAtAction(nameof(GetLoteEtapa), new { id = item.Id }, item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el registro", error = ex.Message });
            }
        }

        // DELETE: api/LoteEtapas/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteLoteEtapa(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var item = await _context.LoteEtapas.FindAsync(id);
                if (item == null)
                {
                    return NotFound(new { message = $"Registro con ID {id} no encontrado" });
                }

                _context.LoteEtapas.Remove(item);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Registro eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el registro", error = ex.Message });
            }
        }

        private bool LoteEtapaExists(int id)
        {
            return _context.LoteEtapas.Any(e => e.Id == id);
        }
    }
}
