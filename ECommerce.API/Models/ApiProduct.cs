using Newtonsoft.Json;

namespace ECommerce.API.Models;

public class ApiProduct
{
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("price")]
    public double Price { get; set; }

    [JsonProperty("availability")]
    public bool Availability { get; set; }
}
