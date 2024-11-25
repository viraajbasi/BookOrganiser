using Microsoft.AspNetCore.Mvc;

namespace BookOrganiser.Controllers;

public class BookController : Controller
{
    public IActionResult Details()
    {
        return View();
    }
}