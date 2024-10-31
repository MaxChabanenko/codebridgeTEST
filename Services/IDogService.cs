using codebridgeTEST.Models;

namespace codebridgeTEST.Services
{
    public interface IDogService
    {
        Task<IEnumerable<Dog>> GetDogsAsync(string attribute, string order, int? pageNumber, int? pageSize);
        Task<Dog> AddDogAsync(Dog dog);
    }
}
