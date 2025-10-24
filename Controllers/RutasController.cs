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
    public class RutasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RutasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Rutas
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Rutas>>> GetRutas()
        {
            try
            {
                var rutas = await _context.Rutas
                    .Include(r => r.PedidoRutas)
                        .ThenInclude(pr => pr.Pedido)
                    .ToListAsync();
                
                return Ok(rutas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las rutas", error = ex.Message });
            }
        }

        // GET: api/Rutas/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Rutas>> GetRuta(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var ruta = await _context.Rutas
                    .Include(r => r.PedidoRutas)
                        .ThenInclude(pr => pr.Pedido)
                            .ThenInclude(p => p.Cliente)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (ruta == null)
                {
                    return NotFound(new { message = $"Ruta con ID {id} no encontrada" });
                }

                return Ok(ruta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la ruta", error = ex.Message });
            }
        }

        // GET: api/Rutas/PorZona/{zona}
        [HttpGet("PorZona/{zona}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Rutas>>> GetRutasPorZona(string zona)
        {
            try
            {
                var rutas = await _context.Rutas
                    .Where(r => r.Zona == zona)
                    .Include(r => r.PedidoRutas)
                    .ToListAsync();

                return Ok(rutas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las rutas por zona", error = ex.Message });
            }
        }

        // GET: api/Rutas/PorTipo/{tipo}
        [HttpGet("PorTipo/{tipo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Rutas>>> GetRutasPorTipo(string tipo)
        {
            try
            {
                var rutas = await _context.Rutas
                    .Where(r => r.Tipo == tipo)
                    .ToListAsync();

                return Ok(rutas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las rutas por tipo", error = ex.Message });
            }
        }

        // PUT: api/Rutas/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutRuta(int id, Rutas ruta)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                if (id != ruta.Id)
                {
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID de la ruta" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar campos requeridos
                if (string.IsNullOrWhiteSpace(ruta.Nombre))
                {
                    return BadRequest(new { message = "El nombre de la ruta es requerido" });
                }

                if (ruta.TiempoEstimadoH < 0)
                {
                    return BadRequest(new { message = "El tiempo estimado no puede ser negativo" });
                }

                var rutaExistente = await _context.Rutas.FindAsync(id);
                if (rutaExistente == null)
                {
                    return NotFound(new { message = $"Ruta con ID {id} no encontrada" });
                }

                // Actualizar propiedades
                rutaExistente.Tipo = ruta.Tipo;
                rutaExistente.Nombre = ruta.Nombre;
                rutaExistente.Zona = ruta.Zona;
                rutaExistente.TiempoEstimadoH = ruta.TiempoEstimadoH;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Ruta actualizada exitosamente", ruta = rutaExistente });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RutaExists(id))
                {
                    return NotFound(new { message = $"Ruta con ID {id} no encontrada" });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar la ruta", error = ex.Message });
            }
        }

        // POST: api/Rutas
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Rutas>> PostRuta(Rutas ruta)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar campos requeridos
                if (string.IsNullOrWhiteSpace(ruta.Nombre))
                {
                    return BadRequest(new { message = "El nombre de la ruta es requerido" });
                }

                if (ruta.TiempoEstimadoH < 0)
                {
                    return BadRequest(new { message = "El tiempo estimado no puede ser negativo" });
                }

                // Validar que no exista una ruta con el mismo nombre
                var nombreExiste = await _context.Rutas.AnyAsync(r => r.Nombre == ruta.Nombre);
                if (nombreExiste)
                {
                    return BadRequest(new { message = "Ya existe una ruta con ese nombre" });
                }

                _context.Rutas.Add(ruta);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetRuta), new { id = ruta.Id }, ruta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear la ruta", error = ex.Message });
            }
        }

        // DELETE: api/Rutas/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRuta(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var ruta = await _context.Rutas
                    .Include(r => r.PedidoRutas)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (ruta == null)
                {
                    return NotFound(new { message = $"Ruta con ID {id} no encontrada" });
                }

                // Verificar si tiene pedidos asociados
                if (ruta.PedidoRutas.Any())
                {
                    return BadRequest(new 
                    { 
                        message = "No se puede eliminar la ruta porque tiene pedidos asociados",
                        pedidosCount = ruta.PedidoRutas.Count
                    });
                }

                _context.Rutas.Remove(ruta);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Ruta eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar la ruta", error = ex.Message });
            }
        }

        private bool RutaExists(int id)
        {
            return _context.Rutas.Any(e => e.Id == id);
        }
    }
}
