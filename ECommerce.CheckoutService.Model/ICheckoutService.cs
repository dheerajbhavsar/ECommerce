﻿using Microsoft.ServiceFabric.Services.Remoting;

namespace ECommerce.CheckoutService.Model;

public interface ICheckoutService : IService
{
    Task<CheckoutSummary> CheckoutAsync(string userId);
    Task<CheckoutSummary[]> GetOrderHistoryAsync(string userId);
}
