using Newtonsoft.Json;

namespace ECommerce.API.Models;

public class ApiCheckoutSummary
{
    [JsonProperty("products")]
    public List<ApiCheckoutProduct> Products { get; set; }

    [JsonProperty("totalPrice")]
    public double TotalPrice { get; set; }

    [JsonProperty("date")]
    public DateTime Date { get; set; }
}
