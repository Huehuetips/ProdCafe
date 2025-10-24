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
    public class PedidoLoteTerminadosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PedidoLoteTerminadosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PedidoLoteTerminados
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PedidoLoteTerminado>>> GetPedidoLoteTerminados()
        {
            try
            {
                var items = await _context.PedidoLoteTerminados
                    .Include(plt => plt.Pedido)
                        .ThenInclude(p => p.Cliente)
                    .Include(plt => plt.LoteTerminado)
                        .ThenInclude(lt => lt.Producto)
                    .Include(plt => plt.Producto)
                    .ToListAsync();
                
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los registros", error = ex.Message });
            }
        }

        // GET: api/PedidoLoteTerminados/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PedidoLoteTerminado>> GetPedidoLoteTerminado(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var item = await _context.PedidoLoteTerminados
                    .Include(plt => plt.Pedido)
                        .ThenInclude(p => p.Cliente)
                    .Include(plt => plt.LoteTerminado)
                        .ThenInclude(lt => lt.Producto)
                            .ThenInclude(p => p.Presentacion)
                    .Include(plt => plt.Producto)
                    .FirstOrDefaultAsync(plt => plt.Id == id);

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

        // GET: api/PedidoLoteTerminados/PorPedido/5
        [HttpGet("PorPedido/{pedidoId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PedidoLoteTerminado>>> GetPorPedido(int pedidoId)
        {
            try
            {
                var items = await _context.PedidoLoteTerminados
                    .Where(plt => plt.PedidoId == pedidoId)
                    .Include(plt => plt.LoteTerminado)
                    .Include(plt => plt.Producto)
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los registros", error = ex.Message });
            }
        }

        // PUT: api/PedidoLoteTerminados/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutPedidoLoteTerminado(int id, PedidoLoteTerminado item)
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

                // Validar que el pedido exista
                var pedidoExiste = await _context.Pedidos.AnyAsync(p => p.Id == item.PedidoId);
                if (!pedidoExiste)
                {
                    return BadRequest(new { message = $"El pedido con ID {item.PedidoId} no existe" });
                }

                // Validar que el lote terminado exista
                var loteTerminadoExiste = await _context.LotesTerminados.AnyAsync(lt => lt.Id == item.LoteTerminadoId);
                if (!loteTerminadoExiste)
                {
                    return BadRequest(new { message = $"El lote terminado con ID {item.LoteTerminadoId} no existe" });
                }

                // Validar que el producto exista
                var productoExiste = await _context.Productos.AnyAsync(p => p.Id == item.ProductoId);
                if (!productoExiste)
                {
                    return BadRequest(new { message = $"El producto con ID {item.ProductoId} no existe" });
                }

                // Validar cantidad
                if (item.Cantidad <= 0)
                {
                    return BadRequest(new { message = "La cantidad debe ser mayor que 0" });
                }

                var itemExistente = await _context.PedidoLoteTerminados.FindAsync(id);
                if (itemExistente == null)
                {
                    return NotFound(new { message = $"Registro con ID {id} no encontrado" });
                }

                // Actualizar propiedades
                itemExistente.PedidoId = item.PedidoId;
                itemExistente.LoteTerminadoId = item.LoteTerminadoId;
                itemExistente.ProductoId = item.ProductoId;
                itemExistente.Cantidad = item.Cantidad;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Registro actualizado exitosamente", item = itemExistente });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoLoteTerminadoExists(id))
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

        // POST: api/PedidoLoteTerminados
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PedidoLoteTerminado>> PostPedidoLoteTerminado(PedidoLoteTerminado item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el pedido exista
                var pedidoExiste = await _context.Pedidos.AnyAsync(p => p.Id == item.PedidoId);
                if (!pedidoExiste)
                {
                    return BadRequest(new { message = $"El pedido con ID {item.PedidoId} no existe" });
                }

                // Validar que el lote terminado exista
                var loteTerminadoExiste = await _context.LotesTerminados.AnyAsync(lt => lt.Id == item.LoteTerminadoId);
                if (!loteTerminadoExiste)
                {
                    return BadRequest(new { message = $"El lote terminado con ID {item.LoteTerminadoId} no existe" });
                }

                // Validar que el producto exista
                var productoExiste = await _context.Productos.AnyAsync(p => p.Id == item.ProductoId);
                if (!productoExiste)
                {
                    return BadRequest(new { message = $"El producto con ID {item.ProductoId} no existe" });
                }

                // Validar cantidad
                if (item.Cantidad <= 0)
                {
                    return BadRequest(new { message = "La cantidad debe ser mayor que 0" });
                }

                _context.PedidoLoteTerminados.Add(item);
                await _context.SaveChangesAsync();

                // Cargar relaciones para la respuesta
                await _context.Entry(item).Reference(i => i.Pedido).LoadAsync();
                await _context.Entry(item).Reference(i => i.LoteTerminado).LoadAsync();
                await _context.Entry(item).Reference(i => i.Producto).LoadAsync();

                return CreatedAtAction(nameof(GetPedidoLoteTerminado), new { id = item.Id }, item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el registro", error = ex.Message });
            }
        }

        // DELETE: api/PedidoLoteTerminados/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePedidoLoteTerminado(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var item = await _context.PedidoLoteTerminados.FindAsync(id);
                if (item == null)
                {
                    return NotFound(new { message = $"Registro con ID {id} no encontrado" });
                }

                _context.PedidoLoteTerminados.Remove(item);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Registro eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el registro", error = ex.Message });
            }
        }

        private bool PedidoLoteTerminadoExists(int id)
        {
            return _context.PedidoLoteTerminados.Any(e => e.Id == id);
        }
    }
}
