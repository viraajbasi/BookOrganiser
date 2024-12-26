using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BookOrganiser.ViewModels;
using BookOrganiser.Models;

namespace BookOrganiser.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<User> signInManager;
    private readonly UserManager<User> userManager;

    public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            User user = new User
            {
                FullName = model.Name,
                Email = model.Email,
                UserName = model.Email,
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }
        }

        return View(model);
    }

    public IActionResult VerifyEmail()
    {
        return View();
    }

    public IActionResult ChangePassword()
    {
        return View();
    }
}