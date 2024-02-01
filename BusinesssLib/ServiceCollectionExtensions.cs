using Bogus;
using Microsoft.Extensions.DependencyInjection;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWireMock(this IServiceCollection services)
    {
        var faker = new Faker<Product>()
            .RuleFor(p => p.Id, f => f.Random.Int(1))
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Lorem.Sentence())
            .RuleFor(p => p.Price, f => f.Finance.Amount())
            .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
            .RuleFor(p => p.Manufacturer, f => f.Company.CompanyName())
            .RuleFor(p => p.SKU, f => f.Commerce.Ean13())
            .RuleFor(p => p.StockQuantity, f => f.Random.Int(1, 100))
            .RuleFor(p => p.IsAvailable, f => f.Random.Bool())
            .RuleFor(p => p.DateAdded, f => f.Date.Past());

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = 9091
        });
        Environment.SetEnvironmentVariable("BASE_URI", server.Url);
        server.Given(Request.Create().WithPath("/products/*").UsingGet()
            .WithHeader("Authorization","*"))
            .RespondWith(Response.Create().WithStatusCode(200).WithBodyAsJson(faker.Generate()));

        server.Given(Request.Create().WithPath("/products").UsingPost().WithHeader("Authorization", "*"))
            .RespondWith(Response.Create().WithStatusCode(201).WithBodyAsJson(faker.Generate()));

        return services;
    }
}
