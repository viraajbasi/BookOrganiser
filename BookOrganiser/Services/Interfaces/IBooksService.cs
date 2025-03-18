using BookOrganiser.Models;

namespace BookOrganiser.Services.Interfaces;

public interface IBooksService
{
    Task<Book?> GetBookByUpstreamIdAsync(string id, UserAccount user);
    Task<List<Book>?> GetBooksByISBNAsync(string isbn, UserAccount user);
    Task<List<Book>?> GetBooksByTitleAsync(string title, UserAccount user);
    Task<List<Book>?> GetBooksByAuthorAsync(string author, UserAccount user);
}