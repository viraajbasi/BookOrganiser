using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookOrganiser.Models;
using BookOrganiser.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BookOrganiser.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public HomeController(AppDbContext context, ILogger<HomeController> logger, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var userBooks = _context.Books.Where(e => e.UserId == user.Id).ToList();
        
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
