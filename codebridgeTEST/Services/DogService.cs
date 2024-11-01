using codebridgeTEST.Data;
using codebridgeTEST.Models;
using Microsoft.EntityFrameworkCore;

namespace codebridgeTEST.Services
{
    public class DogService : IDogService
    {
        private readonly DataContext _context;

        public DogService(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dog>> GetDogsAsync(string attribute, string order, int? pageNumber, int? pageSize)
        {
            IQueryable<Dog> query = _context.Dogs;

            
            //sorting first
            if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(order))
            {
                var propertyInfo = typeof(Dog).GetProperties()
                    .FirstOrDefault(p => p.Name.Equals(attribute, StringComparison.OrdinalIgnoreCase));

                if (propertyInfo == null)
                {
                    throw new ArgumentException(($"Attribute '{attribute}' does not exist"));
                }
                if (order.ToLower() != "desc" && order.ToLower() != "asc")
                {
                    throw new ArgumentException(($"Order only 'desc' and 'asc'"));
                }

                query = order.ToLower() == "desc"
                    ? query.OrderByDescending(d => EF.Property<object>(d, propertyInfo.Name))
                    : query.OrderBy(d => EF.Property<object>(d, propertyInfo.Name));
            }
            //pagination second
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                if (pageNumber <= 0 || pageSize <= 0)
                {
                    throw new ArgumentException("Page values must be positive ");
                }

                query = query.Skip((pageNumber.Value - 1) * pageSize.Value)
                             .Take(pageSize.Value);
            }
            return await query.ToListAsync();
        }

        public async Task<Dog> AddDogAsync(Dog dog)
        {
            if (dog == null)
            {
                throw new ArgumentException("Invalid JSON is passed in a request body.");
            }

            //Dog with the same name already exists in DB.
            var existingDog = await _context.Dogs
                .FirstOrDefaultAsync(d => d.Name.ToLower() == dog.Name.ToLower() && d.TailLength == dog.TailLength);

            if (existingDog != null)
            {
                throw new ArgumentException("Dog with the same name and tail length already exists in DB.");
            }

            if (dog.TailLength < 0)
            {
                throw new ArgumentException("Tail height is a negative number or is not a number.");
            }

            // Other cases that need to be handled in order for API to work properly.
            if (dog.Weight <= 0)
            {
                throw new ArgumentException("Weight must be a bigger then zero");
            }

            _context.Dogs.Add(dog);
            await _context.SaveChangesAsync();

            return dog; 
        }
    }
}
