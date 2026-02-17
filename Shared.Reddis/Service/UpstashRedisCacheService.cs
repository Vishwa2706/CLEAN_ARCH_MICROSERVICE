using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Shared.Reddis.Contract;

namespace Shared.Reddis.Service;

public class UpstashRedisCacheService : IRedisCacheService
{
    private readonly HttpClient _http;
    private readonly string _url;
    private readonly string _token;

    public UpstashRedisCacheService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _url = config["Upstash:Url"]!;
        _token = config["Upstash:Token"]!;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_url}/set/{key}");

        request.Headers.Add("Authorization", $"Bearer {_token}");
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        await _http.SendAsync(request);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_url}/get/{key}");

        request.Headers.Add("Authorization", $"Bearer {_token}");

        var response = await _http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return default;

        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("result", out var result))
            return default;

        if (result.ValueKind == JsonValueKind.Null)
            return default;

        return JsonSerializer.Deserialize<T>(result.GetString()!);
    }

    public async Task DeleteAsync(string key)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_url}/del/{key}");

        request.Headers.Add("Authorization", $"Bearer {_token}");

        await _http.SendAsync(request);
    }
}
