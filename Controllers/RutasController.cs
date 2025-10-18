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
        public async Task<ActionResult<IEnumerable<Rutas>>> GetRutas()
        {
            return await _context.Rutas.ToListAsync();
        }

        // GET: api/Rutas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rutas>> GetRutas(int id)
        {
            var rutas = await _context.Rutas.FindAsync(id);

            if (rutas == null)
            {
                return NotFound();
            }

            return rutas;
        }

        // PUT: api/Rutas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRutas(int id, Rutas rutas)
        {
            if (id != rutas.Id)
            {
                return BadRequest();
            }

            _context.Entry(rutas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RutasExists(id))
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

        // POST: api/Rutas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Rutas>> PostRutas(Rutas rutas)
        {
            _context.Rutas.Add(rutas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRutas", new { id = rutas.Id }, rutas);
        }

        // DELETE: api/Rutas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRutas(int id)
        {
            var rutas = await _context.Rutas.FindAsync(id);
            if (rutas == null)
            {
                return NotFound();
            }

            _context.Rutas.Remove(rutas);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RutasExists(int id)
        {
            return _context.Rutas.Any(e => e.Id == id);
        }
    }
}
