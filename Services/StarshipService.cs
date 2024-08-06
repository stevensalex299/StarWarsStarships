using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class StarshipService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://swapi.dev/api/starships/";

    public StarshipService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Starship[]> GetAllStarshipsAsync()
    {
        var allStarships = new List<Starship>();
        var nextUrl = BaseUrl;

        var settings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };
        
        // Grab all pages of starships
        while (nextUrl != null)
        {
            var response = await _httpClient.GetStringAsync(nextUrl);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response, settings);
            
            if (apiResponse?.Results != null)
            {
                allStarships.AddRange(apiResponse.Results);
            }
            
            nextUrl = apiResponse?.Next;
        }

        return [.. allStarships];
    }

    private class ApiResponse
        {
            public int Count { get; set; }
            public string? Next { get; set; }
            public string? Previous { get; set; }
            public Starship[]? Results { get; set; }
        }
}
