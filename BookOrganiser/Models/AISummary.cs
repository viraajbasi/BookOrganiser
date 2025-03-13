namespace BookOrganiser.Models;

public class AISummary
{
    public int Id { get; set; }
    public string Model { get; set; } = "llama3.2";
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public string Summary { get; set; } = string.Empty;
    public string KeyQuotes { get; set; } = string.Empty;
    public string KeyThemes { get; set; } = string.Empty;
    public DateTime? GeneratedAt { get; set; }
    public bool IsGenerated { get; set; }
}