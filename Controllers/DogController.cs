using codebridgeTEST.Data;
using codebridgeTEST.Models;
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
        private readonly DataContext _context;
        public DogController(DataContext context)
        {
            _context = context;
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
            IQueryable<Dog> query = _context.Dogs;

            //pagination
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                if (pageNumber <= 0|| pageSize <= 0)
                {
                    return BadRequest("Page values must be positive ");
                }

                query = query.Skip((pageNumber.Value - 1) * pageSize.Value)
                             .Take(pageSize.Value);
            }
            //sorting
            if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(order))
            {
                var propertyInfo = typeof(Dog).GetProperties()
                    .FirstOrDefault(p => p.Name.Equals(attribute, StringComparison.OrdinalIgnoreCase));

                if (propertyInfo == null)
                {
                    return BadRequest($"Attribute '{attribute}' does not exist");
                }
                if (order.ToLower() != "desc"&& order.ToLower() != "asc")
                {
                    return BadRequest($"Order only 'desc' and 'asc'");
                }

                query = order.ToLower() == "desc"
                    ? query.OrderByDescending(d => EF.Property<object>(d, propertyInfo.Name)) 
                    : query.OrderBy(d => EF.Property<object>(d, propertyInfo.Name));
            }
        

            var dogs = await query.ToListAsync();

            return Ok(dogs);
        }
    }
}
