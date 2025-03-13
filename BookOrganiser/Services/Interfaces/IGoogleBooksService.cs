using BookOrganiser.Models;
using Google.Apis.Books.v1.Data;

namespace BookOrganiser.Services.Interfaces;

public interface IGoogleBooksService
{
    Task<Volume?> GetBookByISBNAsync(string isbn);
    Task<List<Volume>?> GetBookByTitleAsync(string title);
    Book ConvertToBookModel(Volume book, UserAccount user);
}