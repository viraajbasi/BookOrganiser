using System.Security.Claims;
using BookOrganiser.Controllers;
using BookOrganiser.Data;
using BookOrganiser.Models;
using BookOrganiser.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BookOrganiserTests.ControllerTests;

public class AccountControllerTests
{
    private readonly Mock<UserManager<UserAccount>> _userManagerMock;
    private readonly Mock<SignInManager<UserAccount>> _signInManagerMock;
    private readonly AppDbContext _context;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        var store = new Mock<IUserStore<UserAccount>>();
        _userManagerMock = new Mock<UserManager<UserAccount>>(store.Object, null, null, null, null, null, null, null, null);
        _signInManagerMock = new Mock<SignInManager<UserAccount>>(
            _userManagerMock.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<UserAccount>>().Object,
            null, null, null, null);

        _controller = new AccountController(_signInManagerMock.Object, _userManagerMock.Object, _context);
    }

    [Fact]
    public void Login_ReturnsView()
    {
        var result = _controller.Login();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Register_ReturnsView()
    {
        var result = _controller.Register();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Logout_SignsOutAndRedirects()
    {
        _signInManagerMock.Setup(s => s.SignOutAsync()).Returns(Task.CompletedTask);

        var result = await _controller.Logout();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
    }

    [Fact]
    public void ForgotPassword_ValidUsername_ReturnsViewWithModel()
    {
        var username = "test@example.com";
        var result = _controller.ForgotPassword(username);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ForgotPasswordViewModel>(view.Model);
        Assert.Equal(username, model.Email);
    }

    [Fact]
    public void VerifyEmail_ReturnsView()
    {
        var result = _controller.VerifyEmail();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void ChangePassword_UnauthenticatedUser_RedirectsToLogin()
    {
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((UserAccount)null);

        var result = _controller.ChangePassword().Result;

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
        Assert.Equal("Account", redirect.ControllerName);
    }

    [Fact]
    public void ConfirmPasswordChange_ReturnsView()
    {
        var result = _controller.ConfirmPasswordChange();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task AIFeatures_UnauthenticatedUser_RedirectsToLogin()
    {
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((UserAccount)null);

        var result = await _controller.AIFeatures();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
    }

    [Fact]
    public void ConfirmAIChoice_ReturnsView()
    {
        var result = _controller.ConfirmAIChoice();
        Assert.IsType<ViewResult>(result);
    }
}
