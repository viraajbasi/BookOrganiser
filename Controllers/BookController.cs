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
    private readonly UserManager<User> _userManager;

    public BookController(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Book book)
    {
        if (id != book.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Index", "Home");
        }

        return View(book);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
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

    public async Task<IActionResult> AddBookTitle(string title)
    {
        var results = await SearchByTitle(title);

        if (results.Count == 0)
        {
            return View(new List<Book>());
        }

        var bookResponse = new List<Book>();

        foreach (var result in results)
        {
            bookResponse.Add(await GoogleBooksToModel(result));
        }

        return View(bookResponse);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBookISBN(string isbn)
    {
        var results = await SearchByISBN(isbn);
        var bookResponse = await GoogleBooksToModel(results[0]);
        
        if (results.Count > 0 && ModelState.IsValid)
        {
            _context.Add(bookResponse);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        return NotFound();
    }
    
    private static async Task<List<Volume>> SearchByTitle(string title)
    {
        var service = new BooksService();

        var request = service.Volumes.List($"{title}");
        request.OrderBy = VolumesResource.ListRequest.OrderByEnum.Relevance;
        request.Fields = "totalItems,items(id,volumeInfo(title,subtitle,authors,publisher,publishedDate,description,industryIdentifiers,pageCount,categories,imageLinks,previewLink))";

        var response = await request.ExecuteAsync();

        if (response.Items != null)
        {
            return response.Items.ToList();
        }

        return [];
    }

    private static async Task<List<Volume>> SearchByISBN(string isbn)
    {
        var service = new BooksService();
        var request = service.Volumes.List($"isbn:{isbn}");
        request.OrderBy = VolumesResource.ListRequest.OrderByEnum.Relevance;
        request.Fields = "totalItems,items(id,volumeInfo(title,subtitle,authors,publisher,publishedDate,description,industryIdentifiers,pageCount,categories,imageLinks,previewLink))";

        var response = await request.ExecuteAsync();

        if (response.Items != null)
        {
            return response.Items.ToList();
        }

        return [];
    }

    private async Task<Book> GoogleBooksToModel(Volume gbook)
    {
        var isbn10 = string.Empty;
        var isbn13 = string.Empty;
        var user = await _userManager.GetUserAsync(User);

        foreach (var industryIdentifier in gbook.VolumeInfo.IndustryIdentifiers)
        {
            if (industryIdentifier.Type == "ISBN_10")
            {
                isbn10 = industryIdentifier.Identifier;
            }
            else if (industryIdentifier.Type == "ISBN_13")
            {
                isbn13 = industryIdentifier.Identifier;
            }
        }

        return new Book()
        {
            UserId = user.Id,
            User = user,
            GoogleBooksID = gbook.Id ?? string.Empty,
            Title = gbook.VolumeInfo.Title ?? string.Empty,
            Subtitle = gbook.VolumeInfo.Subtitle ?? string.Empty,
            Authors = gbook.VolumeInfo.Authors,
            Publisher = gbook.VolumeInfo.Publisher ?? string.Empty,
            PublishedDate = gbook.VolumeInfo.PublishedDate ?? string.Empty,
            Description = gbook.VolumeInfo.Description ?? string.Empty,
            ISBN10 = isbn10,
            ISBN13 = isbn13,
            GoogleBooksCategories = gbook.VolumeInfo.Categories,
            Thumbnail = gbook.VolumeInfo.ImageLinks.Thumbnail ?? string.Empty,
            SmallThumbnail = gbook.VolumeInfo.ImageLinks.SmallThumbnail ?? string.Empty,
            SmallImage = gbook.VolumeInfo.ImageLinks.Small ?? string.Empty,
            MediumImage = gbook.VolumeInfo.ImageLinks.Medium ?? string.Empty,
            LargeImage = gbook.VolumeInfo.ImageLinks.Large ?? string.Empty,
            ExtraLargeImage = gbook.VolumeInfo.ImageLinks.ExtraLarge ?? string.Empty,
            GoogleBooksLink = gbook.VolumeInfo.InfoLink ?? string.Empty
        };
    }

    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.Id == id);
    }
}