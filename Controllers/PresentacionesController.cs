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
    public class PresentacionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PresentacionesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Presentaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Presentaciones>>> GetPresentaciones()
        {
            return await _context.Presentaciones.ToListAsync();
        }

        // GET: api/Presentaciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Presentaciones>> GetPresentaciones(int id)
        {
            var presentaciones = await _context.Presentaciones.FindAsync(id);

            if (presentaciones == null)
            {
                return NotFound();
            }

            return presentaciones;
        }

        // PUT: api/Presentaciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPresentaciones(int id, Presentaciones presentaciones)
        {
            if (id != presentaciones.Id)
            {
                return BadRequest();
            }

            _context.Entry(presentaciones).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PresentacionesExists(id))
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

        // POST: api/Presentaciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Presentaciones>> PostPresentaciones(Presentaciones presentaciones)
        {
            _context.Presentaciones.Add(presentaciones);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPresentaciones", new { id = presentaciones.Id }, presentaciones);
        }

        // DELETE: api/Presentaciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePresentaciones(int id)
        {
            var presentaciones = await _context.Presentaciones.FindAsync(id);
            if (presentaciones == null)
            {
                return NotFound();
            }

            _context.Presentaciones.Remove(presentaciones);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PresentacionesExists(int id)
        {
            return _context.Presentaciones.Any(e => e.Id == id);
        }
    }
}
