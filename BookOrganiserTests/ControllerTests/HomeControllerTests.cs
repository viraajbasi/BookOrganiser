using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using BookOrganiser.Controllers;
using BookOrganiser.Data;
using BookOrganiser.Models;
using BookOrganiser.ViewModels.HomeViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookOrganiserTests.ControllerTests;

public class HomeControllerTests
{
    private readonly Mock<UserManager<UserAccount>> _userManagerMock;
    private readonly AppDbContext _context;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        var store = new Mock<IUserStore<UserAccount>>();
        _userManagerMock = new Mock<UserManager<UserAccount>>(store.Object, null, null, null, null, null, null, null, null);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test class instance
            .Options;

        _context = new AppDbContext(options);

        _controller = new HomeController(_context, _userManagerMock.Object);
    }

    private void SetUserContext()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user1"),
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task Index_ReturnsViewResult_WithViewModel()
    {
        var user = new UserAccount { Id = "user1", UserCategories = new List<string> { "fiction" } };
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        _context.Books.Add(new Book { Id = 1, Title = "Test Book", UserId = "user1" });
        _context.SaveChanges();

        SetUserContext();

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.Model);
        Assert.Single(model.Books);
        Assert.Contains("fiction", model.Categories);
    }

    [Fact]
    public async Task EditCategories_ReturnsViewResult_WithCategories()
    {
        var user = new UserAccount { Id = "user1", UserCategories = new List<string> { "science" } };
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        SetUserContext();

        var result = await _controller.EditCategories();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<EditCategoriesViewModel>(viewResult.Model);
        Assert.Contains("science", model.Categories);
    }

    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        var result = _controller.Privacy();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Error_ReturnsViewResult_WithErrorViewModel()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.HttpContext.TraceIdentifier = "test-trace-id";

        var result = _controller.Error("Something went wrong", true, 500);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<ErrorViewModel>(viewResult.Model);
        Assert.Equal("Something went wrong", model.ErrorMessage);
        Assert.Equal(500, model.StatusCode);
        Assert.True(model.ShowInfoText);
        Assert.Equal("test-trace-id", model.RequestId);
    }

    [Fact]
    public async Task AddCategory_AddsCategory_AndRedirects()
    {
        var user = new UserAccount { Id = "user1", UserCategories = new List<string>() };
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        SetUserContext();

        var result = await _controller.AddCategory("History");

        Assert.Contains("history", user.UserCategories);
        Assert.IsType<RedirectToActionResult>(result);
    }

    [Fact]
    public async Task DeleteCategory_RemovesCategory_AndRedirects()
    {
        var user = new UserAccount { Id = "user1", UserCategories = new List<string> { "thriller" } };
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        _context.Books.Add(new Book
        {
            Id = 1,
            Title = "Test Book",
            UserAccount = user,
            CustomCategories = new List<string> { "thriller" }
        });
        _context.SaveChanges();

        SetUserContext();

        var result = await _controller.DeleteCategory("thriller");

        Assert.DoesNotContain("thriller", user.UserCategories);
        var book = _context.Books.First();
        Assert.DoesNotContain("thriller", book.CustomCategories);
        Assert.IsType<RedirectToActionResult>(result);
    }
}