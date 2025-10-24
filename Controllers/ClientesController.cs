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
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Clientes
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Clientes>>> GetClientes()
        {
            try
            {
                var clientes = await _context.Clientes
                    .Include(c => c.Pedidos)
                    .ToListAsync();
                
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los clientes", error = ex.Message });
            }
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Clientes>> GetCliente(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var cliente = await _context.Clientes
                    .Include(c => c.Pedidos)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cliente == null)
                {
                    return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el cliente", error = ex.Message });
            }
        }

        // PUT: api/Clientes/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutCliente(int id, Clientes cliente)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                if (id != cliente.Id)
                {
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID del cliente" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el nombre no esté vacío
                if (string.IsNullOrWhiteSpace(cliente.Nombre))
                {
                    return BadRequest(new { message = "El nombre del cliente es requerido" });
                }

                // Validar formato de email si se proporciona
                if (!string.IsNullOrWhiteSpace(cliente.Email) && !IsValidEmail(cliente.Email))
                {
                    return BadRequest(new { message = "El formato del email no es válido" });
                }

                var clienteExistente = await _context.Clientes.FindAsync(id);
                if (clienteExistente == null)
                {
                    return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
                }

                // Actualizar propiedades
                clienteExistente.Nombre = cliente.Nombre;
                clienteExistente.Tipo = cliente.Tipo;
                clienteExistente.Contacto = cliente.Contacto;
                clienteExistente.Direccion = cliente.Direccion;
                clienteExistente.Telefono = cliente.Telefono;
                clienteExistente.Email = cliente.Email;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Cliente actualizado exitosamente", cliente = clienteExistente });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(id))
                {
                    return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el cliente", error = ex.Message });
            }
        }

        // POST: api/Clientes
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Clientes>> PostCliente(Clientes cliente)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que el nombre no esté vacío
                if (string.IsNullOrWhiteSpace(cliente.Nombre))
                {
                    return BadRequest(new { message = "El nombre del cliente es requerido" });
                }

                // Validar formato de email si se proporciona
                if (!string.IsNullOrWhiteSpace(cliente.Email) && !IsValidEmail(cliente.Email))
                {
                    return BadRequest(new { message = "El formato del email no es válido" });
                }

                // Validar que no exista un cliente con el mismo email
                if (!string.IsNullOrWhiteSpace(cliente.Email))
                {
                    var emailExiste = await _context.Clientes.AnyAsync(c => c.Email == cliente.Email);
                    if (emailExiste)
                    {
                        return BadRequest(new { message = "Ya existe un cliente con ese email" });
                    }
                }

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el cliente", error = ex.Message });
            }
        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID debe ser mayor que 0" });
                }

                var cliente = await _context.Clientes
                    .Include(c => c.Pedidos)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cliente == null)
                {
                    return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
                }

                // Verificar si tiene pedidos asociados
                if (cliente.Pedidos.Any())
                {
                    return BadRequest(new 
                    { 
                        message = "No se puede eliminar el cliente porque tiene pedidos asociados",
                        pedidosCount = cliente.Pedidos.Count
                    });
                }

                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Cliente eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el cliente", error = ex.Message });
            }
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
