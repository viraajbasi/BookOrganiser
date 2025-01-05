using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookOrganiser.Data;
using Microsoft.AspNetCore.Identity;
using BookOrganiser.Models;
using Microsoft.EntityFrameworkCore;

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
        bool doesUserHaveEntry = _context.UserBooks.Any(e => e.UserName == User.Identity.Name);

        if (!doesUserHaveEntry)
        {
            List<string> bookList = [bookID];
            UserBooks userBooks = new()
            {
                UserName = User.Identity.Name,
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
            UserBooks entry = await _context.UserBooks.FirstAsync(e => e.UserName == User.Identity.Name);

            if (entry.BookIds.Contains(bookID))
            {
                return RedirectToAction("Index", "Home");
            }

            entry.BookIds.Add(bookID);

            if (ModelState.IsValid)
            {
                _context.Update(entry);
                await _context.SaveChangesAsync();
            }
        }
        
        return RedirectToAction("Index", "Home");
    }
}