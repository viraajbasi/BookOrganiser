namespace BookOrganiser.Models;

public class Book
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public UserAccount UserAccount { get; set; } = null!;
    public string GoogleBooksID { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public IList<string> Authors { get; set; } = new List<string>();
    public string Publisher { get; set; } = string.Empty;
    public string PublishedDate { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ISBN10 { get; set; } = string.Empty;
    public string ISBN13 { get; set; } = string.Empty;
    public string ISSN { get; set; } = string.Empty;
    public string OtherIdentifier  { get; set; } = string.Empty;
    public int PageCount { get; set; }
    public IList<string> GoogleBooksCategories { get; set; } = new List<string>();
    public string Thumbnail { get; set; } = string.Empty;
    public string SmallThumbnail { get; set; } = string.Empty;
    public string SmallImage { get; set; } = string.Empty;
    public string MediumImage  { get; set; } = string.Empty;
    public string LargeImage { get; set; } = string.Empty;
    public string ExtraLargeImage { get; set; } = string.Empty;
    public string GoogleBooksLink { get; set; } = string.Empty;
    public IList<string> CustomCategories { get; set; } = new List<string>();
}