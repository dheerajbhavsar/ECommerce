namespace ECommerce.ProductCatalog.Model;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task AddProduct(Product product);
}
