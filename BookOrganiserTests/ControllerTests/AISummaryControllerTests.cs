using System.Security.Claims;
using BookOrganiser.Controllers;
using BookOrganiser.Data;
using BookOrganiser.Models;
using BookOrganiser.Services.Interfaces;
using BookOrganiser.ViewModels.AISummaryViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BookOrganiserTests.ControllerTests;

public class AISummaryControllerTests
{
    private readonly AppDbContext _context;
    private readonly Mock<UserManager<UserAccount>> _userManagerMock;
    private readonly Mock<IAIService> _aiServiceMock;
    private readonly AISummaryController _controller;

    public AISummaryControllerTests()
    {
        var store = new Mock<IUserStore<UserAccount>>();
        _userManagerMock = new Mock<UserManager<UserAccount>>(store.Object, null, null, null, null, null, null, null, null);
        _aiServiceMock = new Mock<IAIService>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        _controller = new AISummaryController(_context, _userManagerMock.Object, _aiServiceMock.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task Summary_IdIsNull_ReturnsRedirectToError()
    {
        var result = await _controller.Summary(null);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Error", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
    }

    [Fact]
    public async Task Summary_UserIsNull_ReturnsRedirectToLogin()
    {
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((UserAccount)null);

        var result = await _controller.Summary(1);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
        Assert.Equal("Account", redirect.ControllerName);
    }

    [Fact]
    public async Task Summary_SummaryNotFound_ReturnsRedirectToError()
    {
        var user = new UserAccount { UserName = "user" };
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var result = await _controller.Summary(999);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Error", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
    }

    [Fact]
    public async Task Summary_ValidId_ReturnsViewWithModel()
    {
        var user = new UserAccount { UserName = "user", AcceptedAIFeatures = true };
        var summary = new AISummary { Id = 1, Book = new Book { Title = "Test Book" } };

        _context.AISummary.Add(summary);
        await _context.SaveChangesAsync();

        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var result = await _controller.Summary(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<SummaryViewModel>(viewResult.Model);
        Assert.Equal(summary, model.Summary);
        Assert.True(model.AcceptedAIFeatures);
    }
}
