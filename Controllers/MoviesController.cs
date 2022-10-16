using System;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AlkemyChallenge.Models;
using AlkemyChallenge.Data;
using NHibernate.Util;

namespace AlkemyChallenge.Controllers
{
    [Route("movies")]
    [ApiController]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private readonly DataContext _dbContext;
        public MoviesController(DataContext context)
        {
            _dbContext = context;
        }
        [HttpGet]
        public IActionResult GetMovies([FromQuery] string name, [FromQuery] int genre = -1, [FromQuery] string order = "ASC")
        {
            List<Movie> movies = _dbContext.Movies.ToList();
            if (name != null)
            {
                movies = movies.Where(m => m.Title.Contains(name)).ToList();

            }
            if (order == "ASC")
            {
                movies = movies.OrderBy(m => m.Title).ToList();
            }
            if (order == "DESC")
            {
                movies = movies.OrderByDescending(m => m.Title).ToList();
            }

            if (genre != -1)
            {
                var genreObject = _dbContext.Genres.Find(genre);
                movies = movies.Where(c => c.Genres.Equals(genreObject)).ToList();
            }
            if (movies.Count == 0)
            {
                return NotFound();
            }
            return Ok(movies.Select(m =>
                new
                {
                    title = m.Title,
                    image = m.Img,
                    date = m.Date
                }
            ));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var movie = await _dbContext.Movies.Where(c => c.Id == id)
                .Include(c => c.Genres)
                .Include(c => c.Characters)
                .FirstOrDefaultAsync();
            if (movie == null)
            {
                return NotFound(
                    new
                    {
                        Status = "Not found",
                        Message = "No movies matches the id"
                    });
            }
            return Ok(movie);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _dbContext.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound(
                    new
                    {
                        Status = "Not found",
                        Message = "No movie matches the id"
                    });
            }
            try
            {
                _dbContext.Movies.Remove(movie);
                return Ok();
            }
            catch
            {
                return StatusCode(500, new
                {
                    Status = "Internal error",
                    Message = "Movie could not be deleted"
                });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, Movie updatedMovie)
        {
            var movie = await _dbContext.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound(new
                {
                    Status = "Not found",
                    Message = "No entity matches the id"
                });
            }
            movie = updatedMovie;
            try
            {
                _dbContext.SaveChanges();
                return Ok();
            }
            catch
            {
                return StatusCode(500, new
                {
                    Status = "Internal error",
                    Message = "Movie couldn't be updated"
                });
            }
        }
        [HttpPost]
        public async Task<ActionResult<List<Movie>>> PostMovie(Movie newMovie)
        {
            try
            {
                await _dbContext.Movies.AddAsync(newMovie);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return StatusCode(500, new
                {
                    Status = "Internal error",
                    Message = "Could not create Movie"
                });
            }
        }

    }
}
