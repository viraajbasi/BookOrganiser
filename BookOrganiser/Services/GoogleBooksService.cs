using BookOrganiser.Models;
using BookOrganiser.Services.Interfaces;
using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using Google.Apis.Services;

namespace BookOrganiser.Services;

public class GoogleBooksService : IGoogleBooksService
{
    private readonly BooksService _booksService;

    public GoogleBooksService()
    {
        _booksService = new BooksService(new BaseClientService.Initializer());
    }

    public async Task<Volume?> GetBookByISBNAsync(string isbn)
    {
        var request = _booksService.Volumes.List($"isbn:{isbn}");
        request.OrderBy = VolumesResource.ListRequest.OrderByEnum.Relevance;
        request.Fields = "items(id,volumeInfo(title,subtitle,authors,publisher,publishedDate,description,industryIdentifiers,pageCount,categories,imageLinks,previewLink))";
        request.MaxResults = 1;
        
        var response = await request.ExecuteAsync();

        if (response.Items != null && response.Items.Count > 0)
        {
            return response.Items.ToList()[0];
        }

        return null;
    }

    public async Task<List<Volume>?> GetBookByTitleAsync(string title)
    {
        var request = _booksService.Volumes.List($"{title}");
        request.OrderBy = VolumesResource.ListRequest.OrderByEnum.Relevance;
        request.Fields = "items(id,volumeInfo(title,authors,publisher,publishedDate,description,industryIdentifiers,imageLinks(thumbnail)))";
        request.MaxResults = 40;

        var response = await request.ExecuteAsync();

        if (response != null && response.Items.Count > 0)
        {
            return response.Items.ToList();
        }

        return null;
    }

    public Book ConvertToBookModel(Volume book, UserAccount user)
    {
        var identifiers = book.VolumeInfo.IndustryIdentifiers?.ToDictionary(i => i.Type, i=> i.Identifier)
                          ?? new Dictionary<string, string>();

        return new Book()
        {
            UserId = user.Id,
            UserAccount = user,
            GoogleBooksID = book.Id ?? string.Empty,
            Title = book.VolumeInfo.Title ?? string.Empty,
            Subtitle = book.VolumeInfo.Subtitle ?? string.Empty,
            Authors = book.VolumeInfo.Authors ?? new List<string>(),
            Publisher = book.VolumeInfo.Publisher ?? string.Empty,
            PublishedDate = book.VolumeInfo.PublishedDate ?? string.Empty,
            Description = book.VolumeInfo.Description ?? string.Empty,
            ISBN10 = identifiers.TryGetValue("ISBN_10", out var isbn10) ? isbn10 : string.Empty,
            ISBN13 = identifiers.TryGetValue("ISBN_13", out var isbn13) ? isbn13 : string.Empty,
            ISSN = identifiers.TryGetValue("ISSN", out var issn) ? issn : string.Empty,
            OtherIdentifier = identifiers.TryGetValue("OTHER", out var other) ? other : string.Empty,
            PageCount = book.VolumeInfo.PageCount ?? 0,
            GoogleBooksCategories = book.VolumeInfo.Categories ?? new List<string>(),
            Thumbnail = book.VolumeInfo.ImageLinks?.Thumbnail ?? string.Empty,
            SmallThumbnail = book.VolumeInfo.ImageLinks?.SmallThumbnail ?? string.Empty,
            SmallImage = book.VolumeInfo.ImageLinks?.Small ?? string.Empty,
            MediumImage = book.VolumeInfo.ImageLinks?.Medium ?? string.Empty,
            LargeImage = book.VolumeInfo.ImageLinks?.Large ?? string.Empty,
            ExtraLargeImage = book.VolumeInfo.ImageLinks?.ExtraLarge ?? string.Empty,
            GoogleBooksLink = book.VolumeInfo.InfoLink ?? string.Empty
        };
    }
}