

using Movies.Client.Models;
using System.Text.Json;
using System.Xml.Serialization;

namespace Movies.Client.Services;

public class CRUDSamples : IIntegrationService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CRUDSamples(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task RunAsync()
    {
        await GetResourceAsync<Movie>();
    }

    public async Task GetResourceAsync<T>() 
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("http://localhost:5001");
        httpClient.Timeout = new TimeSpan(0, 0, 30);

        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));

        var respose = await httpClient.GetAsync("api/movies");
        respose.EnsureSuccessStatusCode();
        
        var content = await respose.Content.ReadAsStringAsync();
        IEnumerable<T> movies = new List<T>();
        if(respose.Content.Headers.ContentType?.MediaType == "application/json")
        {
            movies = JsonSerializer.Deserialize<IEnumerable<T>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        if (respose.Content.Headers.ContentType?.MediaType == "application/xml")
        {
           var serialize  = new XmlSerializer(typeof(List<T>));
            movies = serialize.Deserialize(new StringReader(content)) as List<T>;
        }

    }
}
