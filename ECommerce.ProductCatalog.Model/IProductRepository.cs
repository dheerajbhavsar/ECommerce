namespace ECommerce.ProductCatalog.Model;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllProductsAsync();

    Task<Product> GetProduct(Guid productId);

    Task AddProduct(Product product);
}
