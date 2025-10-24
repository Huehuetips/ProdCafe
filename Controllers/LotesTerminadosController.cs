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
    public class LotesTerminadosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LotesTerminadosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/LotesTerminados
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<LotesTerminados>>> GetLotesTerminados()
        {
            try
            {
                var lotesTerminados = await _context.LotesTerminados
                    .Include(lt => lt.Lote)
                    .Include(lt => lt.Producto)
                        .ThenInclude(p => p.Presentacion)
                    .Include(lt => lt.Cataciones)
                    .ToListAsync();
                
                return Ok(lotesTerminados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los lotes terminados", error = ex.Message });
            }
        }

        // GET: api/LotesTerminados/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LotesTerminados>> GetLoteTerminado(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var loteTerminado = await _context.LotesTerminados
                    .Include(lt => lt.Lote)
                    .Include(lt => lt.Producto)
                        .ThenInclude(p => p.Presentacion)
                    .Include(lt => lt.Cataciones)
                    .Include(lt => lt.PedidoLoteTerminados)
                    .FirstOrDefaultAsync(lt => lt.Id == id);

                if (loteTerminado == null)
                {
                    return NotFound(new { message = $"Lote terminado con ID {id} no encontrado" });
                }

                return Ok(loteTerminado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el lote terminado", error = ex.Message });
            }
        }

        // PUT: api/LotesTerminados/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutLoteTerminado(int id, LotesTerminados loteTerminado)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                if (id != loteTerminado.Id)
                {
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID del lote terminado" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el lote exista
                var loteExiste = await _context.Lotes.AnyAsync(l => l.Id == loteTerminado.LoteId);
                if (!loteExiste)
                {
                    return BadRequest(new { message = $"El lote con ID {loteTerminado.LoteId} no existe" });
                }

                // Validar que el producto exista
                var productoExiste = await _context.Productos.AnyAsync(p => p.Id == loteTerminado.ProductoId);
                if (!productoExiste)
                {
                    return BadRequest(new { message = $"El producto con ID {loteTerminado.ProductoId} no existe" });
                }

                // Validar fechas
                if (loteTerminado.FechaVencimiento < loteTerminado.FechaEnvasado)
                {
                    return BadRequest(new { message = "La fecha de vencimiento no puede ser anterior a la fecha de envasado" });
                }

                var loteExistente = await _context.LotesTerminados.FindAsync(id);
                if (loteExistente == null)
                {
                    return NotFound(new { message = $"Lote terminado con ID {id} no encontrado" });
                }

                // Actualizar propiedades
                loteExistente.LoteId = loteTerminado.LoteId;
                loteExistente.ProductoId = loteTerminado.ProductoId;
                loteExistente.FechaEnvasado = loteTerminado.FechaEnvasado;
                loteExistente.FechaVencimiento = loteTerminado.FechaVencimiento;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Lote terminado actualizado exitosamente", loteTerminado = loteExistente });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoteTerminadoExists(id))
                {
                    return NotFound(new { message = $"Lote terminado con ID {id} no encontrado" });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el lote terminado", error = ex.Message });
            }
        }

        // POST: api/LotesTerminados
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LotesTerminados>> PostLoteTerminado(LotesTerminados loteTerminado)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el lote exista
                var loteExiste = await _context.Lotes.AnyAsync(l => l.Id == loteTerminado.LoteId);
                if (!loteExiste)
                {
                    return BadRequest(new { message = $"El lote con ID {loteTerminado.LoteId} no existe" });
                }

                // Validar que el producto exista
                var productoExiste = await _context.Productos.AnyAsync(p => p.Id == loteTerminado.ProductoId);
                if (!productoExiste)
                {
                    return BadRequest(new { message = $"El producto con ID {loteTerminado.ProductoId} no existe" });
                }

                // Validar fechas
                if (loteTerminado.FechaVencimiento < loteTerminado.FechaEnvasado)
                {
                    return BadRequest(new { message = "La fecha de vencimiento no puede ser anterior a la fecha de envasado" });
                }

                _context.LotesTerminados.Add(loteTerminado);
                await _context.SaveChangesAsync();

                // Cargar relaciones para la respuesta
                await _context.Entry(loteTerminado).Reference(lt => lt.Lote).LoadAsync();
                await _context.Entry(loteTerminado).Reference(lt => lt.Producto).LoadAsync();

                return CreatedAtAction(nameof(GetLoteTerminado), new { id = loteTerminado.Id }, loteTerminado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el lote terminado", error = ex.Message });
            }
        }

        // DELETE: api/LotesTerminados/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteLoteTerminado(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var loteTerminado = await _context.LotesTerminados
                    .Include(lt => lt.Cataciones)
                    .Include(lt => lt.PedidoLoteTerminados)
                    .FirstOrDefaultAsync(lt => lt.Id == id);

                if (loteTerminado == null)
                {
                    return NotFound(new { message = $"Lote terminado con ID {id} no encontrado" });
                }

                // Verificar si tiene cataciones o pedidos asociados
                if (loteTerminado.Cataciones.Any() || loteTerminado.PedidoLoteTerminados.Any())
                {
                    return BadRequest(new 
                    { 
                        message = "No se puede eliminar el lote terminado porque tiene cataciones o pedidos asociados",
                        catacionesCount = loteTerminado.Cataciones.Count,
                        pedidosCount = loteTerminado.PedidoLoteTerminados.Count
                    });
                }

                _context.LotesTerminados.Remove(loteTerminado);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Lote terminado eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el lote terminado", error = ex.Message });
            }
        }

        private bool LoteTerminadoExists(int id)
        {
            return _context.LotesTerminados.Any(e => e.Id == id);
        }
    }
}
