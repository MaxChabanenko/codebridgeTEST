
using codebridgeTEST.Controllers;
using codebridgeTEST.Middleware;
using codebridgeTEST.Models;
using codebridgeTEST.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Moq;
using System.Net;

namespace TestingProject
{
    public class RESTTests
    {
        private readonly Mock<IDogService> _mockDogService;
        private readonly DogController _controller;

        public RESTTests()
        {
            _mockDogService = new Mock<IDogService>();
            _controller = new DogController(_mockDogService.Object);
        }

        [Fact]
        public async Task Ping_Valid()
        {
            // Act
            var result = _controller.ping();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Dogshouseservice.Version1.0.1", okResult.Value);
        }
        #region get
        [Fact]
        public async Task GetDogs_Valid()
        {
            // Arrange
            var dogs = new List<Dog>
        {
            new Dog { Name = "Mark", TailLength = 24, Color = "red", Weight = 15 },
            new Dog { Name = "Renton", TailLength = 16, Color = "brown", Weight = 20 }
        };
            _mockDogService.Setup(service => service.GetDogsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(dogs);

            // Act
            var result = await _controller.dogs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnDogs = Assert.IsAssignableFrom<List<Dog>>(okResult.Value);
            Assert.Equal(2, returnDogs.Count);
        }
        [Fact]
        public async Task GetDogs_ValidPagination()
        {
            // Arrange
            var dogs = new List<Dog>
    {
        new Dog { Name = "Max", TailLength = 5, Color = "black", Weight = 12 },
        new Dog { Name = "Bella", TailLength = 10, Color = "brown", Weight = 15 }

    };

            int pageNumber = 2;
            int pageSize = 1;

            _mockDogService.Setup(service => service.GetDogsAsync(null, null, pageNumber, pageSize))
                .ReturnsAsync(dogs.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList());


            // Act
            var result = await _controller.dogs(null, null, pageNumber, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDogs = Assert.IsType<List<Dog>>(okResult.Value);
            Assert.Equal(dogs[1], returnedDogs.First());
        }
        [Fact]
        public async Task GetDogs_ValidSorting()
        {
            // Arrange
            var dogs = new List<Dog>
    {
        new Dog { Name = "Max", TailLength = 5, Color = "black", Weight = 12 },
        new Dog { Name = "Bella", TailLength = 10, Color = "brown", Weight = 15 }
    };

            string attribute = "Name";
            string order = "asc";

            _mockDogService.Setup(service => service.GetDogsAsync(attribute, order, null, null))
                .ReturnsAsync(dogs.OrderBy(d => d.Name).ToList());

            // Act
            var result = await _controller.dogs(attribute, order, null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDogs = Assert.IsType<List<Dog>>(okResult.Value);
            Assert.Equal(dogs[1], returnedDogs.First());
        }

        [Fact]
        public async Task GetDogs_ValidPaginationAndSorting()
        {
            // Arrange
            var dogs = new List<Dog>
    {
        new Dog { Name = "Bella", TailLength = 10, Color = "brown", Weight = 15 },
        new Dog { Name = "Martin", TailLength = 7, Color = "golden", Weight = 20 },
        new Dog { Name = "William", TailLength = 5, Color = "black", Weight = 12 }
    };

            int pageNumber = 1;
            int pageSize = 2;
            string attribute = "Name";
            string order = "asc";

            var expectedDogs = dogs.OrderBy(d => d.Name)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToList();

            _mockDogService.Setup(service => service.GetDogsAsync(attribute, order, pageNumber, pageSize))
                .ReturnsAsync(expectedDogs);

            // Act
            var result = await _controller.dogs(attribute, order, pageNumber, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDogs = Assert.IsType<List<Dog>>(okResult.Value);
            Assert.Equal(expectedDogs.Count, returnedDogs.Count);
            Assert.Equal(expectedDogs, returnedDogs);
        }


        [Fact]
        public async Task GetDogs_InvalidPagination()
        {
            // Arrange
            var invalidPageNumber = -1;
            var invalidPageSize = -1;

            _mockDogService.Setup(service => service.GetDogsAsync(null, null, invalidPageNumber, invalidPageSize))
                .ThrowsAsync(new ArgumentException("Page values must be positive"));

            // Act & Assert
            var result = await _controller.dogs(null, null, invalidPageNumber, invalidPageSize);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Page values must be positive", badRequestResult.Value);

        }
        [Fact]
        public async Task GetDogs_InvalidAttribute()
        {
            // Arrange
            string invalidAttribute = "invalid";
            string validOrder = "asc";
            _mockDogService.Setup(service => service.GetDogsAsync(invalidAttribute, validOrder, null, null))
                .ThrowsAsync(new ArgumentException($"Attribute '{invalidAttribute}' does not exist"));

            // Act
            var result = await _controller.dogs(invalidAttribute, validOrder);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"Attribute '{invalidAttribute}' does not exist", badRequestResult.Value);
        }
        [Fact]
        public async Task GetDogs_InvalidOrder()
        {
            // Arrange
            string validAttribute = "Color";
            string invalidOrder = "invalid";
            _mockDogService.Setup(service => service.GetDogsAsync(validAttribute, invalidOrder, null, null))
                .ThrowsAsync(new ArgumentException("Order only 'desc' and 'asc'"));

            // Act
            var result = await _controller.dogs(validAttribute, invalidOrder);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Order only 'desc' and 'asc'", badRequestResult.Value);
        }

        #endregion get
        #region add
        [Fact]
        public async Task AddDog_Valid()
        {
            // Arrange
            var newDog = new Dog { Name = "John", TailLength = 10, Color = "black&brown", Weight = 15 };
            _mockDogService.Setup(service => service.AddDogAsync(newDog)).ReturnsAsync(newDog);

            // Act
            var result = await _controller.dog(newDog);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Dog added", okResult.Value);
            _mockDogService.Verify(service => service.AddDogAsync(newDog), Times.Once);
        }

        [Fact]
        public async Task AddDog_NullDog()
        {
            // Arrange
            _mockDogService.Setup(service => service.AddDogAsync(null)).ThrowsAsync(new ArgumentException("Invalid JSON is passed in a request body."));

            // Act
            var result = await _controller.dog(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid JSON is passed in a request body.", badRequestResult.Value);
        }

        [Fact]
        public async Task AddDog_DuplicateDog()
        {
            // Arrange
            var newDog = new Dog { Name = "John", TailLength = 10, Color = "black&brown", Weight = 15 };
            _mockDogService.SetupSequence(service => service.AddDogAsync(It.IsAny<Dog>()))
            .ReturnsAsync(newDog)
            .ThrowsAsync(new ArgumentException("Dog with the same name and tail length already exists in DB."));

            // Act
            var result = await _controller.dog(newDog);
            var result2 = await _controller.dog(newDog);

            // Assert

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result2);
            Assert.Equal("Dog with the same name and tail length already exists in DB.", badRequestResult.Value);
        }

        [Fact]
        public async Task AddDog_InvalidTailLength()
        {
            // Arrange
            var invalidDog = new Dog { Name = "victor", TailLength = -1, Color = "white", Weight = 10 };
            _mockDogService.Setup(service => service.AddDogAsync(invalidDog))
                .ThrowsAsync(new ArgumentException("Tail height is a negative number or is not a number."));

            // Act
            var result = await _controller.dog(invalidDog);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Tail height is a negative number or is not a number.", badRequestResult.Value);
        }
        [Fact]
        public async Task AddDog_InvalidWeight()
        {
            // Arrange
            var invalidDog = new Dog { Name = "max", TailLength = 10, Color = "brown", Weight = 0 };
            _mockDogService.Setup(service => service.AddDogAsync(invalidDog))
                .ThrowsAsync(new ArgumentException("Weight must be a bigger then zero"));

            // Act
            var result = await _controller.dog(invalidDog);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Weight must be a bigger then zero", badRequestResult.Value);
        }

        #endregion add

        [Fact]
        public async Task PingTooManyRequests()
        {
            // Arrange
            int _maxRequestsPerSecond = 10;
            var builder = new WebHostBuilder()
                .ConfigureServices(services => { })
                .Configure(app =>
                {
                    app.UseMiddleware<RateLimitingMiddleware>(_maxRequestsPerSecond);

                    app.Map("/ping", builder => builder.Run(async context =>
                    {
                        await context.Response.WriteAsync("Dogshouseservice.Version1.0.1");
                    }));
                });

            using var server = new TestServer(builder);
            using var client = server.CreateClient();

            // Act
            int successfulRequests = 0;
            int tooManyRequestsCount = 0;

            for (int i = 0; i < 15; i++)
            {
                var response = await client.GetAsync("/ping");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    successfulRequests++;
                }
                else if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    tooManyRequestsCount++;
                }
            }

            // Assert
            Assert.Equal(_maxRequestsPerSecond, successfulRequests);
            Assert.Equal(15 - _maxRequestsPerSecond, tooManyRequestsCount);
        }
    }
}