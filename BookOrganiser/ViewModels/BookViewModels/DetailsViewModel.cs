using BookOrganiser.Models;

namespace BookOrganiser.ViewModels.BookViewModels;

public class DetailsViewModel
{
    public IList<string> Categories { get; set; } = new List<string>();
    
    public Book Book { get; set; }
}