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
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Productos
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Productos>>> GetProductos()
        {
            try
            {
                var productos = await _context.Productos
                    .Include(p => p.Presentacion)
                    .ToListAsync();
                
                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los productos", error = ex.Message });
            }
        }

        // GET: api/Productos/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Productos>> GetProducto(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var producto = await _context.Productos
                    .Include(p => p.Presentacion)
                    .Include(p => p.LotesTerminados)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (producto == null)
                {
                    return NotFound(new { message = $"Producto con ID {id} no encontrado" });
                }

                return Ok(producto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el producto", error = ex.Message });
            }
        }

        // PUT: api/Productos/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutProducto(int id, Productos producto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                if (id != producto.Id)
                {
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID del producto" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar campos requeridos
                if (string.IsNullOrWhiteSpace(producto.Nombre))
                {
                    return BadRequest(new { message = "El nombre del producto es requerido" });
                }

                if (producto.Precio < 0)
                {
                    return BadRequest(new { message = "El precio no puede ser negativo" });
                }

                // Validar que la presentación exista
                var presentacionExiste = await _context.Presentaciones.AnyAsync(p => p.Id == producto.PresentacionId);
                if (!presentacionExiste)
                {
                    return BadRequest(new { message = $"La presentación con ID {producto.PresentacionId} no existe" });
                }

                var productoExistente = await _context.Productos.FindAsync(id);
                if (productoExistente == null)
                {
                    return NotFound(new { message = $"Producto con ID {id} no encontrado" });
                }

                // Actualizar propiedades
                productoExistente.Nombre = producto.Nombre;
                productoExistente.PresentacionId = producto.PresentacionId;
                productoExistente.NivelTostado = producto.NivelTostado;
                productoExistente.TipoMolido = producto.TipoMolido;
                productoExistente.Precio = producto.Precio;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Producto actualizado exitosamente", producto = productoExistente });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
                {
                    return NotFound(new { message = $"Producto con ID {id} no encontrado" });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el producto", error = ex.Message });
            }
        }

        // POST: api/Productos
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Productos>> PostProducto(Productos producto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar campos requeridos
                if (string.IsNullOrWhiteSpace(producto.Nombre))
                {
                    return BadRequest(new { message = "El nombre del producto es requerido" });
                }

                if (producto.Precio < 0)
                {
                    return BadRequest(new { message = "El precio no puede ser negativo" });
                }

                // Validar que la presentación exista
                var presentacionExiste = await _context.Presentaciones.AnyAsync(p => p.Id == producto.PresentacionId);
                if (!presentacionExiste)
                {
                    return BadRequest(new { message = $"La presentación con ID {producto.PresentacionId} no existe" });
                }

                // Validar que no exista un producto con el mismo nombre y presentación
                var productoExiste = await _context.Productos
                    .AnyAsync(p => p.Nombre == producto.Nombre && p.PresentacionId == producto.PresentacionId);
                if (productoExiste)
                {
                    return BadRequest(new { message = "Ya existe un producto con ese nombre y presentación" });
                }

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                // Cargar la presentación para la respuesta
                await _context.Entry(producto).Reference(p => p.Presentacion).LoadAsync();

                return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el producto", error = ex.Message });
            }
        }

        // DELETE: api/Productos/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var producto = await _context.Productos
                    .Include(p => p.LotesTerminados)
                    .Include(p => p.PedidoLoteTerminados)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (producto == null)
                {
                    return NotFound(new { message = $"Producto con ID {id} no encontrado" });
                }

                // Verificar si tiene lotes terminados o pedidos asociados
                if (producto.LotesTerminados.Any() || producto.PedidoLoteTerminados.Any())
                {
                    return BadRequest(new 
                    { 
                        message = "No se puede eliminar el producto porque tiene lotes o pedidos asociados",
                        lotesCount = producto.LotesTerminados.Count,
                        pedidosCount = producto.PedidoLoteTerminados.Count
                    });
                }

                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Producto eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el producto", error = ex.Message });
            }
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}
