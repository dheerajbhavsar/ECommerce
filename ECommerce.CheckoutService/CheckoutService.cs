using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ECommerce.CheckoutService.Model;
using ECommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using UserActor.Interfaces;

namespace ECommerce.CheckoutService;

/// <summary>
/// An instance of this class is created for each service replica by the Service Fabric runtime.
/// </summary>
internal sealed class CheckoutService : StatefulService, ICheckoutService
{
    public CheckoutService(StatefulServiceContext context)
        : base(context)
    { }

    public async Task<CheckoutSummary> CheckoutAsync(string userId)
    {
        var result = new CheckoutSummary
        {
            Date = DateTime.UtcNow,
            Products = new List<CheckoutProduct>()
        };

        //call user actor to get the basket
        var userActor = GetUserActor(userId);
        var basket = await userActor.GetBasket();

        //get catalog client
        var catalogService = GetProductCatalogService();

        //constuct CheckoutProduct items by calling to the catalog
        foreach (var basketLine in basket)
        {
            var product = await catalogService
                .GetProductAsync(basketLine.ProductId);
            var checkoutProduct = new CheckoutProduct
            {
                Product = product,
                Price = product.Price,
                Quantity = basketLine.Quantity
            };
            result.Products.Add(checkoutProduct);
        }

        //generate total price
        result.TotalPrice = result.Products.Sum(p => p.Price);

        //clear user basket
        await userActor.ClearBasket();

        await AddToHistoryAsync(result);

        return result;
    }

    public async Task<CheckoutSummary[]> GetOrderHistoryAsync(string userId)
    {
        var result = new List<CheckoutSummary>();
        IReliableDictionary<DateTime, CheckoutSummary> history =
           await StateManager.GetOrAddAsync<IReliableDictionary<DateTime, CheckoutSummary>>("history");

        using (var tx = StateManager.CreateTransaction())
        {
            var allProducts = await history.CreateEnumerableAsync(tx, EnumerationMode.Unordered);
            using var enumerator = allProducts.GetAsyncEnumerator();
            while (await enumerator.MoveNextAsync(CancellationToken.None))
            {
                KeyValuePair<DateTime, CheckoutSummary> current = enumerator.Current;

                result.Add(current.Value);
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
    /// </summary>
    /// <remarks>
    /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
    /// </remarks>
    /// <returns>A collection of listeners.</returns>
    protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
    {
        return new[]
        {
            new ServiceReplicaListener(context =>
                new FabricTransportServiceRemotingListener(context, this))
        };
    }

    private static IUserActor GetUserActor(string userId)
    {
        return ActorProxy.Create<IUserActor>(
            new ActorId(userId),
            new Uri("fabric:/ECommerce/UserActorService"));
    }

    private static IProductCatalogService GetProductCatalogService()
    {
        var proxy = new ServiceProxyFactory(c => new FabricTransportServiceRemotingClientFactory());

        return proxy.CreateServiceProxy<IProductCatalogService>(
            new Uri("fabric:/ECommerce/ECommerce.ProductCatalog"),
            new ServicePartitionKey(0));
    }

    private async Task AddToHistoryAsync(CheckoutSummary checkout)
    {
        var history = await StateManager
            .GetOrAddAsync<IReliableDictionary<DateTime, CheckoutSummary>>("history");

        using var tx = StateManager.CreateTransaction();
        await history.AddAsync(tx, checkout.Date, checkout);
        await tx.CommitAsync();
    }
}
