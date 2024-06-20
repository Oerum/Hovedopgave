using Newtonsoft.Json;

namespace Crosscutting.SellixPayload;

public class SellixCouponObject
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Coupon
    {
        [JsonProperty("id")]
        public int? id { get; set; }

        [JsonProperty("uniqid")]
        public string? uniqid { get; set; }

        [JsonProperty("shop_id")]
        public int? shop_id { get; set; }

        [JsonProperty("type")]
        public string? type { get; set; }

        [JsonProperty("coupon_type")]
        public string? coupon_type { get; set; }

        [JsonProperty("code")]
        public string? code { get; set; }

        [JsonProperty("stripe_promo_id")]
        public object? stripe_promo_id { get; set; }

        [JsonProperty("stripe_coupon_id")]
        public object? stripe_coupon_id { get; set; }

        [JsonProperty("use_type")]
        public string? use_type { get; set; }

        [JsonProperty("discount")]
        public double? discount { get; set; }

        [JsonProperty("currency")]
        public object? currency { get; set; }

        [JsonProperty("used")]
        public int? used { get; set; }

        [JsonProperty("disabled_with_volume_discounts")]
        public bool? disabled_with_volume_discounts { get; set; }

        [JsonProperty("all_recurring_bill_invoices")]
        public bool? all_recurring_bill_invoices { get; set; }

        [JsonProperty("max_uses")]
        public int? max_uses { get; set; }

        [JsonProperty("smart_contract_address")]
        public object? smart_contract_address { get; set; }

        [JsonProperty("token_id")]
        public object? token_id { get; set; }

        [JsonProperty("blockchain")]
        public object? blockchain { get; set; }

        [JsonProperty("expire_at")]
        public string? expire_at { get; set; }

        [JsonProperty("created_at")]
        public int? created_at { get; set; }

        [JsonProperty("updated_at")]
        public int? updated_at { get; set; }

        [JsonProperty("updated_by")]
        public int? updated_by { get; set; }

        [JsonProperty("products_count")]
        public int? products_count { get; set; }
    }

    public class Data
    {
        [JsonProperty("coupons")]
        public List<Coupon>? coupons;
    }

    public class Root
    {
        [JsonProperty("status")]
        public int? status { get; set; }

        [JsonProperty("data")]
        public Data? data { get; set; }

        [JsonProperty("error")]
        public object? error { get; set; }

        [JsonProperty("message")]
        public object? message { get; set; }

        [JsonProperty("env")]
        public string? env { get; set; }
    }


}