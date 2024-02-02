using Newtonsoft.Json;

namespace Crosscutting.SellixPayload;

public class SellixCouponObject
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Coupon
    {
        [JsonProperty("id")]
        public int? Id;

        [JsonProperty("uniqid")]
        public string? Uniqid;

        [JsonProperty("shop_id")]
        public int? ShopId;

        [JsonProperty("type")]
        public string? Type;

        [JsonProperty("code")]
        public string? Code;

        [JsonProperty("stripe_promo_id")]
        public object? StripePromoId;

        [JsonProperty("stripe_coupon_id")]
        public object? StripeCouponId;

        [JsonProperty("use_type")]
        public string? UseType;

        [JsonProperty("discount")]
        public int? Discount;

        [JsonProperty("currency")]
        public object? Currency;

        [JsonProperty("used")]
        public int? Used;

        [JsonProperty("disabled_with_volume_discounts")]
        public bool? DisabledWithVolumeDiscounts;

        [JsonProperty("all_recurring_bill_invoices")]
        public bool? AllRecurringBillInvoices;

        [JsonProperty("max_uses")]
        public int? MaxUses;

        [JsonProperty("expire_at")]
        public object? ExpireAt;

        [JsonProperty("created_at")]
        public int? CreatedAt;

        [JsonProperty("updated_at")]
        public int? UpdatedAt;

        [JsonProperty("updated_by")]
        public int? UpdatedBy;

        [JsonProperty("products_count")]
        public int? ProductsCount;
    }

    public class Data
    {
        [JsonProperty("coupons")]
        public List<Coupon>? Coupons;
    }

    public class Root
    {
        [JsonProperty("status")]
        public int? Status;

        [JsonProperty("data")]
        public Data? Data;

        [JsonProperty("error")]
        public object? Error;

        [JsonProperty("message")]
        public object? Message;

        [JsonProperty("env")]
        public string? Env;
    }
}