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
    public class PedidosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PedidosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Pedidos
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Pedidos>>> GetPedidos()
        {
            try
            {
                var pedidos = await _context.Pedidos
                    .Include(p => p.Cliente)
                    .Include(p => p.PedidoLoteTerminados)
                    .Include(p => p.PedidoRutas)
                    .ToListAsync();
                
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los pedidos", error = ex.Message });
            }
        }

        // GET: api/Pedidos/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Pedidos>> GetPedido(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var pedido = await _context.Pedidos
                    .Include(p => p.Cliente)
                    .Include(p => p.PedidoLoteTerminados)
                        .ThenInclude(plt => plt.Producto)
                    .Include(p => p.PedidoLoteTerminados)
                        .ThenInclude(plt => plt.LoteTerminado)
                    .Include(p => p.PedidoRutas)
                        .ThenInclude(pr => pr.Ruta)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pedido == null)
                {
                    return NotFound(new { message = $"Pedido con ID {id} no encontrado" });
                }

                return Ok(pedido);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el pedido", error = ex.Message });
            }
        }

        // GET: api/Pedidos/PorCliente/5
        [HttpGet("PorCliente/{clienteId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Pedidos>>> GetPedidosPorCliente(int clienteId)
        {
            try
            {
                var pedidos = await _context.Pedidos
                    .Where(p => p.ClienteId == clienteId)
                    .Include(p => p.Cliente)
                    .Include(p => p.PedidoLoteTerminados)
                    .ToListAsync();

                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los pedidos del cliente", error = ex.Message });
            }
        }

        // GET: api/Pedidos/Prioritarios
        [HttpGet("Prioritarios")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Pedidos>>> GetPedidosPrioritarios()
        {
            try
            {
                var pedidos = await _context.Pedidos
                    .Where(p => p.Prioritaria)
                    .Include(p => p.Cliente)
                    .OrderBy(p => p.Fecha)
                    .ToListAsync();

                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los pedidos prioritarios", error = ex.Message });
            }
        }

        // GET: api/Pedidos/PorEstado/{estado}
        [HttpGet("PorEstado/{estado}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Pedidos>>> GetPedidosPorEstado(string estado)
        {
            try
            {
                var pedidos = await _context.Pedidos
                    .Where(p => p.Estado == estado)
                    .Include(p => p.Cliente)
                    .ToListAsync();

                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los pedidos por estado", error = ex.Message });
            }
        }

        // PUT: api/Pedidos/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutPedido(int id, Pedidos pedido)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                if (id != pedido.Id)
                {
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID del pedido" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el cliente exista
                var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == pedido.ClienteId);
                if (!clienteExiste)
                {
                    return BadRequest(new { message = $"El cliente con ID {pedido.ClienteId} no existe" });
                }

                // Validar estados válidos
                var estadosValidos = new[] { "Pendiente", "En Proceso", "Listo", "Entregado", "Cancelado" };
                if (!string.IsNullOrWhiteSpace(pedido.Estado) && !estadosValidos.Contains(pedido.Estado))
                {
                    return BadRequest(new { message = $"Estado inválido. Debe ser uno de: {string.Join(", ", estadosValidos)}" });
                }

                var pedidoExistente = await _context.Pedidos.FindAsync(id);
                if (pedidoExistente == null)
                {
                    return NotFound(new { message = $"Pedido con ID {id} no encontrado" });
                }

                // Actualizar propiedades
                pedidoExistente.ClienteId = pedido.ClienteId;
                pedidoExistente.Fecha = pedido.Fecha;
                pedidoExistente.Estado = pedido.Estado;
                pedidoExistente.Tipo = pedido.Tipo;
                pedidoExistente.Prioritaria = pedido.Prioritaria;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Pedido actualizado exitosamente", pedido = pedidoExistente });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoExists(id))
                {
                    return NotFound(new { message = $"Pedido con ID {id} no encontrado" });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el pedido", error = ex.Message });
            }
        }

        // POST: api/Pedidos
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Pedidos>> PostPedido(Pedidos pedido)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el cliente exista
                var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == pedido.ClienteId);
                if (!clienteExiste)
                {
                    return BadRequest(new { message = $"El cliente con ID {pedido.ClienteId} no existe" });
                }

                // Validar estados válidos
                var estadosValidos = new[] { "Pendiente", "En Proceso", "Listo", "Entregado", "Cancelado" };
                if (!string.IsNullOrWhiteSpace(pedido.Estado) && !estadosValidos.Contains(pedido.Estado))
                {
                    return BadRequest(new { message = $"Estado inválido. Debe ser uno de: {string.Join(", ", estadosValidos)}" });
                }

                // Si no se especifica estado, establecer como Pendiente
                if (string.IsNullOrWhiteSpace(pedido.Estado))
                {
                    pedido.Estado = "Pendiente";
                }

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                // Cargar el cliente para la respuesta
                await _context.Entry(pedido).Reference(p => p.Cliente).LoadAsync();

                return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, pedido);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el pedido", error = ex.Message });
            }
        }

        // DELETE: api/Pedidos/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePedido(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var pedido = await _context.Pedidos
                    .Include(p => p.PedidoLoteTerminados)
                    .Include(p => p.PedidoRutas)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pedido == null)
                {
                    return NotFound(new { message = $"Pedido con ID {id} no encontrado" });
                }

                // Verificar si tiene productos o rutas asociadas
                if (pedido.PedidoLoteTerminados.Any() || pedido.PedidoRutas.Any())
                {
                    return BadRequest(new 
                    { 
                        message = "No se puede eliminar el pedido porque tiene productos o rutas asociadas",
                        productosCount = pedido.PedidoLoteTerminados.Count,
                        rutasCount = pedido.PedidoRutas.Count
                    });
                }

                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Pedido eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el pedido", error = ex.Message });
            }
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.Id == id);
        }
    }
}
