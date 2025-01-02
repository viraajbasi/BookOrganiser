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
        bool doesUserHaveEntry = _context.UserBooks.Any(e => e.UserName == userName);

        if (!doesUserHaveEntry)
        {
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
        }
        else
        {
            UserBooks entry = _context.UserBooks.First(e => e.UserName == userName)!;

            if (entry.BookIds.Contains(bookID))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                entry.BookIds.Add(bookID);
            }

            if (ModelState.IsValid)
            {
                _context.Update(entry);
                await _context.SaveChangesAsync();
            }
        }
        
        return RedirectToAction("Index", "Home");
    }
}