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
    public class CatacionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CatacionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Catacions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Catacion>>> GetCataciones()
        {
            return await _context.Cataciones.ToListAsync();
        }

        // GET: api/Catacions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Catacion>> GetCatacion(int id)
        {
            var catacion = await _context.Cataciones.FindAsync(id);

            if (catacion == null)
            {
                return NotFound();
            }

            return catacion;
        }

        // PUT: api/Catacions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCatacion(int id, Catacion catacion)
        {
            if (id != catacion.Id)
            {
                return BadRequest();
            }

            _context.Entry(catacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CatacionExists(id))
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

        // POST: api/Catacions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Catacion>> PostCatacion(Catacion catacion)
        {
            _context.Cataciones.Add(catacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCatacion", new { id = catacion.Id }, catacion);
        }

        // DELETE: api/Catacions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCatacion(int id)
        {
            var catacion = await _context.Cataciones.FindAsync(id);
            if (catacion == null)
            {
                return NotFound();
            }

            _context.Cataciones.Remove(catacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CatacionExists(int id)
        {
            return _context.Cataciones.Any(e => e.Id == id);
        }
    }
}
