using BookOrganiser.Data;
using BookOrganiser.Models;
using BookOrganiser.Services.Interfaces;
using BookOrganiser.ViewModels.AISummaryViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookOrganiser.Controllers;

[Authorize]
public class AISummaryController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<UserAccount> _userManager;
    private readonly IAIService _aiService;

    public AISummaryController(AppDbContext context, UserManager<UserAccount> userManager, IAIService aiService)
    {
        _context = context;
        _userManager = userManager;
        _aiService = aiService;
    }
    
    public async Task<IActionResult> Summary(int? id)
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
        
        var summary = await _context.AISummary.Include(aiSummary => aiSummary.Book).FirstOrDefaultAsync(m => m.Id == id);
        if (summary == null)
        {
            return RedirectToAction("Error", "Home", new { message = "An unknown error has occurred", statusCode = 404 });
        }

        var viewModel = new SummaryViewModel
        {
            Summary = summary,
            AcceptedAIFeatures = user.AcceptedAIFeatures
        };
        
        return View(viewModel);
    }
}