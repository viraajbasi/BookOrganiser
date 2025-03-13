namespace BookOrganiser.ViewModels.HomeViewModels;

public class EditCategoriesViewModel
{
    public IList<string> Categories { get; set; } = new List<string>();
    public bool HasAnyCategories => Categories.Count > 0;
}