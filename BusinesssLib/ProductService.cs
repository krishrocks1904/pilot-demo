using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductService> _logger;
    private readonly IConfiguration configuration;
    private readonly string? _baseUri;
    private readonly IConfidentialClientApplication _app;

    public ProductService(HttpClient httpClient, ILogger<ProductService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        this.configuration = configuration;
        _baseUri = Environment.GetEnvironmentVariable("BASE_URI");
        _app = ConfidentialClientApplicationBuilder.Create(configuration["ClientId"])
            .WithClientSecret(configuration["ClientSecret"])
            .WithAuthority(AzureCloudInstance.AzurePublic, configuration["TenantId"])
            .Build();
    }

    private async Task<string> GetToken()
    {

        var result = await _app.AcquireTokenForClient(new[] { configuration["audience"] }).ExecuteAsync();
        return result.AccessToken;

    }

    public async Task<Product> GetProduct(int id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}/products/{id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetToken());
        
        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Error getting product with id {id}. Status code: {response.StatusCode}");
            throw new Exception("Error getting product");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Product>(responseContent);
    }

    public async Task<Product> CreateProduct(Product product)
    {
        var productJson = JsonConvert.SerializeObject(product);
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUri}/products")
        {
            Content = new StringContent(productJson, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetToken());

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Error creating product. Status code: {response.StatusCode}");
            throw new Exception("Error creating product");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Product>(responseContent);
    }
}
