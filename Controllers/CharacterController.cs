using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AlkemyChallenge.Models;
using AlkemyChallenge.Data;
using NHibernate.Util;

namespace AlkemyChallenge.Controllers
{
    [Route("character")]
    [ApiController]
    [Authorize]
    public class CharacterController : ControllerBase
    {
        private readonly DataContext _dbContext;
        public CharacterController(DataContext context)
        {
            _dbContext = context;
        }
        [HttpGet]
        public IActionResult GetCharacters([FromQuery] string name, [FromQuery] int age = -1, [FromQuery] int movieId = -1)
        {

            List<Character> characters = _dbContext.Characters.ToList(); 
            if(name != null)
            {
                characters = characters.Where(c => c.Name.Contains(name)).ToList();

            }
            if(age >= 0)
            {
                characters = characters.Where(c => c.Age >= age).ToList();
            }

            if(movieId != -1)
            {
                var movie = _dbContext.Movies.Find(movieId);
                characters = characters.Where(c => c.Movies.Equals(movie)).ToList();
            }
            if (characters.Count == 0)
            {
                return NotFound();
            }            
            return Ok(characters);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var character = await _dbContext.Characters.Where(c => c.Id == id).Include(c => c.Movies).FirstOrDefaultAsync();
            if(character == null)
            {
                return NotFound(
                    new
                    {
                        Status = "Not found",
                        Message = "No character matches the id"
                    });
            }
            return Ok(character);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacter(int id)
        {
            var character = await _dbContext.Characters.FindAsync(id);
            if(character == null)
            {
                return NotFound(
                    new
                    {
                        Status = "Not found",
                        Message = "No character matches the id"
                    });
            }
            try
            {
                _dbContext.Characters.Remove(character);
                return Ok();
            }
            catch
            {
                return StatusCode(500, new
                {
                    Status = "Internal error",
                    Message = "Character could not be deleted"
                });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCharacter(int id, Character updatedCharacter)
        {
            var character = await _dbContext.Characters.FindAsync(id);
            if(character == null )
            {
                return NotFound(new
                {
                    Status = "Not found",
                    Message = "No entity matches the id"
                });
            }
            character = updatedCharacter;
            try
            {
                _dbContext.SaveChanges();
                return Ok();
            } catch
            {
                return StatusCode(500, new
                {
                    Status = "Internal error",
                    Message = "Character couldn't be updated"
                });
            }
        }
        [HttpPost]
        public async Task<ActionResult<List<Character>>> PostCharacter(Character newCharacter)
        {
            try
            {
                await _dbContext.Characters.AddAsync(newCharacter);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return StatusCode(500, new
                {
                    Status = "Internal error",
                    Message = "Could not create character"
                });
            }
        }
        
    }
}
