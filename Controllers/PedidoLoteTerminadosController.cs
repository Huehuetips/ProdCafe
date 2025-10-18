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
        public async Task<ActionResult<IEnumerable<PedidoLoteTerminado>>> GetPedidoLoteTerminados()
        {
            return await _context.PedidoLoteTerminados.ToListAsync();
        }

        // GET: api/PedidoLoteTerminados/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoLoteTerminado>> GetPedidoLoteTerminado(int id)
        {
            var pedidoLoteTerminado = await _context.PedidoLoteTerminados.FindAsync(id);

            if (pedidoLoteTerminado == null)
            {
                return NotFound();
            }

            return pedidoLoteTerminado;
        }

        // PUT: api/PedidoLoteTerminados/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedidoLoteTerminado(int id, PedidoLoteTerminado pedidoLoteTerminado)
        {
            if (id != pedidoLoteTerminado.Id)
            {
                return BadRequest();
            }

            _context.Entry(pedidoLoteTerminado).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoLoteTerminadoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PedidoLoteTerminados
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PedidoLoteTerminado>> PostPedidoLoteTerminado(PedidoLoteTerminado pedidoLoteTerminado)
        {
            _context.PedidoLoteTerminados.Add(pedidoLoteTerminado);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPedidoLoteTerminado", new { id = pedidoLoteTerminado.Id }, pedidoLoteTerminado);
        }

        // DELETE: api/PedidoLoteTerminados/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedidoLoteTerminado(int id)
        {
            var pedidoLoteTerminado = await _context.PedidoLoteTerminados.FindAsync(id);
            if (pedidoLoteTerminado == null)
            {
                return NotFound();
            }

            _context.PedidoLoteTerminados.Remove(pedidoLoteTerminado);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PedidoLoteTerminadoExists(int id)
        {
            return _context.PedidoLoteTerminados.Any(e => e.Id == id);
        }
    }
}
