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
    public class LotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LotesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Lotes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lotes>>> GetLotes()
        {
            return await _context.Lotes.ToListAsync();
        }

        // GET: api/Lotes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Lotes>> GetLotes(int id)
        {
            var lotes = await _context.Lotes.FindAsync(id);

            if (lotes == null)
            {
                return NotFound();
            }

            return lotes;
        }

        // PUT: api/Lotes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLotes(int id, Lotes lotes)
        {
            if (id != lotes.Id)
            {
                return BadRequest();
            }

            _context.Entry(lotes).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LotesExists(id))
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

        // POST: api/Lotes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Lotes>> PostLotes(Lotes lotes)
        {
            _context.Lotes.Add(lotes);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLotes", new { id = lotes.Id }, lotes);
        }

        // DELETE: api/Lotes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLotes(int id)
        {
            var lotes = await _context.Lotes.FindAsync(id);
            if (lotes == null)
            {
                return NotFound();
            }

            _context.Lotes.Remove(lotes);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LotesExists(int id)
        {
            return _context.Lotes.Any(e => e.Id == id);
        }
    }
}
