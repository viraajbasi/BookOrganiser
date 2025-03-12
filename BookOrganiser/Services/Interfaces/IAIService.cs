using BookOrganiser.Models;

namespace BookOrganiser.Services.Interfaces;

public interface IAIService
{
    Task<string> GenerateBookSummaryAsync(Book book);
    Task<string> GenerateKeyQuotesAsync(Book book);
    Task<string> GenerateKeyThemesAsync(Book book);
}