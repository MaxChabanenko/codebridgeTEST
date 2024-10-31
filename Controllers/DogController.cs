using codebridgeTEST.Data;
using codebridgeTEST.Models;
using codebridgeTEST.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace codebridgeTEST.Controllers
{
    [ApiController]
    [Route("[action]")]
    public class DogController : ControllerBase
    {
        private readonly IDogService _dogService;

        public DogController(IDogService dogService)
        {
            _dogService = dogService;
        }

        [HttpGet]
        public IActionResult ping()
        {
            return Ok("Dogshouseservice.Version1.0.1");
        }

        //for curl request cover url in brakets 
        [HttpGet]
        public async Task<IActionResult> dogs(
                    [FromQuery] string attribute = null,
                    [FromQuery] string order = null,
                    [FromQuery] int? pageNumber = null,
                    [FromQuery] int? pageSize = null)
        {
            try
            {
                var dogs = await _dogService.GetDogsAsync(attribute, order, pageNumber, pageSize);
                return Ok(dogs);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }

        }
        //curl -X POST http://localhost/dog -H "Content-Type: application/json" -d "{\"name\": \"Doggy\", \"color\": \"red\", \"tail_length\": 173, \"weight\": 33}"
        [HttpPost]
        public async Task<IActionResult> dog(Dog dog)
        {
            try
            {
                var addedDog = await _dogService.AddDogAsync(dog);
                return Ok("Dog added");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }
        }

    }
}
