using System.Text.Json;
using BookOrganiser.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookOrganiser.Models;
using BookOrganiser.Services.Interfaces;
using BookOrganiser.ViewModels.BookViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BookOrganiser.Controllers;

[Authorize]
public class BookController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<UserAccount> _userManager;
    private readonly IBooksService _booksService;

    public BookController(AppDbContext context, UserManager<UserAccount> userManager, IBooksService booksService)
    {
        _context = context;
        _userManager = userManager;
        _booksService = booksService;
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Error", "Home", new { message = "An unknown error has occurred", statusCode = 404 });
        }
        
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var book = await _context.Books.Include(book => book.AISummary).FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return RedirectToAction("Error", "Home", new { message = "An unknown error has occurred", statusCode = 404 });
        }
        
        var viewModel = new DetailsViewModel
        {
            Categories = user.UserCategories,
            Book = book,
        };

        return View(viewModel);
    }
    
    public IActionResult FindBooksTitle()
    {
        return View();
    }

    public IActionResult FindBooksAuthor()
    {
        return View();
    }

    public IActionResult FindBooksISBN()
    {
        return View();
    }

    public async Task<IActionResult> SearchResults()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var resultsJson = HttpContext.Session.GetString("SearchResults");
        if (resultsJson != null)
        {
            var books = JsonSerializer.Deserialize<List<Book>>(resultsJson);
            HttpContext.Session.Remove("SearchResults");
            
            return View(books);
        }
        
        return View(new List<Book>());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FindBooksTitle(string title)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var results = await _booksService.GetBooksByTitleAsync(title, user);

        if (results != null)
        {
            HttpContext.Session.SetString("SearchResults", JsonSerializer.Serialize(results));
        }

        return RedirectToAction("SearchResults", "Book");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FindBooksAuthor(string author)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var results = await _booksService.GetBooksByAuthorAsync(author, user);

        if (results != null)
        {
            HttpContext.Session.SetString("SearchResults", JsonSerializer.Serialize(results));
        }

        return RedirectToAction("SearchResults", "Book");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FindBooksISBN(string isbn)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var results = await _booksService.GetBooksByISBNAsync(isbn, user);

        if (results != null)
        {
            HttpContext.Session.SetString("SearchResults", JsonSerializer.Serialize(results));
        }

        return RedirectToAction("SearchResults", "Book");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBookFromSearchResults(string id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var result = await _booksService.GetBookByUpstreamId(id, user);
        
        if (result != null && ModelState.IsValid)
        {
            var aiSummary = new AISummary
            {
                BookId = result.Id,
                Book = result,
            };
            
            result.AISummary = aiSummary;
            
            _context.Add(result);
            _context.AISummary.Add(aiSummary);
            
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
        
        TempData["Error"] = $"No search results found for '{id}'";
        return RedirectToAction("FindBooksTitle", "Book");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBook(int id)
    {
        if (ModelState.IsValid)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            var aiSummary = await _context.AISummary.FirstOrDefaultAsync(e => e.BookId == id);
            if (aiSummary != null)
            {
                _context.AISummary.Remove(aiSummary);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        return RedirectToAction("Error", "Home", new { message = "An unknown error has occurred", statusCode = 404 });
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

        return RedirectToAction("Error", "Home", new { message = $"Unknown error occured while adding book to category: '{category}'" });
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

        return RedirectToAction("Error", "Home", new { message = $"Unknown error occured while removing book from category: '{category}'" });
    }
}
