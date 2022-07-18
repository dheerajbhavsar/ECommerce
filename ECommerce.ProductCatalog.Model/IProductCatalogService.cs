using Microsoft.ServiceFabric.Services.Remoting;

namespace ECommerce.ProductCatalog.Model;

public interface IProductCatalogService : IService
{
    Task<Product[]> GetAllProductsAsync();
    Task AddProductAsync(Product product);
}
