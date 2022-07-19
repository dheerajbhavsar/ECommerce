using Newtonsoft.Json;

namespace ECommerce.API.Models;

public class ApiBasketAddRequest
{
    [JsonProperty("productId")]
    public Guid ProductId { get; set; }

    [JsonProperty("quantity")]
    public int Quantity { get; set; }
}
