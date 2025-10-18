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
    public class OrdenCompraTipoGranosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdenCompraTipoGranosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/OrdenCompraTipoGranos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdenCompraTipoGrano>>> GetOrdenCompraTipoGranos()
        {
            return await _context.OrdenCompraTipoGranos.ToListAsync();
        }

        // GET: api/OrdenCompraTipoGranos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenCompraTipoGrano>> GetOrdenCompraTipoGrano(int id)
        {
            var ordenCompraTipoGrano = await _context.OrdenCompraTipoGranos.FindAsync(id);

            if (ordenCompraTipoGrano == null)
            {
                return NotFound();
            }

            return ordenCompraTipoGrano;
        }

        // PUT: api/OrdenCompraTipoGranos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrdenCompraTipoGrano(int id, OrdenCompraTipoGrano ordenCompraTipoGrano)
        {
            if (id != ordenCompraTipoGrano.Id)
            {
                return BadRequest();
            }

            _context.Entry(ordenCompraTipoGrano).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdenCompraTipoGranoExists(id))
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

        // POST: api/OrdenCompraTipoGranos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrdenCompraTipoGrano>> PostOrdenCompraTipoGrano(OrdenCompraTipoGrano ordenCompraTipoGrano)
        {
            _context.OrdenCompraTipoGranos.Add(ordenCompraTipoGrano);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrdenCompraTipoGrano", new { id = ordenCompraTipoGrano.Id }, ordenCompraTipoGrano);
        }

        // DELETE: api/OrdenCompraTipoGranos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrdenCompraTipoGrano(int id)
        {
            var ordenCompraTipoGrano = await _context.OrdenCompraTipoGranos.FindAsync(id);
            if (ordenCompraTipoGrano == null)
            {
                return NotFound();
            }

            _context.OrdenCompraTipoGranos.Remove(ordenCompraTipoGrano);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrdenCompraTipoGranoExists(int id)
        {
            return _context.OrdenCompraTipoGranos.Any(e => e.Id == id);
        }
    }
}
