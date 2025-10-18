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
    public class OrdenesComprasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdenesComprasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/OrdenesCompras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdenesCompra>>> GetOrdenesCompras()
        {
            return await _context.OrdenesCompras.ToListAsync();
        }

        // GET: api/OrdenesCompras/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenesCompra>> GetOrdenesCompra(int id)
        {
            var ordenesCompra = await _context.OrdenesCompras.FindAsync(id);

            if (ordenesCompra == null)
            {
                return NotFound();
            }

            return ordenesCompra;
        }

        // PUT: api/OrdenesCompras/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrdenesCompra(int id, OrdenesCompra ordenesCompra)
        {
            if (id != ordenesCompra.Id)
            {
                return BadRequest();
            }

            _context.Entry(ordenesCompra).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdenesCompraExists(id))
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

        // POST: api/OrdenesCompras
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrdenesCompra>> PostOrdenesCompra(OrdenesCompra ordenesCompra)
        {
            _context.OrdenesCompras.Add(ordenesCompra);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrdenesCompra", new { id = ordenesCompra.Id }, ordenesCompra);
        }

        // DELETE: api/OrdenesCompras/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrdenesCompra(int id)
        {
            var ordenesCompra = await _context.OrdenesCompras.FindAsync(id);
            if (ordenesCompra == null)
            {
                return NotFound();
            }

            _context.OrdenesCompras.Remove(ordenesCompra);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrdenesCompraExists(int id)
        {
            return _context.OrdenesCompras.Any(e => e.Id == id);
        }
    }
}
