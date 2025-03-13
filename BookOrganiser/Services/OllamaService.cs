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
        var categories = string.Join(',', book.UpstreamCategories);
        var prompt = $"""
                     Summarise the book: {book.Title} by {authors} in 250 words or less.
                     According to Google Books, the book is in the following categories: {categories}.
                     You MUST use the following html structure:
                     
                     <p>[PROVIDE A CLEAR, CHRONOLOGICAL SUMMARY OF THE MAIN PLOT EVENTS]</p>
                     
                     Provide ONLY this information.
                     If you cannot provide at least 10 quotes, respond EXACTLY with "Sorry, I am unable to provide a summary for you." and NOTHING ELSE.
                     Provide ONLY the complete HTML structure without any explanation, introduction, or commentary.
                     """;

        return await GenerateResponseAsync(prompt);
    }

    public async Task<string> GenerateKeyQuotesAsync(Book book)
    {
        var authors = string.Join(',', book.Authors);
        var categories = string.Join(',', book.UpstreamCategories);
        var prompt = $"""
                      Provide at least 10 key quotes from the book: {book.Title} by {authors}.
                      According to Google Books, the book is in the following categories: {categories}.
                      You MUST use the following HTML structure:
                      
                      <ol class="quotes-list">
                        <li class="quote-item">
                          <blockquote class="quote-text">"[QUOTE 1]"</blockquote>
                          <p class="quote-context">[BRIEF CONTEXT - 10 WORDS MAX]</p>
                        </li>
                        <li class="quote-item">
                          <blockquote class="quote-text">"[QUOTE 2]"</blockquote>
                          <p class="quote-context">[BRIEF CONTEXT - 10 WORDS MAX]</p>
                        </li>
                        <!-- Continue pattern for quotes 3-10 -->
                      </ol>

                      Provide ONLY this information.
                      Do NOT provide more than 10 quotes.
                      Each quote should be significant to the book's themes, plot, or character development.
                      Use direct quotes with proper punctuation.
                      If you cannot provide at least 10 quotes, respond EXACTLY with "Sorry, I am unable to provide a 10 key quotes for you." and NOTHING ELSE.
                      Provide ONLY the complete HTML structure without any explanation, introduction, or commentary.
                      """;
        
        return await GenerateResponseAsync(prompt);
    }

    public async Task<string> GenerateKeyThemesAsync(Book book)
    {
        var authors = string.Join(',', book.Authors);
        var categories = string.Join(',', book.UpstreamCategories);
        var prompt = $"""
                      Provide at least 3 key themes from the book: {book.Title} by {authors}.
                      According to Google Books, the book is in the following categories: {categories}.
                      You MUST use the following HTML structure:

                      <ul class="themes-list">
                        <li class="theme-item">
                          <h3 class="theme-title">[THEME 1 TITLE]</h3>
                          <p class="theme-explanation">[BRIEF EXPLANATION OF THEME 1 - 50 WORDS MAX]</p>
                          <p class="theme-evidence">Evidence: [SPECIFIC EXAMPLE FROM THE BOOK - 30 WORDS MAX]</p>
                        </li>
                        <li class="theme-item">
                          <h3 class="theme-title">[THEME 2 TITLE]</h3>
                          <p class="theme-explanation">[BRIEF EXPLANATION OF THEME 2 - 50 WORDS MAX]</p>
                          <p class="theme-evidence">Evidence: [SPECIFIC EXAMPLE FROM THE BOOK - 30 WORDS MAX]</p>
                        </li>
                        <li class="theme-item">
                          <h3 class="theme-title">[THEME 3 TITLE]</h3>
                          <p class="theme-explanation">[BRIEF EXPLANATION OF THEME 3 - 50 WORDS MAX]</p>
                          <p class="theme-evidence">Evidence: [SPECIFIC EXAMPLE FROM THE BOOK - 30 WORDS MAX]</p>
                        </li>
                      </ul>
                        
                      Provide ONLY this information.
                      Do NOT provide more than 3 themes.
                      Each theme should be significant and central to the book's meaning
                      Provide a brief explanation for each theme (50 words maximum)
                      Include one specific example from the book that illustrates each theme (30 words maximum)
                      If you cannot provide at least 10 quotes, respond EXACTLY with "Sorry, I am unable to provide a 3 key themes for you." and NOTHING ELSE.
                      Provide ONLY the complete HTML structure without any explanation, introduction, or commentary.
                      """;
        
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
        public ChatMessage? Message { get; set; }
    
        [JsonProperty("model")]
        public string? Model { get; set; }

        [JsonProperty("created_at")]
        public string? CreatedAt { get; set; }

        [JsonProperty("done")]
        public bool? Done { get; set; }
    }

    private class ChatMessage
    {
        [JsonProperty("role")]
        public string? Role { get; set; }
    
        [JsonProperty("content")]
        public string? Content { get; set; }
    }
}