using System.Text;
using BookOrganiser.Models;
using BookOrganiser.Services.Interfaces;
using Newtonsoft.Json;

namespace BookOrganiser.Services;

public class OllamaService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _defaultModel;

    public OllamaService(HttpClient httpClient, string baseUrl = "http://localhost:11434", string defaultModel = "llama3.2")
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
        _defaultModel = defaultModel;
    }

    public async Task<string> GenerateBookSummaryAsync(Book book)
    {
        var authors = string.Join(',', book.Authors);
        var categories = string.Join(',', book.GoogleBooksCategories);
        var prompt = $"Generate a concise summary for the book title: '{book.Title}' by '{authors}'." + 
                     $"The book is in the following categories: '{categories}' and was published on {book.PublishedDate}." +
                     $"If available, consider these details from the description: {book.Description}.";

        return await GenerateResponseAsync(prompt);
    }

    public async Task<string> GenerateKeyQuotesAsync(Book book)
    {
        var authors = string.Join(',', book.Authors);
        var categories = string.Join(',', book.GoogleBooksCategories);
        var prompt = $"Generate ten key quotes from the book: '{book.Title}' by '{authors}'." + 
                     $"The book is in the following categories: '{categories}' and was published on {book.PublishedDate}." +
                     $"If available, consider these details from the description: {book.Description}.";
        
        return await GenerateResponseAsync(prompt);
    }

    public async Task<string> GenerateKeyThemesAsync(Book book)
    {
        var authors = string.Join(',', book.Authors);
        var categories = string.Join(',', book.GoogleBooksCategories);
        var prompt = $"Summarise the key themes from the book: '{book.Title}' by '{authors}'." + 
                     $"The book is in the following categories: '{categories}' and was published on {book.PublishedDate}." +
                     $"If available, consider these details from the description: {book.Description}.";
        
        return await GenerateResponseAsync(prompt);
    }

    private async Task<string> GenerateResponseAsync(string prompt, string? model = null)
    {
        var requestUrl = $"{_baseUrl}/api/chat";
        var requestModel = model ?? _defaultModel;

        var requestBody = new
        {
            model = requestModel,
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            stream = false
        };

        var content = new StringContent(
        JsonConvert.SerializeObject(requestBody),
        Encoding.UTF8,
        "applications/json"
        );

        try
        {
            var response = await _httpClient.PostAsync(requestUrl, content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var ollamaResponse = JsonConvert.DeserializeObject<OllamaChatResponse>(jsonResponse);

            return ollamaResponse?.Message?.Content ?? string.Empty;
        }
        catch (HttpRequestException e)
        {
            throw new ApplicationException("An error occured while generating the AI response.", e);
        }
    }
    
    private class OllamaChatResponse
    {
        [JsonProperty("message")]
        public ChatMessage Message { get; set; }
    
        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("done")]
        public bool Done { get; set; }
    }

    private class ChatMessage
    {
        [JsonProperty("role")]
        public string Role { get; set; }
    
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}