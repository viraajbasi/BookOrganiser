namespace BookOrganiser.Models;

public class UserBooks
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public List<string> BookIds { get; set; } = new();
}