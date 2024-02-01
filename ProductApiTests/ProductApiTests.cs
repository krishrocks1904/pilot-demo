using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ProductApiTests
{
    public class ProductServiceTests
    {
        private ProductService _productService;
        private Mock<HttpClient> _mockHttpClient;
        private Mock<ILogger<ProductService>> _mockLogger;
        private Mock<IConfiguration> _mockConfiguration;

        [SetUp]
        public void Setup()
        {
            _mockHttpClient = new Mock<HttpClient>();
            _mockLogger = new Mock<ILogger<ProductService>>();
            _mockConfiguration = new Mock<IConfiguration>();

            _productService = new ProductService(_mockHttpClient.Object, _mockLogger.Object, _mockConfiguration.Object);
        }

        [Test]
        public async Task GetProduct_WithValidId_ReturnsProduct()
        {
            // Arrange
            int productId = 1;
            string baseUri = "https://example.com";
            string accessToken = "dummyAccessToken";
            var expectedProduct = new Product { Id = productId, Name = "Test Product" };

            _mockConfiguration.Setup(c => c["ClientId"]).Returns("dummyClientId");
            _mockConfiguration.Setup(c => c["ClientSecret"]).Returns("dummyClientSecret");
            _mockConfiguration.Setup(c => c["TenantId"]).Returns("dummyTenantId");
            _mockConfiguration.Setup(c => c["audience"]).Returns("dummyAudience");
            _mockConfiguration.Setup(c => c["BASE_URI"]).Returns(baseUri);

            _mockHttpClient
                .Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(expectedProduct))
                });

            // Act
            var result = await _productService.GetProduct(productId);

            // Assert
            result.Should().BeEquivalentTo(expectedProduct);
            _mockHttpClient.Verify(c => c.SendAsync(It.IsAny<HttpRequestMessage>()), Times.Once);
        }

        [Test]
        public void GetProduct_WithInvalidId_ThrowsException()
        {
            // Arrange
            int productId = -1;
            string baseUri = "https://example.com";
            string accessToken = "dummyAccessToken";

            _mockConfiguration.Setup(c => c["ClientId"]).Returns("dummyClientId");
            _mockConfiguration.Setup(c => c["ClientSecret"]).Returns("dummyClientSecret");
            _mockConfiguration.Setup(c => c["TenantId"]).Returns("dummyTenantId");
            _mockConfiguration.Setup(c => c["audience"]).Returns("dummyAudience");
            _mockConfiguration.Setup(c => c["BASE_URI"]).Returns(baseUri);

            _mockHttpClient
                .Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            // Act
            Func<Task> act = async () => await _productService.GetProduct(productId);

            // Assert
            act.Should().ThrowAsync<Exception>().WithMessage("Error getting product");
            _mockHttpClient.Verify(c => c.SendAsync(It.IsAny<HttpRequestMessage>()), Times.Once);
        }

        [Test]
        public async Task CreateProduct_WithValidProduct_ReturnsCreatedProduct()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Test Product" };
            string baseUri = "https://example.com";
            string accessToken = "dummyAccessToken";
            var expectedProduct = new Product { Id = 1, Name = "Test Product" };

            _mockConfiguration.Setup(c => c["ClientId"]).Returns("dummyClientId");
            _mockConfiguration.Setup(c => c["ClientSecret"]).Returns("dummyClientSecret");
            _mockConfiguration.Setup(c => c["TenantId"]).Returns("dummyTenantId");
            _mockConfiguration.Setup(c => c["audience"]).Returns("dummyAudience");
            _mockConfiguration.Setup(c => c["BASE_URI"]).Returns(baseUri);

            _mockHttpClient
                .Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Created,
                    Content = new StringContent(JsonConvert.SerializeObject(expectedProduct))
                });

            // Act
            var result = await _productService.CreateProduct(product);

            // Assert
            result.Should().BeEquivalentTo(expectedProduct);
            _mockHttpClient.Verify(c => c.SendAsync(It.IsAny<HttpRequestMessage>()), Times.Once);
        }

        [Test]
        public void CreateProduct_WithInvalidProduct_ThrowsException()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Test Product" };
            string baseUri = "https://example.com";
            string accessToken = "dummyAccessToken";

            _mockConfiguration.Setup(c => c["ClientId"]).Returns("dummyClientId");
            _mockConfiguration.Setup(c => c["ClientSecret"]).Returns("dummyClientSecret");
            _mockConfiguration.Setup(c => c["TenantId"]).Returns("dummyTenantId");
            _mockConfiguration.Setup(c => c["audience"]).Returns("dummyAudience");
            _mockConfiguration.Setup(c => c["BASE_URI"]).Returns(baseUri);

            _mockHttpClient
                .Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            // Act
            Func<Task> act = async () => await _productService.CreateProduct(product);

            // Assert
            act.Should().ThrowAsync<Exception>().WithMessage("Error creating product");
            _mockHttpClient.Verify(c => c.SendAsync(It.IsAny<HttpRequestMessage>()), Times.Once);
        }
    }
}
