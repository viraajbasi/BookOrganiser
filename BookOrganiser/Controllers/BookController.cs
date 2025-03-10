using BookOrganiser.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using BookOrganiser.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BookOrganiser.Controllers;

[Authorize]
public class BookController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<UserAccount> _userManager;

    public BookController(AppDbContext context, UserManager<UserAccount> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Details(int? id)
    {
        var user = await _userManager.GetUserAsync(User);
        
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        if (id == null)
        {
            return RedirectToAction("Error", "Home", new { message = "Error finding object", statusCode = 404 });
        }

        var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return RedirectToAction("Error", "Home", new { message = "Error finding object", statusCode = 404 });
        }

        ViewBag.Categories = user.UserCategories;
        ViewBag.AIEnabled = user.AcceptedAIFeatures;

        return View(book);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Error", "Home", new { message = "Error finding object", statusCode = 404 });
        }

        var book = await _context.Books
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return RedirectToAction("Error", "Home", new { message = "Error finding object", statusCode = 404 });
        }

        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Find()
    {
        return View();
    }

    public async Task<IActionResult> SearchResults(string title)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var results = await GetSearchResults(title);

        if (results == null)
        {
            return View(new List<Book>());
        }

        var bookResponse = new List<Book>();

        foreach (var result in results)
        {
            bookResponse.Add(GoogleBooksToModel(result, user));
        }

        return View(bookResponse);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBook(string identifier, string isISBN)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var queryType = Convert.ToBoolean(isISBN);
        var result = await PerformSearchQuery(identifier, queryType);
        
        if (result != null && ModelState.IsValid)
        {
            var bookResponse = GoogleBooksToModel(result, user);
            _context.Add(bookResponse);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        return RedirectToAction("Error", "Home", new { message = "Invalid search query" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBookToCategory(int id, string category)
    {
        if (ModelState.IsValid)
        {
            var entity = await _context.Books.FirstAsync(e => e.Id == id); 
            entity.CustomCategories.Add(category);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index", "Home");
        }

        return RedirectToAction("Error", "Home", new { message = $"Error adding book to category: {category}" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveBookFromCategory(int id, string category)
    {
        if (ModelState.IsValid)
        {
            var entity = await _context.Books.FirstAsync(e => e.Id == id);
            entity.CustomCategories.Remove(category);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index", "Home");
        }

        return RedirectToAction("Error", "Home", new { message = $"Error removing book from category: {category}" });
    }
    
    private static async Task<Volume?> PerformSearchQuery(string query, bool isISBN)
    {
        var service = new BooksService();
        
        if (isISBN)
        {
            var request = service.Volumes.List($"isbn:{query}");
            request.OrderBy = VolumesResource.ListRequest.OrderByEnum.Relevance;
            request.Fields = "items(id,volumeInfo(title,subtitle,authors,publisher,publishedDate,description,industryIdentifiers,pageCount,categories,imageLinks,previewLink))";
            request.MaxResults = 1;

            var response = await request.ExecuteAsync();

            if (response.Items != null)
            {
                return response.Items.ToList()[0];
            }
        }
        else
        {
            var request = service.Volumes.Get($"{query}");
            request.Fields = "id,volumeInfo(title,subtitle,authors,publisher,publishedDate,description,industryIdentifiers,pageCount,categories,imageLinks,previewLink)";
            
            var response = await request.ExecuteAsync();

            if (response != null)
            {
                return response;
            }
        }

        return null;
    }
    
    private static async Task<List<Volume>?> GetSearchResults(string title)
    {
        var service = new BooksService();

        var request = service.Volumes.List($"{title}");
        request.OrderBy = VolumesResource.ListRequest.OrderByEnum.Relevance;
        request.Fields = "items(id,volumeInfo(title,authors,publisher,publishedDate,description,industryIdentifiers,imageLinks(thumbnail)))";
        request.MaxResults = 40;

        var response = await request.ExecuteAsync();

        if (response != null && response.Items.Count > 0)
        {
            return response.Items.ToList();
        }

        return null;
    }
    
    private Book GoogleBooksToModel(Volume book, UserAccount user)
    {
        var identifiers = book.VolumeInfo.IndustryIdentifiers?.ToDictionary(i => i.Type, i=> i.Identifier)
                          ?? new Dictionary<string, string>();

        return new Book()
        {
            UserId = user.Id,
            UserAccount = user,
            GoogleBooksID = book.Id ?? string.Empty,
            Title = book.VolumeInfo.Title ?? string.Empty,
            Subtitle = book.VolumeInfo.Subtitle ?? string.Empty,
            Authors = book.VolumeInfo.Authors ?? new List<string>(),
            Publisher = book.VolumeInfo.Publisher ?? string.Empty,
            PublishedDate = book.VolumeInfo.PublishedDate ?? string.Empty,
            Description = book.VolumeInfo.Description ?? string.Empty,
            ISBN10 = identifiers.TryGetValue("ISBN_10", out var isbn10) ? isbn10 : string.Empty,
            ISBN13 = identifiers.TryGetValue("ISBN_13", out var isbn13) ? isbn13 : string.Empty,
            ISSN = identifiers.TryGetValue("ISSN", out var issn) ? issn : string.Empty,
            OtherIdentifier = identifiers.TryGetValue("OTHER", out var other) ? other : string.Empty,
            PageCount = book.VolumeInfo.PageCount ?? 0,
            GoogleBooksCategories = book.VolumeInfo.Categories ?? new List<string>(),
            Thumbnail = book.VolumeInfo.ImageLinks?.Thumbnail ?? string.Empty,
            SmallThumbnail = book.VolumeInfo.ImageLinks?.SmallThumbnail ?? string.Empty,
            SmallImage = book.VolumeInfo.ImageLinks?.Small ?? string.Empty,
            MediumImage = book.VolumeInfo.ImageLinks?.Medium ?? string.Empty,
            LargeImage = book.VolumeInfo.ImageLinks?.Large ?? string.Empty,
            ExtraLargeImage = book.VolumeInfo.ImageLinks?.ExtraLarge ?? string.Empty,
            GoogleBooksLink = book.VolumeInfo.InfoLink ?? string.Empty
        };
    }
}
