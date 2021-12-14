using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;

namespace Net_CampMyProject.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.Admin)]
    public class FilmRatingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FilmRatingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FilmRatings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FilmRating>>> GetFilmRatings()
        {
            return await _context.FilmRatings.ToListAsync();
        }

        // GET: api/FilmRatings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FilmRating>> GetFilmRating(int id)
        {
            var filmRating = await _context.FilmRatings.FindAsync(id);

            if (filmRating == null)
            {
                return NotFound();
            }

            return filmRating;
        }

        // PUT: api/FilmRatings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFilmRating(int id, FilmRating filmRating)
        {
            if (id != filmRating.Id)
            {
                return BadRequest();
            }

            _context.Entry(filmRating).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FilmRatingExists(id))
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

        // POST: api/FilmRatings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FilmRating>> PostFilmRating(FilmRating filmRating)
        {
            _context.FilmRatings.Add(filmRating);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFilmRating", new { id = filmRating.Id }, filmRating);
        }

        // DELETE: api/FilmRatings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFilmRating(int id)
        {
            var filmRating = await _context.FilmRatings.FindAsync(id);
            if (filmRating == null)
            {
                return NotFound();
            }

            _context.FilmRatings.Remove(filmRating);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FilmRatingExists(int id)
        {
            return _context.FilmRatings.Any(e => e.Id == id);
        }
    }
}
