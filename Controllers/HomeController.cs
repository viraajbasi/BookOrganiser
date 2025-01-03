using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookOrganiser.Models;
using BookOrganiser.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BookOrganiser.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(AppDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var strUserBooks = _context.UserBooks.FirstOrDefault(e => e.UserName == User.Identity.Name);

        if (strUserBooks == null)
        {
            return View(new List<Book>());
        }

        var intBookIDs = strUserBooks.BookIds.Select(e => int.Parse(e)).ToList();
        var userBooks = _context.Books.AsEnumerable().Where(b => intBookIDs.Contains(b.Id)).ToList();
        return View(userBooks);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
