using BookOrganiser.Data;
using BookOrganiser.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookOrganiser.Services;

public class AISummaryBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public AISummaryBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingAISummaries(stoppingToken);
            }
            catch
            {
                throw new ApplicationException();
            }
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task ProcessPendingAISummaries(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var aiService = scope.ServiceProvider.GetRequiredService<IAIService>();
        var pendingSummaries = await context.AISummary
            .Include(s => s.Book)
            .Where(s => !s.IsGenerated &&
                        (s.Summary == string.Empty ||
                         s.KeyQuotes == string.Empty ||
                         s.KeyThemes == string.Empty))
            .ToListAsync(stoppingToken);

        foreach (var summary in pendingSummaries)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                if (string.IsNullOrEmpty(summary.Summary))
                {
                    summary.Summary = await aiService.GenerateBookSummaryAsync(summary.Book);
                }

                if (string.IsNullOrEmpty(summary.KeyQuotes))
                {
                    summary.KeyQuotes = await aiService.GenerateKeyQuotesAsync(summary.Book);
                }

                if (string.IsNullOrEmpty(summary.KeyThemes))
                {
                    summary.KeyThemes = await aiService.GenerateKeyThemesAsync(summary.Book);
                }
                
                summary.IsGenerated = true;
                summary.GeneratedAt = DateTime.UtcNow;
                
                await context.SaveChangesAsync(stoppingToken);
            }
            catch
            {
                throw new ApplicationException();
            }
        }
    }
}