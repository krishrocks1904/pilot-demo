using System.Threading.Tasks;

public interface IProductService
{
    Task<Product> GetProduct(int id);
    Task<Product> CreateProduct(Product product);
}
