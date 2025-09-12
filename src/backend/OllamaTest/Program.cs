using System.Net.Http.Json;

while(true)
{
    OllamaClient client = new OllamaClient(new HttpClient());
    var response = await client.AskAsync("Como te va?");
    Console.WriteLine(response);
}


public class OllamaClient
{
    private readonly HttpClient _http;

    public OllamaClient(HttpClient http) => _http = http;

    public async Task<string> AskAsync(string prompt)
    {
        var request = new
        {
            model = "llama3",
            prompt = prompt,
            stream = false
        };

        var response = await _http.PostAsJsonAsync("http://localhost:11434/api/generate", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();
        return result?.Response ?? string.Empty;
    }

    private class OllamaResponse
    {
        public string Response { get; set; } = "";
    }
}
