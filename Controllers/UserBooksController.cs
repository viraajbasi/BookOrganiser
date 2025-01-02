using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookOrganiser.Data;
using Microsoft.AspNetCore.Identity;
using BookOrganiser.Models;

namespace BookOrganiser.Controllers;

[Authorize]
public class UserBooksController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public UserBooksController(AppDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> AddBookToUser(string bookID)
    {
        string userName = User.Identity.Name;
        List<string> bookList = [bookID];
        
        UserBooks userBooks = new()
        {
            UserName = userName,
            BookIds = bookList,
        };

        if (ModelState.IsValid)
        {
            _context.Add(userBooks);
            await _context.SaveChangesAsync();
        }
        
        return RedirectToAction("Index", "Home");
    }
}