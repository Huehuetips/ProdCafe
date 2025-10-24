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
    public class PedidoRutasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PedidoRutasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PedidoRutas
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PedidoRuta>>> GetPedidoRutas()
        {
            try
            {
                var items = await _context.PedidoRutas
                    .Include(pr => pr.Pedido)
                        .ThenInclude(p => p.Cliente)
                    .Include(pr => pr.Ruta)
                    .ToListAsync();
                
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los registros", error = ex.Message });
            }
        }

        // GET: api/PedidoRutas/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PedidoRuta>> GetPedidoRuta(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var item = await _context.PedidoRutas
                    .Include(pr => pr.Pedido)
                        .ThenInclude(p => p.Cliente)
                    .Include(pr => pr.Ruta)
                    .FirstOrDefaultAsync(pr => pr.Id == id);

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

        // GET: api/PedidoRutas/PorPedido/5
        [HttpGet("PorPedido/{pedidoId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PedidoRuta>>> GetPorPedido(int pedidoId)
        {
            try
            {
                var items = await _context.PedidoRutas
                    .Where(pr => pr.PedidoId == pedidoId)
                    .Include(pr => pr.Ruta)
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los registros", error = ex.Message });
            }
        }

        // GET: api/PedidoRutas/PorRuta/5
        [HttpGet("PorRuta/{rutaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PedidoRuta>>> GetPorRuta(int rutaId)
        {
            try
            {
                var items = await _context.PedidoRutas
                    .Where(pr => pr.RutaId == rutaId)
                    .Include(pr => pr.Pedido)
                        .ThenInclude(p => p.Cliente)
                    .OrderBy(pr => pr.FechaSalida)
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los registros", error = ex.Message });
            }
        }

        // GET: api/PedidoRutas/EnTransito
        [HttpGet("EnTransito")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PedidoRuta>>> GetEnTransito()
        {
            try
            {
                var items = await _context.PedidoRutas
                    .Where(pr => pr.FechaEntrega == null || pr.Estado == "En Tránsito")
                    .Include(pr => pr.Pedido)
                        .ThenInclude(p => p.Cliente)
                    .Include(pr => pr.Ruta)
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los registros en tránsito", error = ex.Message });
            }
        }

        // PUT: api/PedidoRutas/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutPedidoRuta(int id, PedidoRuta item)
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

                // Validar que la ruta exista
                var rutaExiste = await _context.Rutas.AnyAsync(r => r.Id == item.RutaId);
                if (!rutaExiste)
                {
                    return BadRequest(new { message = $"La ruta con ID {item.RutaId} no existe" });
                }

                // Validar estados válidos
                var estadosValidos = new[] { "Pendiente", "En Tránsito", "Entregado", "Cancelado" };
                if (!string.IsNullOrWhiteSpace(item.Estado) && !estadosValidos.Contains(item.Estado))
                {
                    return BadRequest(new { message = $"Estado inválido. Debe ser uno de: {string.Join(", ", estadosValidos)}" });
                }

                // Validar fechas
                if (item.FechaEntrega.HasValue && item.FechaEntrega < item.FechaSalida)
                {
                    return BadRequest(new { message = "La fecha de entrega no puede ser anterior a la fecha de salida" });
                }

                var itemExistente = await _context.PedidoRutas.FindAsync(id);
                if (itemExistente == null)
                {
                    return NotFound(new { message = $"Registro con ID {id} no encontrado" });
                }

                // Actualizar propiedades
                itemExistente.PedidoId = item.PedidoId;
                itemExistente.RutaId = item.RutaId;
                itemExistente.FechaSalida = item.FechaSalida;
                itemExistente.FechaEntrega = item.FechaEntrega;
                itemExistente.Estado = item.Estado;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Registro actualizado exitosamente", item = itemExistente });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoRutaExists(id))
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

        // POST: api/PedidoRutas
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PedidoRuta>> PostPedidoRuta(PedidoRuta item)
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

                // Validar que la ruta exista
                var rutaExiste = await _context.Rutas.AnyAsync(r => r.Id == item.RutaId);
                if (!rutaExiste)
                {
                    return BadRequest(new { message = $"La ruta con ID {item.RutaId} no existe" });
                }

                // Validar estados válidos
                var estadosValidos = new[] { "Pendiente", "En Tránsito", "Entregado", "Cancelado" };
                if (!string.IsNullOrWhiteSpace(item.Estado) && !estadosValidos.Contains(item.Estado))
                {
                    return BadRequest(new { message = $"Estado inválido. Debe ser uno de: {string.Join(", ", estadosValidos)}" });
                }

                // Si no se especifica estado, establecer como Pendiente
                if (string.IsNullOrWhiteSpace(item.Estado))
                {
                    item.Estado = "Pendiente";
                }

                // Validar fechas
                if (item.FechaEntrega.HasValue && item.FechaEntrega < item.FechaSalida)
                {
                    return BadRequest(new { message = "La fecha de entrega no puede ser anterior a la fecha de salida" });
                }

                _context.PedidoRutas.Add(item);
                await _context.SaveChangesAsync();

                // Cargar relaciones para la respuesta
                await _context.Entry(item).Reference(i => i.Pedido).LoadAsync();
                await _context.Entry(item).Reference(i => i.Ruta).LoadAsync();

                return CreatedAtAction(nameof(GetPedidoRuta), new { id = item.Id }, item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el registro", error = ex.Message });
            }
        }

        // DELETE: api/PedidoRutas/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePedidoRuta(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var item = await _context.PedidoRutas.FindAsync(id);
                if (item == null)
                {
                    return NotFound(new { message = $"Registro con ID {id} no encontrado" });
                }

                _context.PedidoRutas.Remove(item);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Registro eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el registro", error = ex.Message });
            }
        }

        private bool PedidoRutaExists(int id)
        {
            return _context.PedidoRutas.Any(e => e.Id == id);
        }
    }
}
