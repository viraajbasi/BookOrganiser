using BookOrganiser.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BookOrganiser.ViewModels;
using BookOrganiser.Models;
using BookOrganiser.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;

namespace BookOrganiser.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<UserAccount> _signInManager;
    private readonly UserManager<UserAccount> _userManager;
    private readonly AppDbContext _context;

    public AccountController(SignInManager<UserAccount> signInManager, UserManager<UserAccount> userManager, AppDbContext context)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            
            ModelState.AddModelError(string.Empty, "Email or password is incorrect.");
            return View(model);
        }

        return View(model);
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userAccount = new UserAccount
            {
                FullName = model.Name,
                Email = model.Email,
                UserName = model.Email,
            };

            var result = await _userManager.CreateAsync(userAccount, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        return View(model);
    }

    public IActionResult VerifyEmail()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong.");
                return View(model);
            }

            return RedirectToAction("ForgotPassword", "Account", new { username = user.UserName });
        }

        return View(model);
    }

    public IActionResult ForgotPassword(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("VerifyEmail", "Account");
        }

        return View(new ForgotPasswordViewModel { Email = username });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(model.Email);

            if (user != null)
            {
                var result = await _userManager.RemovePasswordAsync(user);
                
                if (result.Succeeded)
                {
                    await _userManager.AddPasswordAsync(user, model.NewPassword);
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Email not found.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Something went wrong. Try again later.");
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}