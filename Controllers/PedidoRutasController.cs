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
        public async Task<ActionResult<IEnumerable<PedidoRuta>>> GetPedidoRutas()
        {
            return await _context.PedidoRutas.ToListAsync();
        }

        // GET: api/PedidoRutas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoRuta>> GetPedidoRuta(int id)
        {
            var pedidoRuta = await _context.PedidoRutas.FindAsync(id);

            if (pedidoRuta == null)
            {
                return NotFound();
            }

            return pedidoRuta;
        }

        // PUT: api/PedidoRutas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedidoRuta(int id, PedidoRuta pedidoRuta)
        {
            if (id != pedidoRuta.Id)
            {
                return BadRequest();
            }

            _context.Entry(pedidoRuta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoRutaExists(id))
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

        // POST: api/PedidoRutas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PedidoRuta>> PostPedidoRuta(PedidoRuta pedidoRuta)
        {
            _context.PedidoRutas.Add(pedidoRuta);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPedidoRuta", new { id = pedidoRuta.Id }, pedidoRuta);
        }

        // DELETE: api/PedidoRutas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedidoRuta(int id)
        {
            var pedidoRuta = await _context.PedidoRutas.FindAsync(id);
            if (pedidoRuta == null)
            {
                return NotFound();
            }

            _context.PedidoRutas.Remove(pedidoRuta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PedidoRutaExists(int id)
        {
            return _context.PedidoRutas.Any(e => e.Id == id);
        }
    }
}
