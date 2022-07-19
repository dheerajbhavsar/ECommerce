using ECommerce.API.Models;
using ECommerce.CheckoutService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;

namespace ECommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CheckoutController : ControllerBase
{
    private static readonly Random rnd = new(DateTime.UtcNow.Second);

    [HttpGet("{userId}")]
    public async Task<ApiCheckoutSummary> CheckoutAsync(string userId)
    {
        var summary = await GetCheckoutService().CheckoutAsync(userId);
        return ToApiCheckoutSummary(summary);
    }

    [HttpGet("history/{userId}")]
    public async Task<IEnumerable<ApiCheckoutSummary>> GetHistoryAsync(string userId)
    {
        var history = await GetCheckoutService().GetOrderHistoryAsync(userId);

        return history.Select(ToApiCheckoutSummary);
    }

    private static ApiCheckoutSummary ToApiCheckoutSummary(CheckoutSummary model)
    {
        return new ApiCheckoutSummary
        {
            Products = model.Products.Select(p => new ApiCheckoutProduct
            {
                ProductId = p.Product.Id,
                ProductName = p.Product.Name,
                Price = p.Price,
                Quantity = p.Quantity
            }).ToList(),
            Date = model.Date,
            TotalPrice = model.TotalPrice
        };
    }

    private static ICheckoutService GetCheckoutService()
    {
        long key = LongRandom();

        var proxyFactory = new ServiceProxyFactory(
           c => new FabricTransportServiceRemotingClientFactory());

        return proxyFactory.CreateServiceProxy<ICheckoutService>(
           new Uri("fabric:/ECommerce/ECommerce.CheckoutService"),
           new ServicePartitionKey(key));
    }

    private static long LongRandom()
    {
        byte[] buf = new byte[8];
        rnd.NextBytes(buf);
        long longRand = BitConverter.ToInt64(buf, 0);
        return longRand;
    }
}
