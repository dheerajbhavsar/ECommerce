using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace UserActor.Interfaces;

/// <summary>
/// This interface defines the methods exposed by an actor.
/// Clients use this interface to interact with the actor that implements it.
/// </summary>
public interface IUserActor : IActor
{
    /// <summary>
    /// Add the Product quantity to basket
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    Task AddToBasket(Guid productId, int quantity);

    /// <summary>
    /// Get the basket for user
    /// </summary>
    /// <returns></returns>
    Task<BasketItem[]> GetBasket();
    
    /// <summary>
    /// Remove products from basket
    /// </summary>
    /// <returns></returns>
    Task ClearBasket();
}
