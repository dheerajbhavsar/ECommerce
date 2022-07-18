using ECommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace ECommerce.ProductCatalog;

public class ServiceFabricProductRepository : IProductRepository
{
    private readonly IReliableStateManager _stateManager;

    public ServiceFabricProductRepository(IReliableStateManager stateManager)
    {
        _stateManager = stateManager;
    }
    
    public async Task AddProduct(Product product)
    {
        var products = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");
        
        using var tx = _stateManager.CreateTransaction();
        await products.AddOrUpdateAsync(tx, product.Id, product, (id, product) => product);

        await tx.CommitAsync();
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        var products = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");

        var result = new List<Product>();

        using var tx = _stateManager.CreateTransaction();
        var allProducts = await products.CreateEnumerableAsync(tx, EnumerationMode.Unordered);

        using var enumerator = allProducts.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync(CancellationToken.None))
        {
            KeyValuePair<Guid, Product> current = enumerator.Current;
            result.Add(current.Value);
        }

        return result;
    }
}
