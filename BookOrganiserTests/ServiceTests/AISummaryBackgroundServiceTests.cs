using BookOrganiser.Data;
using BookOrganiser.Models;
using BookOrganiser.Services;
using BookOrganiser.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BookOrganiserTests.ServiceTests;

public class AISummaryBackgroundServiceTests
{
    private readonly AppDbContext _context;
    private readonly Mock<IAIService> _aiServiceMock;
    private readonly IServiceProvider _serviceProvider;
    private readonly AISummaryBackgroundService _service;

    public AISummaryBackgroundServiceTests()
    {
        // Create in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        // Mock AI service
        _aiServiceMock = new Mock<IAIService>();

        // Create service provider
        var services = new ServiceCollection();
        services.AddSingleton(_context);
        services.AddSingleton(_aiServiceMock.Object);
        _serviceProvider = services.BuildServiceProvider();

        // Create service
        _service = new AISummaryBackgroundService(_serviceProvider);
    }

    private void SetupTestData(List<AISummary> summaries)
    {
        // Clear existing data
        _context.AISummary.RemoveRange(_context.AISummary);
        _context.SaveChanges();

        // Add test data
        _context.AISummary.AddRange(summaries);
        _context.SaveChanges();
    }

    [Fact]
    public async Task ExecuteAsync_ProcessesPendingSummaries()
    {
        // Arrange
        var book = new Book 
        { 
            Id = 1, 
            Title = "Test Book", 
            Authors = new List<string> { "Test Author 1", "Test Author 2" }
        };
            
        var pendingSummaries = new List<AISummary>
        {
            new AISummary 
            { 
                Id = 1, 
                BookId = book.Id, 
                Book = book,
                IsGenerated = false,
                Summary = string.Empty,
                KeyQuotes = string.Empty,
                KeyThemes = string.Empty
            }
        };

        SetupTestData(pendingSummaries);
            
        _aiServiceMock.Setup(s => s.GenerateBookSummaryAsync(It.IsAny<Book>()))
            .ReturnsAsync("Generated summary");
            
        _aiServiceMock.Setup(s => s.GenerateKeyQuotesAsync(It.IsAny<Book>()))
            .ReturnsAsync("Generated key quotes");
            
        _aiServiceMock.Setup(s => s.GenerateKeyThemesAsync(It.IsAny<Book>()))
            .ReturnsAsync("Generated key themes");

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(2)); // Cancel after 2 seconds

        // Act
        await _service.StartAsync(cancellationTokenSource.Token);
        await Task.Delay(TimeSpan.FromSeconds(1.5)); // Give time for the service to process
        await _service.StopAsync(CancellationToken.None);

        // Assert
        _aiServiceMock.Verify(s => s.GenerateBookSummaryAsync(It.IsAny<Book>()), Times.Once);
        _aiServiceMock.Verify(s => s.GenerateKeyQuotesAsync(It.IsAny<Book>()), Times.Once);
        _aiServiceMock.Verify(s => s.GenerateKeyThemesAsync(It.IsAny<Book>()), Times.Once);
            
        // Verify data was updated
        var updatedSummary = await _context.AISummary.FirstOrDefaultAsync();
        Assert.NotNull(updatedSummary);
        Assert.Equal("Generated summary", updatedSummary.Summary);
        Assert.Equal("Generated key quotes", updatedSummary.KeyQuotes);
        Assert.Equal("Generated key themes", updatedSummary.KeyThemes);
        Assert.True(updatedSummary.IsGenerated);
        Assert.NotNull(updatedSummary.GeneratedAt);
    }

    [Fact]
    public async Task ExecuteAsync_SkipsAlreadyGeneratedSummaries()
    {
        // Arrange
        var book = new Book 
        { 
            Id = 1, 
            Title = "Test Book", 
            Authors = new List<string> { "Test Author 1", "Test Author 2" }
        };
            
        var completedSummaries = new List<AISummary>
        {
            new AISummary 
            { 
                Id = 1, 
                BookId = book.Id, 
                Book = book,
                IsGenerated = true,
                Summary = "Existing summary",
                KeyQuotes = "Existing quotes",
                KeyThemes = "Existing themes",
                GeneratedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        SetupTestData(completedSummaries);

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(2)); // Cancel after 2 seconds

        // Act
        await _service.StartAsync(cancellationTokenSource.Token);
        await Task.Delay(TimeSpan.FromSeconds(1.5)); // Give time for the service to process
        await _service.StopAsync(CancellationToken.None);

        // Assert
        _aiServiceMock.Verify(s => s.GenerateBookSummaryAsync(It.IsAny<Book>()), Times.Never);
        _aiServiceMock.Verify(s => s.GenerateKeyQuotesAsync(It.IsAny<Book>()), Times.Never);
        _aiServiceMock.Verify(s => s.GenerateKeyThemesAsync(It.IsAny<Book>()), Times.Never);
            
        // Verify data wasn't changed
        var summary = await _context.AISummary.FirstOrDefaultAsync();
        Assert.Equal("Existing summary", summary.Summary);
        Assert.Equal("Existing quotes", summary.KeyQuotes);
        Assert.Equal("Existing themes", summary.KeyThemes);
    }

    [Fact]
    public async Task ExecuteAsync_OnlyGeneratesMissingFields()
    {
        // Arrange
        var book = new Book 
        { 
            Id = 1, 
            Title = "Test Book", 
            Authors = new List<string> { "Test Author 1", "Test Author 2" }
        };
            
        var partialSummaries = new List<AISummary>
        {
            new AISummary 
            { 
                Id = 1, 
                BookId = book.Id, 
                Book = book,
                IsGenerated = false,
                Summary = "Existing summary", // Only summary is filled
                KeyQuotes = string.Empty,
                KeyThemes = string.Empty
            }
        };

        SetupTestData(partialSummaries);
            
        _aiServiceMock.Setup(s => s.GenerateKeyQuotesAsync(It.IsAny<Book>()))
            .ReturnsAsync("Generated key quotes");
            
        _aiServiceMock.Setup(s => s.GenerateKeyThemesAsync(It.IsAny<Book>()))
            .ReturnsAsync("Generated key themes");

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(2)); // Cancel after 2 seconds

        // Act
        await _service.StartAsync(cancellationTokenSource.Token);
        await Task.Delay(TimeSpan.FromSeconds(1.5)); // Give time for the service to process
        await _service.StopAsync(CancellationToken.None);

        // Assert
        _aiServiceMock.Verify(s => s.GenerateBookSummaryAsync(It.IsAny<Book>()), Times.Never);
        _aiServiceMock.Verify(s => s.GenerateKeyQuotesAsync(It.IsAny<Book>()), Times.Once);
        _aiServiceMock.Verify(s => s.GenerateKeyThemesAsync(It.IsAny<Book>()), Times.Once);
            
        // Verify only certain fields were updated
        var updatedSummary = await _context.AISummary.FirstOrDefaultAsync();
        Assert.Equal("Existing summary", updatedSummary.Summary);
        Assert.Equal("Generated key quotes", updatedSummary.KeyQuotes);
        Assert.Equal("Generated key themes", updatedSummary.KeyThemes);
        Assert.True(updatedSummary.IsGenerated);
    }

    [Fact]
    public async Task ExecuteAsync_HandlesExceptions()
    {
        // Arrange
        var book = new Book 
        { 
            Id = 1, 
            Title = "Test Book", 
            Authors = new List<string> { "Test Author 1", "Test Author 2" }
        };
            
        var pendingSummaries = new List<AISummary>
        {
            new AISummary 
            { 
                Id = 1, 
                BookId = book.Id, 
                Book = book,
                IsGenerated = false,
                Summary = string.Empty,
                KeyQuotes = string.Empty,
                KeyThemes = string.Empty
            }
        };

        SetupTestData(pendingSummaries);
            
        _aiServiceMock.Setup(s => s.GenerateBookSummaryAsync(It.IsAny<Book>()))
            .ThrowsAsync(new Exception("API failure"));

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(2)); // Cancel after 2 seconds

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(async () => 
        {
            await _service.StartAsync(cancellationTokenSource.Token);
            await Task.Delay(TimeSpan.FromSeconds(1.5)); // Give time for the service to process
        });
    }
}