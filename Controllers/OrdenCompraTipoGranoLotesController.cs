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
    public class OrdenCompraTipoGranoLotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdenCompraTipoGranoLotesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/OrdenCompraTipoGranoLotes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdenCompraTipoGranoLote>>> GetOrdenCompraTipoGranoLotes()
        {
            return await _context.OrdenCompraTipoGranoLotes.ToListAsync();
        }

        // GET: api/OrdenCompraTipoGranoLotes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenCompraTipoGranoLote>> GetOrdenCompraTipoGranoLote(int id)
        {
            var ordenCompraTipoGranoLote = await _context.OrdenCompraTipoGranoLotes.FindAsync(id);

            if (ordenCompraTipoGranoLote == null)
            {
                return NotFound();
            }

            return ordenCompraTipoGranoLote;
        }

        // PUT: api/OrdenCompraTipoGranoLotes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrdenCompraTipoGranoLote(int id, OrdenCompraTipoGranoLote ordenCompraTipoGranoLote)
        {
            if (id != ordenCompraTipoGranoLote.Id)
            {
                return BadRequest();
            }

            _context.Entry(ordenCompraTipoGranoLote).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdenCompraTipoGranoLoteExists(id))
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

        // POST: api/OrdenCompraTipoGranoLotes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrdenCompraTipoGranoLote>> PostOrdenCompraTipoGranoLote(OrdenCompraTipoGranoLote ordenCompraTipoGranoLote)
        {
            _context.OrdenCompraTipoGranoLotes.Add(ordenCompraTipoGranoLote);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrdenCompraTipoGranoLote", new { id = ordenCompraTipoGranoLote.Id }, ordenCompraTipoGranoLote);
        }

        // DELETE: api/OrdenCompraTipoGranoLotes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrdenCompraTipoGranoLote(int id)
        {
            var ordenCompraTipoGranoLote = await _context.OrdenCompraTipoGranoLotes.FindAsync(id);
            if (ordenCompraTipoGranoLote == null)
            {
                return NotFound();
            }

            _context.OrdenCompraTipoGranoLotes.Remove(ordenCompraTipoGranoLote);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrdenCompraTipoGranoLoteExists(int id)
        {
            return _context.OrdenCompraTipoGranoLotes.Any(e => e.Id == id);
        }
    }
}
