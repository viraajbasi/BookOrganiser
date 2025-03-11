namespace BookOrganiser.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public string? ErrorMessage { get; set; } = string.Empty;
    public bool ShowInfoText { get; set; }
    public int StatusCode { get; set; }
}
