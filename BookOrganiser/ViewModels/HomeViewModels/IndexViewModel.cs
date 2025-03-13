using BookOrganiser.Models;

namespace BookOrganiser.ViewModels.HomeViewModels;

public class IndexViewModel
{
    public IList<string> Categories { get; set; } = new List<string>();

    public List<Book> Books { get; set; } = new();

    public bool HasAnyBooks => Books.Count != 0;
}