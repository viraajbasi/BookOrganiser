namespace BookOrganiser.Models;

public class UserBooks
{
    public string UserName { get; set; } = string.Empty;
    public List<string> BookIds { get; set; } = new List<string>();
}