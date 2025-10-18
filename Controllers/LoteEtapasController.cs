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
    public class LoteEtapasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoteEtapasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/LoteEtapas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoteEtapa>>> GetLoteEtapas()
        {
            return await _context.LoteEtapas.ToListAsync();
        }

        // GET: api/LoteEtapas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoteEtapa>> GetLoteEtapa(int id)
        {
            var loteEtapa = await _context.LoteEtapas.FindAsync(id);

            if (loteEtapa == null)
            {
                return NotFound();
            }

            return loteEtapa;
        }

        // PUT: api/LoteEtapas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoteEtapa(int id, LoteEtapa loteEtapa)
        {
            if (id != loteEtapa.Id)
            {
                return BadRequest();
            }

            _context.Entry(loteEtapa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoteEtapaExists(id))
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

        // POST: api/LoteEtapas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LoteEtapa>> PostLoteEtapa(LoteEtapa loteEtapa)
        {
            _context.LoteEtapas.Add(loteEtapa);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLoteEtapa", new { id = loteEtapa.Id }, loteEtapa);
        }

        // DELETE: api/LoteEtapas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoteEtapa(int id)
        {
            var loteEtapa = await _context.LoteEtapas.FindAsync(id);
            if (loteEtapa == null)
            {
                return NotFound();
            }

            _context.LoteEtapas.Remove(loteEtapa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LoteEtapaExists(int id)
        {
            return _context.LoteEtapas.Any(e => e.Id == id);
        }
    }
}
