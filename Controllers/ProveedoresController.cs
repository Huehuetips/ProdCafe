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
    public class ProveedoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProveedoresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Proveedores
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Proveedores>>> GetProveedores()
        {
            try
            {
                var proveedores = await _context.Proveedores
                    .Include(p => p.OrdenesCompras)
                    .ToListAsync();
                
                return Ok(proveedores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los proveedores", error = ex.Message });
            }
        }

        // GET: api/Proveedores/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Proveedores>> GetProveedor(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var proveedor = await _context.Proveedores
                    .Include(p => p.OrdenesCompras)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (proveedor == null)
                {
                    return NotFound(new { message = $"Proveedor con ID {id} no encontrado" });
                }

                return Ok(proveedor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el proveedor", error = ex.Message });
            }
        }

        // PUT: api/Proveedores/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutProveedor(int id, Proveedores proveedor)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                if (id != proveedor.Id)
                {
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID del proveedor" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el nombre no esté vacío
                if (string.IsNullOrWhiteSpace(proveedor.Nombre))
                {
                    return BadRequest(new { message = "El nombre del proveedor es requerido" });
                }

                var proveedorExistente = await _context.Proveedores.FindAsync(id);
                if (proveedorExistente == null)
                {
                    return NotFound(new { message = $"Proveedor con ID {id} no encontrado" });
                }

                // Actualizar propiedades
                proveedorExistente.Nombre = proveedor.Nombre;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Proveedor actualizado exitosamente", proveedor = proveedorExistente });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProveedorExists(id))
                {
                    return NotFound(new { message = $"Proveedor con ID {id} no encontrado" });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el proveedor", error = ex.Message });
            }
        }

        // POST: api/Proveedores
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Proveedores>> PostProveedor(Proveedores proveedor)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el nombre no esté vacío
                if (string.IsNullOrWhiteSpace(proveedor.Nombre))
                {
                    return BadRequest(new { message = "El nombre del proveedor es requerido" });
                }

                // Validar que no exista un proveedor con el mismo nombre
                var nombreExiste = await _context.Proveedores.AnyAsync(p => p.Nombre == proveedor.Nombre);
                if (nombreExiste)
                {
                    return BadRequest(new { message = "Ya existe un proveedor con ese nombre" });
                }

                _context.Proveedores.Add(proveedor);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProveedor), new { id = proveedor.Id }, proveedor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el proveedor", error = ex.Message });
            }
        }

        // DELETE: api/Proveedores/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var proveedor = await _context.Proveedores
                    .Include(p => p.OrdenesCompras)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (proveedor == null)
                {
                    return NotFound(new { message = $"Proveedor con ID {id} no encontrado" });
                }

                // Verificar si tiene órdenes de compra asociadas
                if (proveedor.OrdenesCompras.Any())
                {
                    return BadRequest(new 
                    { 
                        message = "No se puede eliminar el proveedor porque tiene órdenes de compra asociadas",
                        ordenesCount = proveedor.OrdenesCompras.Count
                    });
                }

                _context.Proveedores.Remove(proveedor);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Proveedor eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el proveedor", error = ex.Message });
            }
        }

        private bool ProveedorExists(int id)
        {
            return _context.Proveedores.Any(e => e.Id == id);
        }
    }
}
