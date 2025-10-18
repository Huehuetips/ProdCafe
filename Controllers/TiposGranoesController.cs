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
    public class TiposGranoesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TiposGranoesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TiposGranoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TiposGrano>>> GetTiposGranos()
        {
            return await _context.TiposGranos.ToListAsync();
        }

        // GET: api/TiposGranoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TiposGrano>> GetTiposGrano(int id)
        {
            var tiposGrano = await _context.TiposGranos.FindAsync(id);

            if (tiposGrano == null)
            {
                return NotFound();
            }

            return tiposGrano;
        }

        // PUT: api/TiposGranoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTiposGrano(int id, TiposGrano tiposGrano)
        {
            if (id != tiposGrano.Id)
            {
                return BadRequest();
            }

            _context.Entry(tiposGrano).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TiposGranoExists(id))
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

        // POST: api/TiposGranoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TiposGrano>> PostTiposGrano(TiposGrano tiposGrano)
        {
            _context.TiposGranos.Add(tiposGrano);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTiposGrano", new { id = tiposGrano.Id }, tiposGrano);
        }

        // DELETE: api/TiposGranoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTiposGrano(int id)
        {
            var tiposGrano = await _context.TiposGranos.FindAsync(id);
            if (tiposGrano == null)
            {
                return NotFound();
            }

            _context.TiposGranos.Remove(tiposGrano);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TiposGranoExists(int id)
        {
            return _context.TiposGranos.Any(e => e.Id == id);
        }
    }
}
