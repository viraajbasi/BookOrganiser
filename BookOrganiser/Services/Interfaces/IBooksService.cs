using BookOrganiser.Models;

namespace BookOrganiser.Services.Interfaces;

public interface IBooksService
{
    Task<Book?> GetBookByISBNAsync(string isbn, UserAccount user);
    Task<List<Book>?> GetBooksByTitleAsync(string title, UserAccount user);
}