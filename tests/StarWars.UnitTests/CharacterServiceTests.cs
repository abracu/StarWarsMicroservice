using Moq;
using System.Text;
using System.Text.Json;
using StarWars.Application.DTOs;
using StarWars.Application.Interfaces;
using StarWars.Infrastructure.ExternalApi;
using Microsoft.Extensions.Caching.Distributed; // Using Distributed Cache interface (Redis)
using Xunit;

namespace StarWars.UnitTests;

public class CharacterServiceTests
{
    [Fact]
    public async Task CachedService_Should_Call_InnerService_When_Cache_Is_Miss()
    {
        // Arrange
        var mockService = new Mock<ISwapiService>();
        var mockCache = new Mock<IDistributedCache>();

        // 1. Setup the Service Mock to return simulated data
        var expectedData = new List<CharacterDto> 
        { 
            new CharacterDto { Name = "Luke Skywalker", Url = "https://swapi.dev/api/people/1/" } 
        };
        
        mockService.Setup(s => s.GetPeopleAsync(1))
                   .ReturnsAsync(expectedData);

        // 2. Setup the Redis Mock to simulate an empty cache (Cache Miss)
        // GetAsync returns null when the key is not found
        mockCache.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((byte[]?)null);

        // Instantiate the System Under Test (SUT) injecting the mocks
        var cachedService = new CachedSwapiService(mockService.Object, mockCache.Object);

        // Act
        var result = await cachedService.GetPeopleAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Luke Skywalker", result.First().Name);
        
        // Verify that the real service was called since the cache was empty
        mockService.Verify(s => s.GetPeopleAsync(1), Times.Once);
        
        // Verify that the new data was saved to the cache
        mockCache.Verify(c => c.SetAsync(
            It.IsAny<string>(), 
            It.IsAny<byte[]>(), 
            It.IsAny<DistributedCacheEntryOptions>(), 
            It.IsAny<CancellationToken>()), Times.Once);
    }
}