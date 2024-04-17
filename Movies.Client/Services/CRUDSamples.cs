

using Movies.Client.Helpers;
using Movies.Client.Models;
using System.Text.Json;
using System.Xml.Serialization;

namespace Movies.Client.Services;

public class CRUDSamples : IIntegrationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptionsWrapper _jsonSerializerOptionsWrapper;

    public CRUDSamples(IHttpClientFactory httpClientFactory,
        JsonSerializerOptionsWrapper jsonSerializerOptionsWrapper)
    {
        _jsonSerializerOptionsWrapper = jsonSerializerOptionsWrapper?? throw new ArgumentNullException(nameof(jsonSerializerOptionsWrapper));
        _httpClientFactory = httpClientFactory?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task RunAsync()
    {
        await GetResourceAsync<Movie>();
        await GetResourceThroughHttpRequestMessageAsync<Movie>();
    }


    public async Task GetResourceThroughHttpRequestMessageAsync<T>()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");
        //httpClient.BaseAddress = new Uri("http://localhost:5001");
        //httpClient.Timeout = new TimeSpan(0, 0, 30);

        // enable request message handler

        var request = new HttpRequestMessage(HttpMethod.Get, "api/movies");

        // enble the request header
        request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        IEnumerable<T> movies = new List<T>();
        movies = JsonSerializer.Deserialize<IEnumerable<T>>(content, _jsonSerializerOptionsWrapper.Options);
    }

    public async Task GetResourceAsync<T>() 
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");
       // httpClient.BaseAddress = new Uri("http://localhost:5001");
        //httpClient.Timeout = new TimeSpan(0, 0, 30);

        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));

        var respose = await httpClient.GetAsync("api/movies");
        respose.EnsureSuccessStatusCode();
        
        var content = await respose.Content.ReadAsStringAsync();
        IEnumerable<T> movies = new List<T>();
        if(respose.Content.Headers.ContentType?.MediaType == "application/json")
        {
            movies = JsonSerializer.Deserialize<IEnumerable<T>>(content, _jsonSerializerOptionsWrapper.Options);
        }

        if (respose.Content.Headers.ContentType?.MediaType == "application/xml")
        {
           var serialize  = new XmlSerializer(typeof(List<T>));
            movies = serialize.Deserialize(new StringReader(content)) as List<T>;
        }

    }
}
