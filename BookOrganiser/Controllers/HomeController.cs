using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookOrganiser.Models;
using BookOrganiser.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BookOrganiser.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<UserAccount> _userManager;

    public HomeController(AppDbContext context, UserManager<UserAccount> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var userBooks = _context.Books.Where(e => e.UserId == user.Id).ToList();
        
        ViewBag.HasAnyBooks = userBooks.Count != 0;
        ViewBag.Categories = user.UserCategories;
        
        return View(userBooks);
    }

    [Authorize]
    public async Task<IActionResult> EditCategories()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        ViewBag.Categories = user.UserCategories.Remove("Favourites");
        ViewBag.HasAnyCategories = user.UserCategories.Count > 0;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string? message, bool? showInfoText, int statusCode = 500)
    {
        return View(
            new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorMessage = message ?? "An Unknown Error Has Occurred",
                StatusCode = statusCode,
                ShowInfoText = showInfoText ?? true
            });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory(string category)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        category = category.ToLower();
        if (user.UserCategories.Contains(category))
        {
            TempData["Error"] = "Category already exists.";
            return RedirectToAction("Index", "Home");
        }
        
        if (ModelState.IsValid)
        {
            user.UserCategories.Add(category);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index", "Home");
        }
        
        return RedirectToAction("Error", "Home", new { message = $"Unknown error occurred whilst creating category '{category}'" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(string category)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (category.ToLower() == "favourites")
        {
            TempData["Error"] = "Cannot delete favourites.";
            return RedirectToAction("Index", "Home");
        }
        
        var booksToUpdate = _context.Books.Where(e => e.UserAccount == user && e.CustomCategories.Contains(category));
        
        if (ModelState.IsValid)
        {
            foreach (var book in booksToUpdate)
            {
                book.CustomCategories.Remove(category);
            }
            user.UserCategories.Remove(category);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index", "Home");
        }
        
        return RedirectToAction("Error", "Home", new { message = $"Unknown error occured whilst deleting category '{category}'" });
    }
}
