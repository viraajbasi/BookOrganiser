using BookOrganiser.Models;

namespace BookOrganiser.ViewModels.AISummaryViewModels;

public class SummaryViewModel
{
    public bool AcceptedAIFeatures {get; set;}
    public AISummary Summary { get; set; }
}