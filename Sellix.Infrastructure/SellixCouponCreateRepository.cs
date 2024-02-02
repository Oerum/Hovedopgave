using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using Crosscutting.SellixPayload;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sellix.Application.Interfaces;


namespace Sellix.Infrastructure;

public class SellixCouponCreateRepository : ISellixCouponCreateRepository
{
    private readonly ILogger<SellixCouponCreateRepository> _logger;
    private readonly HttpClient _httpClient;

    public SellixCouponCreateRepository(ILogger<SellixCouponCreateRepository> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("SellixAPI");
    }

    async Task<string> ISellixCouponCreateRepository.CreateCoupon(string discordId)
    {
        try
        {
            _logger.LogInformation("Create Coupon For: " + discordId);

            try
            {
                var couponResult = await _httpClient.GetAsync("coupons");

                if (couponResult.IsSuccessStatusCode)
                {
                    var couponsBody = await couponResult.Content.ReadAsStringAsync();

                    var couponsJsonBody = JsonConvert.DeserializeObject<SellixCouponObject.Root>(couponsBody);

                    if (couponsJsonBody is { Data.Coupons: not null })
                    {
                        foreach (var coupon in couponsJsonBody.Data?.Coupons!)
                        {
                            if (coupon.Code == discordId)
                            {
                                await _httpClient.DeleteAsync($"coupons/:{coupon.Uniqid}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Get Coupons Error", ex);
            }

            try
            {
                var couponPayload = new
                {
                    code = discordId,
                    discount_value = 10,
                    max_uses = 1,
                    products_bound = Array.Empty<string>(),
                    discount_type = "PERCENTAGE",
                    disabled_with_volume_discounts = false,
                    //expire_at = DateTime.UtcNow.AddHours(1).ToString("MMM d, yyyy")
                };

                var couponCreateResult = await _httpClient.PostAsJsonAsync("coupons", couponPayload);

                if (couponCreateResult.IsSuccessStatusCode)
                {
                    return $"Code: {couponPayload.code}" +
                           "\nDiscount: 10%" +
                           $"\nExpires: {DateTime.UtcNow.AddHours(1):MMM d, yyyy}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Create Coupons Error", ex);
            }

            throw new Exception("Unsuccessful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new Exception(ex.Message);
        }
    }
}