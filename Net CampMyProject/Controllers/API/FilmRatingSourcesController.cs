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
    public class FilmRatingSourcesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FilmRatingSourcesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FilmRatingSources
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FilmRatingSource>>> GetFilmRatingSources()
        {
            return await _context.FilmRatingSources.ToListAsync();
        }

        // GET: api/FilmRatingSources/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FilmRatingSource>> GetFilmRatingSource(int id)
        {
            var filmRatingSource = await _context.FilmRatingSources.FindAsync(id);

            if (filmRatingSource == null)
            {
                return NotFound();
            }

            return filmRatingSource;
        }

        // PUT: api/FilmRatingSources/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFilmRatingSource(int id, FilmRatingSource filmRatingSource)
        {
            if (id != filmRatingSource.Id)
            {
                return BadRequest();
            }

            _context.Entry(filmRatingSource).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FilmRatingSourceExists(id))
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

        // POST: api/FilmRatingSources
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FilmRatingSource>> PostFilmRatingSource(FilmRatingSource filmRatingSource)
        {
            _context.FilmRatingSources.Add(filmRatingSource);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFilmRatingSource", new { id = filmRatingSource.Id }, filmRatingSource);
        }

        // DELETE: api/FilmRatingSources/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFilmRatingSource(int id)
        {
            var filmRatingSource = await _context.FilmRatingSources.FindAsync(id);
            if (filmRatingSource == null)
            {
                return NotFound();
            }

            _context.FilmRatingSources.Remove(filmRatingSource);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FilmRatingSourceExists(int id)
        {
            return _context.FilmRatingSources.Any(e => e.Id == id);
        }
    }
}
