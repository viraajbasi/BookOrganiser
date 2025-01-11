using BookOrganiser.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using BookOrganiser.Models;
using Microsoft.AspNetCore.Authorization;

namespace BookOrganiser.Controllers;

[Authorize]
public class BookController : Controller
{
    private readonly AppDbContext _context;

    public BookController(AppDbContext context)
    {
        _context = context;
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

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,GoogleBooksId,Title,Subtitle,Authors,Publisher,PublishedDate,Description,ISBN10,ISBN13,PageCount,GoogleBooksCategories,Thumbnail,SmallThumbnail,SmallImage,MediumImage,LargeImage,ExtraLargeImage,GoogleBooksLink,CustomCategories")]Book book)
    {
        if (ModelState.IsValid)
        {
            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
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
    public async Task<IActionResult> Edit(int id, [Bind("Id,GoogleBooksId,Title,Subtitle,Authors,Publisher,PublishedDate,Description,ISBN10,ISBN13,PageCount,GoogleBooksCategories,Thumbnail,SmallThumbnail,SmallImage,MediumImage,LargeImage,ExtraLargeImage,GoogleBooksLink,CustomCategories")]Book book)
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

        List<Book> bookResponse = new();

        foreach (var result in results)
        {
            bookResponse.Add(GoogleBooksToModel(result));
        }

        return View(bookResponse);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBookISBN(string isbn)
    {
        List<Volume> results = await SearchByISBN(isbn);

        var bookResponse = GoogleBooksToModel(results[0]);
        
        if (results.Count > 0 && ModelState.IsValid && IsUniqueBook(bookResponse))
        {
            _context.Add(bookResponse);
            await _context.SaveChangesAsync();

            return RedirectToAction("AddBookToUser", "UserBooks", new { bookID = bookResponse.Id});
        }
        
        var bookInDb = await _context.Books.FirstAsync(e => e.GoogleBooksID == bookResponse.GoogleBooksID);
        return RedirectToAction("AddBookToUser", "UserBooks", new { bookID = bookInDb.Id });
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

    private Book GoogleBooksToModel(Volume gbook)
    {
        string isbn10 = string.Empty;
        string isbn13 = string.Empty;

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

    private bool IsUniqueBook(Book book)
    {
        return !_context.Books.Any(e => e.GoogleBooksID == book.GoogleBooksID);
    }
}