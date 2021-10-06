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
    public class FilmsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FilmsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MostPopularFilms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Film>>> GetFilms()
        {
            return await _context.Films.ToListAsync();
        }

        // GET: api/MostPopularFilms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Film>> GetMostPopularFilm(string id)
        {
            var mostPopularFilm = await _context.Films.FindAsync(id);

            if (mostPopularFilm == null)
            {
                return NotFound();
            }

            return mostPopularFilm;
        }

        // PUT: api/MostPopularFilms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMostPopularFilm(string id, Film film)
        {
            if (id != film.ImbId)
            {
                return BadRequest();
            }

            _context.Entry(film).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MostPopularFilmExists(id))
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

        // POST: api/MostPopularFilms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Film>> PostMostPopularFilm(Film film)
        {
            _context.Films.Add(film);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MostPopularFilmExists(film.ImbId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMostPopularFilm", new { id = film.ImbId }, film);
        }

        // DELETE: api/MostPopularFilms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMostPopularFilm(string id)
        {
            var mostPopularFilm = await _context.Films.FindAsync(id);
            if (mostPopularFilm == null)
            {
                return NotFound();
            }

            _context.Films.Remove(mostPopularFilm);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MostPopularFilmExists(string id)
        {
            return _context.Films.Any(e => e.ImbId == id);
        }

        
    }
}
