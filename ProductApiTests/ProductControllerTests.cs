using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

[TestFixture]
public class ProductControllerTests
{
    private Mock<IProductService> _mockProductService;
    private Mock<ILogger<ProductController>> _mockLogger;
    private ProductController _controller;

    [SetUp]
    public void Setup()
    {
        _mockProductService = new Mock<IProductService>();
        _mockLogger = new Mock<ILogger<ProductController>>();
        _controller = new ProductController(_mockProductService.Object, _mockLogger.Object);
    }

    [Test]
    public async Task GetProduct_ReturnsProductWithExpectedId()
    {
        // Arrange
        var expectedProduct = new Product { Id = 1 };
        _mockProductService.Setup(s => s.GetProduct(It.IsAny<int>())).ReturnsAsync(expectedProduct);

        // Act
        var result = await _controller.GetProduct(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var actualProduct = okResult.Value.Should().BeAssignableTo<Product>().Subject;
        actualProduct.Id.Should().Be(expectedProduct.Id);
    }
}
