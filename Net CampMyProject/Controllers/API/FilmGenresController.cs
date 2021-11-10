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
    public class FilmGenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FilmGenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FilmGenres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FilmGenre>>> GetFilmGenre()
        {
            return await _context.FilmGenres.ToListAsync();
        }

        // GET: api/FilmGenres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FilmGenre>> GetFilmGenre(int id)
        {
            var filmGenre = await _context.FilmGenres.FindAsync(id);

            if (filmGenre == null)
            {
                return NotFound();
            }

            return filmGenre;
        }

        // PUT: api/FilmGenres/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFilmGenre(int id, FilmGenre filmGenre)
        {
            if (id != filmGenre.Id)
            {
                return BadRequest();
            }

            _context.Entry(filmGenre).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FilmGenreExists(id))
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

        // POST: api/FilmGenres
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FilmGenre>> PostFilmGenre(FilmGenre filmGenre)
        {
            var dbFilmGenre = await _context.FilmGenres
                .AsNoTracking()
                .FirstOrDefaultAsync(fg => fg.FilmId == filmGenre.FilmId && fg.GenreId == filmGenre.GenreId);

            if (dbFilmGenre == null)
            {
                _context.FilmGenres.Add(filmGenre);
                await _context.SaveChangesAsync();

                dbFilmGenre = filmGenre;
            }

            return CreatedAtAction("GetFilmGenre", new { id = dbFilmGenre.Id }, dbFilmGenre);
        }

        // DELETE: api/FilmGenres/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFilmGenre(int id)
        {
            var filmGenre = await _context.FilmGenres.FindAsync(id);
            if (filmGenre == null)
            {
                return NotFound();
            }

            _context.FilmGenres.Remove(filmGenre);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FilmGenreExists(int id)
        {
            return _context.FilmGenres.Any(e => e.Id == id);
        }
    }
}
