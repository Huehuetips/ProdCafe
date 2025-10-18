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
    public class LotesTerminadosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LotesTerminadosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/LotesTerminados
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LotesTerminados>>> GetLotesTerminados()
        {
            return await _context.LotesTerminados.ToListAsync();
        }

        // GET: api/LotesTerminados/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LotesTerminados>> GetLotesTerminados(int id)
        {
            var lotesTerminados = await _context.LotesTerminados.FindAsync(id);

            if (lotesTerminados == null)
            {
                return NotFound();
            }

            return lotesTerminados;
        }

        // PUT: api/LotesTerminados/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLotesTerminados(int id, LotesTerminados lotesTerminados)
        {
            if (id != lotesTerminados.Id)
            {
                return BadRequest();
            }

            _context.Entry(lotesTerminados).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LotesTerminadosExists(id))
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

        // POST: api/LotesTerminados
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LotesTerminados>> PostLotesTerminados(LotesTerminados lotesTerminados)
        {
            _context.LotesTerminados.Add(lotesTerminados);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLotesTerminados", new { id = lotesTerminados.Id }, lotesTerminados);
        }

        // DELETE: api/LotesTerminados/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLotesTerminados(int id)
        {
            var lotesTerminados = await _context.LotesTerminados.FindAsync(id);
            if (lotesTerminados == null)
            {
                return NotFound();
            }

            _context.LotesTerminados.Remove(lotesTerminados);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LotesTerminadosExists(int id)
        {
            return _context.LotesTerminados.Any(e => e.Id == id);
        }
    }
}
