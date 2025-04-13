using Moq;
using BookOrganiser.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using BookOrganiser.Models;
using BookOrganiser.Services.Interfaces;
using BookOrganiser.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookOrganiserTests.ControllerTests;

public class BookControllerTests
{
    private readonly Mock<UserManager<UserAccount>> _userManagerMock;
    private readonly Mock<IBooksService> _booksServiceMock;
    private readonly AppDbContext _dbContext;

    public BookControllerTests()
    {
        var userStoreMock = new Mock<IUserStore<UserAccount>>();
        _userManagerMock = new Mock<UserManager<UserAccount>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        _booksServiceMock = new Mock<IBooksService>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test class instance
            .Options;
        
        _dbContext = new AppDbContext(options);
    }

    private BookController CreateControllerWithSession()
    {
        var controller = new BookController(_dbContext, _userManagerMock.Object, _booksServiceMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };
        controller.HttpContext.Session = new DummySession();
        return controller;
    }

    [Fact]
    public async Task Details_Returns_View_When_Book_Found()
    {
        var user = new UserAccount { Id = "1" };
        var book = new Book { Id = 1, Title = "Sample Book", AISummary = new AISummary() };
        _dbContext.Books.Add(book);
        await _dbContext.SaveChangesAsync();

        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var controller = CreateControllerWithSession();
        var result = await controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<BookOrganiser.ViewModels.BookViewModels.DetailsViewModel>(viewResult.Model);
        Assert.Equal("Sample Book", model.Book.Title);
    }

    [Fact]
    public void FindBooksTitle_Get_Returns_View()
    {
        var controller = CreateControllerWithSession();
        var result = controller.FindBooksTitle();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void FindBooksAuthor_Get_Returns_View()
    {
        var controller = CreateControllerWithSession();
        var result = controller.FindBooksAuthor();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void FindBooksISBN_Get_Returns_View()
    {
        var controller = CreateControllerWithSession();
        var result = controller.FindBooksISBN();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task SearchResults_Returns_View_With_Empty_List_When_No_Session_Data()
    {
        var user = new UserAccount { Id = "1" };
        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var controller = CreateControllerWithSession();
        var result = await controller.SearchResults();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Book>>(viewResult.Model);
        Assert.Empty(model);
    }

    [Fact]
    public async Task FindBooksTitle_Post_Redirects_To_SearchResults_When_Found()
    {
        var user = new UserAccount { Id = "1" };
        var books = new List<Book> { new Book { Id = 1, Title = "Test" } };
        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _booksServiceMock.Setup(b => b.GetBooksByTitleAsync("Test", user)).ReturnsAsync(books);

        var controller = CreateControllerWithSession();
        var result = await controller.FindBooksTitle("Test");

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("SearchResults", redirect.ActionName);
    }

    [Fact]
    public async Task AddBookFromSearchResults_Adds_And_Redirects()
    {
        var user = new UserAccount { Id = "1" };
        var newBook = new Book { Id = 2, UpstreamId = "abc123" };

        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _booksServiceMock.Setup(b => b.GetBookByUpstreamIdAsync("abc123", user)).ReturnsAsync(newBook);

        var controller = CreateControllerWithSession();
        var result = await controller.AddBookFromSearchResults("abc123");

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task DeleteBook_Removes_And_Redirects()
    {
        var book = new Book { Id = 3 };
        var aiSummary = new AISummary { BookId = 3 };
        _dbContext.Books.Add(book);
        _dbContext.AISummary.Add(aiSummary);
        await _dbContext.SaveChangesAsync();

        var controller = CreateControllerWithSession();
        var result = await controller.DeleteBook(3);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task AddBookToCategory_Adds_And_Saves()
    {
        var book = new Book { Id = 4, CustomCategories = new List<string>() };
        _dbContext.Books.Add(book);
        await _dbContext.SaveChangesAsync();

        var controller = CreateControllerWithSession();
        var result = await controller.AddBookToCategory(4, "Fiction");

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task RemoveBookFromCategory_Removes_And_Saves()
    {
        var book = new Book { Id = 5, CustomCategories = new List<string> { "Fiction" } };
        _dbContext.Books.Add(book);
        await _dbContext.SaveChangesAsync();

        var controller = CreateControllerWithSession();
        var result = await controller.RemoveBookFromCategory(5, "Fiction");

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }
}

public class DummySession : ISession
{
    private readonly Dictionary<string, byte[]> _storage = new();
    public IEnumerable<string> Keys => _storage.Keys;
    public string Id => "dummy";
    public bool IsAvailable => true;
    public void Clear() => _storage.Clear();
    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void Remove(string key) => _storage.Remove(key);
    public void Set(string key, byte[] value) => _storage[key] = value;
    public bool TryGetValue(string key, out byte[] value) => _storage.TryGetValue(key, out value);
}
