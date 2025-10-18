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
    public class EtapasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EtapasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Etapas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Etapas>>> GetEtapas()
        {
            return await _context.Etapas.ToListAsync();
        }

        // GET: api/Etapas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Etapas>> GetEtapas(int id)
        {
            var etapas = await _context.Etapas.FindAsync(id);

            if (etapas == null)
            {
                return NotFound();
            }

            return etapas;
        }

        // PUT: api/Etapas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEtapas(int id, Etapas etapas)
        {
            if (id != etapas.Id)
            {
                return BadRequest();
            }

            _context.Entry(etapas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EtapasExists(id))
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

        // POST: api/Etapas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Etapas>> PostEtapas(Etapas etapas)
        {
            _context.Etapas.Add(etapas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEtapas", new { id = etapas.Id }, etapas);
        }

        // DELETE: api/Etapas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEtapas(int id)
        {
            var etapas = await _context.Etapas.FindAsync(id);
            if (etapas == null)
            {
                return NotFound();
            }

            _context.Etapas.Remove(etapas);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EtapasExists(int id)
        {
            return _context.Etapas.Any(e => e.Id == id);
        }
    }
}
