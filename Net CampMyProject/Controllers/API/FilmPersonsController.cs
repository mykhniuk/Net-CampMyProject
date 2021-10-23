using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;

namespace Net_CampMyProject.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmPersonsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FilmPersonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FilmPersons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FilmPerson>>> GetFilmPersons()
        {
            return await _context.FilmPersons.ToListAsync();
        }

        // GET: api/FilmPersons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FilmPerson>> GetFilmPerson(int id)
        {
            var filmPerson = await _context.FilmPersons.FindAsync(id);

            if (filmPerson == null)
            {
                return NotFound();
            }

            return filmPerson;
        }

        // PUT: api/FilmPersons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFilmPerson(int id, FilmPerson filmPerson)
        {
            if (id != filmPerson.Id)
            {
                return BadRequest();
            }

            _context.Entry(filmPerson).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FilmPersonExists(id))
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

        // POST: api/FilmPersons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FilmPerson>> PostFilmPerson(FilmPerson filmPerson)
        {
            _context.FilmPersons.Add(filmPerson);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFilmPerson", new { id = filmPerson.Id }, filmPerson);
        }

        // DELETE: api/FilmPersons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFilmPerson(int id)
        {
            var filmPerson = await _context.FilmPersons.FindAsync(id);
            if (filmPerson == null)
            {
                return NotFound();
            }

            _context.FilmPersons.Remove(filmPerson);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FilmPersonExists(int id)
        {
            return _context.FilmPersons.Any(e => e.Id == id);
        }
    }
}
