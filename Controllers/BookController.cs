using BookOrganiser.Data;
using Microsoft.AspNetCore.Mvc;

namespace BookOrganiser.Controllers;

public class BookController : Controller
{
    private readonly AppDbContext _context;
    
    public BookController(AppDbContext context)
    {
        _context = context;
    }
    public IActionResult Details()
    {
        return View();
    }
}