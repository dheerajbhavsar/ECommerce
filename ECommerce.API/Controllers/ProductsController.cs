using ECommerce.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<ApiProduct>> GetAsync()
    {
        return new[] {  new ApiProduct
            {
                Id = Guid.NewGuid(),
                Name = "DELL Latitude",
                Description = "This is product description",
                Price = 1200,
                Availability = true
            }
        };
    }

    [HttpPost]
    public async Task PostAsync(ApiProduct product)
    {

    }
}
