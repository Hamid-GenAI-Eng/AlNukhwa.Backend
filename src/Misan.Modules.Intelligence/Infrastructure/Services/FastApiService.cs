using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Misan.Shared.Kernel.Abstractions;

namespace Misan.Modules.Intelligence.Infrastructure.Services;

public interface IFastApiService
{
    Task<string> ChatAsync(string prompt, object? context = null);
}

public class FastApiService : IFastApiService
{
    private readonly HttpClient _httpClient;

    public FastApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> ChatAsync(string prompt, object? context = null)
    {
        try
        {
            var payload = new { prompt = prompt, context = context };
            var response = await _httpClient.PostAsJsonAsync("/chat", payload);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FastApiResponse>();
                return result?.Response ?? "AI response empty.";
            }
            return $"AI Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
        }
        catch (HttpRequestException ex)
        {
             return $"AI Connection Error: {ex.Message}";
        }
    }
    
    private class FastApiResponse { public string Response { get; set; } = string.Empty; }
}
