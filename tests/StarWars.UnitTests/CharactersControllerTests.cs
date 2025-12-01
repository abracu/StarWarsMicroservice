using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using StarWars.Api.Controllers;
using StarWars.Application.DTOs;
using StarWars.Application.Interfaces;
using StarWars.Domain.Entities;
using StarWars.Infrastructure.Persistence;
using Xunit;

namespace StarWars.UnitTests;

public class CharactersControllerTests
{
    private readonly Mock<ISwapiService> _mockSwapiService;
    private readonly StarWarsDbContext _dbContext;
    private readonly CharactersController _controller;

    public CharactersControllerTests()
    {
        // 1. Mock the external service dependency
        _mockSwapiService = new Mock<ISwapiService>();

        // 2. Setup In-Memory Database (Simulates PostgreSQL)
        var options = new DbContextOptionsBuilder<StarWarsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name per test ensures isolation
            .Options;
        
        _dbContext = new StarWarsDbContext(options);

        // 3. Instantiate the Controller (System Under Test) with dependencies
        _controller = new CharactersController(_mockSwapiService.Object, _dbContext);
    }

    [Fact]
    public async Task Get_ShouldReturnOk_WithCharacters()
    {
        // Arrange
        var fakeData = new List<CharacterDto> { new CharacterDto { Name = "Yoda" } };
        _mockSwapiService.Setup(s => s.GetPeopleAsync(1))
                         .ReturnsAsync(fakeData);

        // Act
        var result = await _controller.Get(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result); // Verify response is 200 OK
        var returnCharacters = Assert.IsAssignableFrom<IEnumerable<CharacterDto>>(okResult.Value);
        Assert.Single(returnCharacters);
    }

    [Fact]
    public async Task AddFavorite_ShouldSaveToDb_AndReturnCreated()
    {
        // Arrange
        var newFavorite = new CreateFavoriteDto { Name = "Darth Vader", Url = "url/vader" };

        // Act
        var result = await _controller.AddFavorite(newFavorite);

        // Assert
        // 1. Verify HTTP Response status
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, createdResult.StatusCode);

        // 2. Verify data persistence in the DB
        var savedFav = await _dbContext.FavoriteCharacters.FirstOrDefaultAsync(f => f.Url == "url/vader");
        Assert.NotNull(savedFav);
        Assert.Equal("Darth Vader", savedFav.Name);
    }
}