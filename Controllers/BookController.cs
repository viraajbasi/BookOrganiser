using BookOrganiser.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using BookOrganiser.Models;

namespace BookOrganiser.Controllers;

public class BookController : Controller
{
    private readonly AppDbContext _context;

    public BookController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Books.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> AddBook(string isbn)
    {
        List<Volume> results = await SearchByISBN(isbn);

        var bookResponse = GoogleBooksToModel(results[0]);
        
        if (results.Count > 0 && ModelState.IsValid)
        {
            _context.Add(bookResponse);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
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

    private static Book GoogleBooksToModel(Volume gbook)
    {
        return new Book()
        {
            GoogleBooksID = gbook.Id,
            Title = gbook.VolumeInfo.Title,
            Subtitle = gbook.VolumeInfo.Subtitle,
            Authors = gbook.VolumeInfo.Authors,
            Publisher = gbook.VolumeInfo.Publisher,
            PublishedDate = gbook.VolumeInfo.PublishedDate,
            Description = gbook.VolumeInfo.Description,
            ISBN10 = "PLACEHOLDER",
            ISBN13 = "PLACEHOLDER",
            GoogleBooksCategories = gbook.VolumeInfo.Categories,
            Thumbnail = gbook.VolumeInfo.ImageLinks.Thumbnail,
            SmallThumbnail = gbook.VolumeInfo.ImageLinks.SmallThumbnail,
            SmallImage = gbook.VolumeInfo.ImageLinks.Small,
            MediumImage = gbook.VolumeInfo.ImageLinks.Medium,
            LargeImage = gbook.VolumeInfo.ImageLinks.Large,
            ExtraLargeImage = gbook.VolumeInfo.ImageLinks.ExtraLarge,
            GoogleBooksLink = gbook.VolumeInfo.InfoLink
        };
    }
}