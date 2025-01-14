﻿using Newtonsoft.Json;

namespace Crosscutting.SellixPayload
{
#pragma warning disable CS8618

    public class SellixPayloadProductVariants
    {
        public class Product_Variants
        {
            [JsonProperty("price")]
            public double? Price;

            [JsonProperty("title")]
            public string Title;

            [JsonProperty("description")]
            public string Description;
        }

        public class AvailableStripeApm
        {
            [JsonProperty("id")]
            public string Id;

            [JsonProperty("name")]
            public string Name;
        }

        public class CustomFields
        {
            [JsonProperty("hwid")]
            public string HWID;

            [JsonProperty("name")]
            public string Name;

            [JsonProperty("surname")]
            public string Surname;

            [JsonProperty("acf_name")]
            public string AcfName;

            [JsonProperty("acf_surname")]
            public string AcfSurname;

            [JsonProperty("discord_user")] //Sellixdoubleegrated
            public string DiscordUser;

            [JsonProperty("discord_id")] //Sellixdoubleegrated
            public string DiscordId;
        }

        public class Data
        {
            [JsonProperty("id")]
            public double? Id;

            [JsonProperty("uniqid")]
            public string Uniqid;

            [JsonProperty("recurring_billing_id")]
            public object RecurringBillingId;

            [JsonProperty("type")]
            public string Type;

            [JsonProperty("subtype")]
            public object Subtype;

            [JsonProperty("total")]
            public double? Total;

            [JsonProperty("total_display")]
            public double? TotalDisplay;

            [JsonProperty("product_variants")]
            public ProductVariants ProductVariants;

            [JsonProperty("exchange_rate")]
            public double? ExchangeRate;

            [JsonProperty("crypto_exchange_rate")]
            public double? CryptoExchangeRate;

            [JsonProperty("currency")]
            public string Currency;

            [JsonProperty("shop_id")]
            public double? ShopId;

            [JsonProperty("shop_image_name")]
            public string ShopImageName;

            [JsonProperty("shop_image_storage")]
            public string ShopImageStorage;

            [JsonProperty("cloudflare_image_id")]
            public string CloudflareImageId;

            [JsonProperty("name")]
            public string Name;

            [JsonProperty("customer_email")]
            public string CustomerEmail;

            [JsonProperty("affiliate_revenue_customer_id")]
            public object AffiliateRevenueCustomerId;

            [JsonProperty("paypal_email_delivery")]
            public bool? PaypalEmailDelivery;

            [JsonProperty("product_id")]
            public string ProductId;

            [JsonProperty("product_title")]
            public string ProductTitle;

            [JsonProperty("product_type")]
            public string ProductType;

            [JsonProperty("subscription_id")]
            public object SubscriptionId;

            [JsonProperty("subscription_time")]
            public object SubscriptionTime;

            [JsonProperty("gateway")]
            public string Gateway;

            [JsonProperty("blockchain")]
            public object Blockchain;

            [JsonProperty("paypal_apm")]
            public object PaypalApm;

            [JsonProperty("stripe_apm")]
            public object StripeApm;

            [JsonProperty("paypal_email")]
            public object PaypalEmail;

            [JsonProperty("paypal_order_id")]
            public object PaypalOrderId;

            [JsonProperty("paypal_payer_email")]
            public object PaypalPayerEmail;

            [JsonProperty("paypal_fee")]
            public double? PaypalFee;

            [JsonProperty("paypal_subscription_id")]
            public object PaypalSubscriptionId;

            [JsonProperty("paypal_subscription_link")]
            public object PaypalSubscriptionLink;

            [JsonProperty("lex_order_id")]
            public object LexOrderId;

            [JsonProperty("lex_payment_method")]
            public object LexPaymentMethod;

            [JsonProperty("paydash_paymentID")]
            public object PaydashPaymentID;

            [JsonProperty("virtual_payments_id")]
            public object VirtualPaymentsId;

            [JsonProperty("stripe_client_secret")]
            public string StripeClientSecret;

            [JsonProperty("stripe_price_id")]
            public object StripePriceId;

            [JsonProperty("skrill_email")]
            public object SkrillEmail;

            [JsonProperty("skrill_sid")]
            public object SkrillSid;

            [JsonProperty("skrill_link")]
            public object SkrillLink;

            [JsonProperty("perfectmoney_id")]
            public object PerfectmoneyId;

            [JsonProperty("binance_invoice_id")]
            public object BinanceInvoiceId;

            [JsonProperty("binance_qrcode")]
            public object BinanceQrcode;

            [JsonProperty("binance_checkout_url")]
            public object BinanceCheckoutUrl;

            [JsonProperty("crypto_address")]
            public object CryptoAddress;

            [JsonProperty("crypto_amount")]
            public double? CryptoAmount;

            [JsonProperty("crypto_received")]
            public double? CryptoReceived;

            [JsonProperty("crypto_uri")]
            public object CryptoUri;

            [JsonProperty("crypto_confirmations_needed")]
            public double? CryptoConfirmationsNeeded;

            [JsonProperty("crypto_scheduled_payout")]
            public bool? CryptoScheduledPayout;

            [JsonProperty("crypto_payout")]
            public double? CryptoPayout;

            [JsonProperty("fee_billed")]
            public bool? FeeBilled;

            [JsonProperty("bill_info")]
            public object BillInfo;

            [JsonProperty("cashapp_qrcode")]
            public object CashappQrcode;

            [JsonProperty("cashapp_note")]
            public object CashappNote;

            [JsonProperty("cashapp_cashtag")]
            public object CashappCashtag;

            [JsonProperty("country")]
            public string Country;

            [JsonProperty("location")]
            public string Location;

            [JsonProperty("ip")]
            public string Ip;

            [JsonProperty("is_vpn_or_proxy")]
            public bool? IsVpnOrProxy;

            [JsonProperty("user_agent")]
            public string UserAgent;

            [JsonProperty("quantity")]
            public double? Quantity;

            [JsonProperty("coupon_id")]
            public object CouponId;

            [JsonProperty("custom_fields")]
            public CustomFields CustomFields;

            [JsonProperty("developer_invoice")]
            public bool? DeveloperInvoice;

            [JsonProperty("developer_title")]
            public object DeveloperTitle;

            [JsonProperty("developer_webhook")]
            public object DeveloperWebhook;

            [JsonProperty("developer_return_url")]
            public object DeveloperReturnUrl;

            [JsonProperty("status")]
            public string Status;

            [JsonProperty("status_details")]
            public object StatusDetails;

            [JsonProperty("void_details")]
            public object VoidDetails;

            [JsonProperty("discount")]
            public double? Discount;

            [JsonProperty("fee_percentage")]
            public double? FeePercentage;

            [JsonProperty("fee_breakdown")]
            public string FeeBreakdown;

            [JsonProperty("day_value")]
            public double? DayValue;

            [JsonProperty("day")]
            public string Day;

            [JsonProperty("month")]
            public string Month;

            [JsonProperty("year")]
            public double? Year;

            [JsonProperty("product_addons")]
            public object ProductAddons;

            [JsonProperty("created_at")]
            public double? CreatedAt;

            [JsonProperty("updated_at")]
            public double? UpdatedAt;

            [JsonProperty("updated_by")]
            public double? UpdatedBy;

            [JsonProperty("ip_info")]
            public IpInfo IpInfo;

            [JsonProperty("service_text")]
            public string ServiceText;

            [JsonProperty("webhooks")]
            public List<Webhook> Webhooks;

            [JsonProperty("paypal_dispute")]
            public object PaypalDispute;

            [JsonProperty("product_downloads")]
            public List<object> ProductDownloads;

            [JsonProperty("status_history")]
            public List<StatusHistory> StatusHistory;

            [JsonProperty("crypto_transactions")]
            public List<object> CryptoTransactions;

            [JsonProperty("stripe_user_id")]
            public string StripeUserId;

            [JsonProperty("stripe_publishable_key")]
            public string StripePublishableKey;

            [JsonProperty("products")]
            public List<object> Products;

            [JsonProperty("gateways_available")]
            public List<string> GatewaysAvailable;

            [JsonProperty("country_regulations")]
            public string CountryRegulations;

            [JsonProperty("available_stripe_apm")]
            public List<AvailableStripeApm> AvailableStripeApm;

            [JsonProperty("shop_paypal_credit_card")]
            public bool? ShopPaypalCreditCard;

            [JsonProperty("shop_force_paypal_email_delivery")]
            public bool? ShopForcePaypalEmailDelivery;
        }

        public class IpInfo
        {
            [JsonProperty("success")]
            public bool? Success;

            [JsonProperty("message")]
            public string Message;

            [JsonProperty("fraud_score")]
            public double? FraudScore;

            [JsonProperty("country_code")]
            public string CountryCode;

            [JsonProperty("region")]
            public string Region;

            [JsonProperty("city")]
            public string City;

            [JsonProperty("ISP")]
            public string ISP;

            [JsonProperty("ASN")]
            public double? ASN;

            [JsonProperty("operating_system")]
            public string OperatingSystem;

            [JsonProperty("browser")]
            public string Browser;

            [JsonProperty("organization")]
            public string Organization;

            [JsonProperty("is_crawler")]
            public bool? IsCrawler;

            [JsonProperty("timezone")]
            public string Timezone;

            [JsonProperty("mobile")]
            public bool? Mobile;

            [JsonProperty("host")]
            public string Host;

            [JsonProperty("proxy")]
            public bool? Proxy;

            [JsonProperty("vpn")]
            public bool? Vpn;

            [JsonProperty("tor")]
            public bool? Tor;

            [JsonProperty("active_vpn")]
            public bool? ActiveVpn;

            [JsonProperty("active_tor")]
            public bool? ActiveTor;

            [JsonProperty("device_brand")]
            public string DeviceBrand;

            [JsonProperty("device_model")]
            public string DeviceModel;

            [JsonProperty("recent_abuse")]
            public bool? RecentAbuse;

            [JsonProperty("bot_status")]
            public bool? BotStatus;

            [JsonProperty("connection_type")]
            public string ConnectionType;

            [JsonProperty("abuse_velocity")]
            public string AbuseVelocity;

            [JsonProperty("zip_code")]
            public string ZipCode;

            [JsonProperty("latitude")]
            public double? Latitude;

            [JsonProperty("longitude")]
            public double? Longitude;

            [JsonProperty("request_id")]
            public string RequestId;

            [JsonProperty("transaction_details")]
            public object TransactionDetails;

            [JsonProperty("asn")]
            public double? Asn;

            [JsonProperty("isp")]
            public string Isp;
        }

        public class ProductVariants
        {
            [JsonProperty("63dbdb737bfbb")]
            public Product_Variants product_Variants;
        }

        public class Root
        {
            [JsonProperty("event")]
            public string Event;

            [JsonProperty("data")]
            public Data Data;
        }

        public class StatusHistory
        {
            [JsonProperty("id")]
            public double? Id;

            [JsonProperty("invoice_id")]
            public string InvoiceId;

            [JsonProperty("status")]
            public string Status;

            [JsonProperty("details")]
            public string Details;

            [JsonProperty("created_at")]
            public double? CreatedAt;
        }

        public class Webhook
        {
            [JsonProperty("uniqid")] public string Uniqid;

            [JsonProperty("url")] public string Url;

            [JsonProperty("event")] public string Event;

            [JsonProperty("retries")] public double? Retries;

            [JsonProperty("response_code")] public double? ResponseCode;

            [JsonProperty("created_at")] public double? CreatedAt;
        }
    }

    public class SellixPayloadNormal
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class AvailableStripeApm
        {
            [JsonProperty("id")]
            public string Id;

            [JsonProperty("name")]
            public string Name;
        }

        public class CustomFields
        {
            [JsonProperty("Hwid")]
            public string HWID;

            [JsonProperty("name")]
            public string Name;

            [JsonProperty("surname")]
            public string Surname;

            [JsonProperty("acf_name")]
            public string AcfName;

            [JsonProperty("acf_surname")]
            public string AcfSurname;

            [JsonProperty("discord_user")]
            public string DiscordUser;

            [JsonProperty("discord_id")]
            public string DiscordId;
        }

        public class Data
        {
            [JsonProperty("id")]
            public double? Id;

            [JsonProperty("uniqid")]
            public string Uniqid;

            [JsonProperty("recurring_billing_id")]
            public object RecurringBillingId;

            [JsonProperty("type")]
            public string Type;

            [JsonProperty("subtype")]
            public object Subtype;

            [JsonProperty("total")]
            public double? Total;

            [JsonProperty("total_display")]
            public double? TotalDisplay;

            [JsonProperty("product_variants")]
            public object ProductVariants;

            [JsonProperty("exchange_rate")]
            public double? ExchangeRate;

            [JsonProperty("crypto_exchange_rate")]
            public double? CryptoExchangeRate;

            [JsonProperty("currency")]
            public string Currency;

            [JsonProperty("shop_id")]
            public double? ShopId;

            [JsonProperty("shop_image_name")]
            public string ShopImageName;

            [JsonProperty("shop_image_storage")]
            public string ShopImageStorage;

            [JsonProperty("cloudflare_image_id")]
            public string CloudflareImageId;

            [JsonProperty("name")]
            public string Name;

            [JsonProperty("customer_email")]
            public string CustomerEmail;

            [JsonProperty("affiliate_revenue_customer_id")]
            public object AffiliateRevenueCustomerId;

            [JsonProperty("paypal_email_delivery")]
            public bool? PaypalEmailDelivery;

            [JsonProperty("product_id")]
            public string ProductId;

            [JsonProperty("product_title")]
            public string ProductTitle;

            [JsonProperty("product_type")]
            public string ProductType;

            [JsonProperty("subscription_id")]
            public object SubscriptionId;

            [JsonProperty("subscription_time")]
            public object SubscriptionTime;

            [JsonProperty("gateway")]
            public string Gateway;

            [JsonProperty("blockchain")]
            public object Blockchain;

            [JsonProperty("paypal_apm")]
            public object PaypalApm;

            [JsonProperty("stripe_apm")]
            public object StripeApm;

            [JsonProperty("paypal_email")]
            public object PaypalEmail;

            [JsonProperty("paypal_order_id")]
            public object PaypalOrderId;

            [JsonProperty("paypal_payer_email")]
            public object PaypalPayerEmail;

            [JsonProperty("paypal_fee")]
            public double? PaypalFee;

            [JsonProperty("paypal_subscription_id")]
            public object PaypalSubscriptionId;

            [JsonProperty("paypal_subscription_link")]
            public object PaypalSubscriptionLink;

            [JsonProperty("lex_order_id")]
            public object LexOrderId;

            [JsonProperty("lex_payment_method")]
            public object LexPaymentMethod;

            [JsonProperty("paydash_paymentID")]
            public object PaydashPaymentID;

            [JsonProperty("virtual_payments_id")]
            public object VirtualPaymentsId;

            [JsonProperty("stripe_client_secret")]
            public string StripeClientSecret;

            [JsonProperty("stripe_price_id")]
            public object StripePriceId;

            [JsonProperty("skrill_email")]
            public object SkrillEmail;

            [JsonProperty("skrill_sid")]
            public object SkrillSid;

            [JsonProperty("skrill_link")]
            public object SkrillLink;

            [JsonProperty("perfectmoney_id")]
            public object PerfectmoneyId;

            [JsonProperty("binance_invoice_id")]
            public object BinanceInvoiceId;

            [JsonProperty("binance_qrcode")]
            public object BinanceQrcode;

            [JsonProperty("binance_checkout_url")]
            public object BinanceCheckoutUrl;

            [JsonProperty("crypto_address")]
            public object CryptoAddress;

            [JsonProperty("crypto_amount")]
            public double? CryptoAmount;

            [JsonProperty("crypto_received")]
            public double? CryptoReceived;

            [JsonProperty("crypto_uri")]
            public object CryptoUri;

            [JsonProperty("crypto_confirmations_needed")]
            public double? CryptoConfirmationsNeeded;

            [JsonProperty("crypto_scheduled_payout")]
            public bool? CryptoScheduledPayout;

            [JsonProperty("crypto_payout")]
            public bool? CryptoPayout;

            [JsonProperty("fee_billed")]
            public bool? FeeBilled;

            [JsonProperty("bill_info")]
            public object BillInfo;

            [JsonProperty("cashapp_qrcode")]
            public object CashappQrcode;

            [JsonProperty("cashapp_note")]
            public object CashappNote;

            [JsonProperty("cashapp_cashtag")]
            public object CashappCashtag;

            [JsonProperty("country")]
            public string Country;

            [JsonProperty("location")]
            public string Location;

            [JsonProperty("ip")]
            public string Ip;

            [JsonProperty("is_vpn_or_proxy")]
            public bool? IsVpnOrProxy;

            [JsonProperty("user_agent")]
            public string UserAgent;

            [JsonProperty("quantity")]
            public double? Quantity;

            [JsonProperty("coupon_id")]
            public object CouponId;

            [JsonProperty("coupon_code")]
            public string CouponCode;

            [JsonProperty("custom_fields")]
            public CustomFields CustomFields;

            [JsonProperty("developer_invoice")]
            public bool? DeveloperInvoice;

            [JsonProperty("developer_title")]
            public object DeveloperTitle;

            [JsonProperty("developer_webhook")]
            public object DeveloperWebhook;

            [JsonProperty("developer_return_url")]
            public object DeveloperReturnUrl;

            [JsonProperty("status")]
            public string Status;

            [JsonProperty("status_details")]
            public object StatusDetails;

            [JsonProperty("void_details")]
            public object VoidDetails;

            [JsonProperty("discount")]
            public double? Discount;

            [JsonProperty("fee_percentage")]
            public double? FeePercentage;

            [JsonProperty("fee_breakdown")]
            public string FeeBreakdown;

            [JsonProperty("day_value")]
            public double? DayValue;

            [JsonProperty("day")]
            public string Day;

            [JsonProperty("month")]
            public string Month;

            [JsonProperty("year")]
            public double? Year;

            [JsonProperty("product_addons")]
            public object ProductAddons;

            [JsonProperty("created_at")]
            public double? CreatedAt;

            [JsonProperty("updated_at")]
            public double? UpdatedAt;

            [JsonProperty("updated_by")]
            public double? UpdatedBy;

            [JsonProperty("ip_info")]
            public IpInfo IpInfo;

            [JsonProperty("service_text")]
            public string ServiceText;

            [JsonProperty("webhooks")]
            public List<Webhook> Webhooks;

            [JsonProperty("paypal_dispute")]
            public object PaypalDispute;

            [JsonProperty("product_downloads")]
            public List<object> ProductDownloads;

            [JsonProperty("status_history")]
            public List<StatusHistory> StatusHistory;

            [JsonProperty("crypto_transactions")]
            public List<object> CryptoTransactions;

            [JsonProperty("stripe_user_id")]
            public string StripeUserId;

            [JsonProperty("stripe_publishable_key")]
            public string StripePublishableKey;

            [JsonProperty("products")]
            public List<Products> Products;

            [JsonProperty("gateways_available")]
            public List<string> GatewaysAvailable;

            [JsonProperty("country_regulations")]
            public string CountryRegulations;

            [JsonProperty("available_stripe_apm")]
            public List<AvailableStripeApm> AvailableStripeApm;

            [JsonProperty("shop_paypal_credit_card")]
            public bool? ShopPaypalCreditCard;

            [JsonProperty("shop_force_paypal_email_delivery")]
            public bool? ShopForcePaypalEmailDelivery;
        }

        public class IpInfo
        {
            [JsonProperty("success")]
            public bool? Success;

            [JsonProperty("message")]
            public string Message;

            [JsonProperty("fraud_score")]
            public double? FraudScore;

            [JsonProperty("country_code")]
            public string CountryCode;

            [JsonProperty("region")]
            public string Region;

            [JsonProperty("city")]
            public string City;

            [JsonProperty("ISP")]
            public string ISP;

            [JsonProperty("ASN")]
            public double? ASN;

            [JsonProperty("operating_system")]
            public string OperatingSystem;

            [JsonProperty("browser")]
            public string Browser;

            [JsonProperty("organization")]
            public string Organization;

            [JsonProperty("is_crawler")]
            public bool? IsCrawler;

            [JsonProperty("timezone")]
            public string Timezone;

            [JsonProperty("mobile")]
            public bool? Mobile;

            [JsonProperty("host")]
            public string Host;

            [JsonProperty("proxy")]
            public bool? Proxy;

            [JsonProperty("vpn")]
            public bool? Vpn;

            [JsonProperty("tor")]
            public bool? Tor;

            [JsonProperty("active_vpn")]
            public bool? ActiveVpn;

            [JsonProperty("active_tor")]
            public bool? ActiveTor;

            [JsonProperty("device_brand")]
            public string DeviceBrand;

            [JsonProperty("device_model")]
            public string DeviceModel;

            [JsonProperty("recent_abuse")]
            public bool? RecentAbuse;

            [JsonProperty("bot_status")]
            public bool? BotStatus;

            [JsonProperty("connection_type")]
            public string ConnectionType;

            [JsonProperty("abuse_velocity")]
            public string AbuseVelocity;

            [JsonProperty("zip_code")]
            public string ZipCode;

            [JsonProperty("latitude")]
            public double? Latitude;

            [JsonProperty("longitude")]
            public double? Longitude;

            [JsonProperty("request_id")]
            public string RequestId;

            [JsonProperty("transaction_details")]
            public object TransactionDetails;

            [JsonProperty("asn")]
            public double? Asn;

            [JsonProperty("isp")]
            public string Isp;
        }

        public class Root
        {
            [JsonProperty("event")]
            public string Event;

            [JsonProperty("data")]
            public Data Data;
        }

        public class StatusHistory
        {
            [JsonProperty("id")]
            public double? Id;

            [JsonProperty("invoice_id")]
            public string InvoiceId;

            [JsonProperty("status")]
            public string Status;

            [JsonProperty("details")]
            public string Details;

            [JsonProperty("created_at")]
            public double? CreatedAt;
        }

        public class Webhook
        {
            [JsonProperty("uniqid")]
            public string Uniqid;

            [JsonProperty("url")]
            public string Url;

            [JsonProperty("event")]
            public string Event;

            [JsonProperty("retries")]
            public double? Retries;

            [JsonProperty("response_code")]
            public double? ResponseCode;

            [JsonProperty("created_at")]
            public double? CreatedAt;
        }

        public class Products
        {
            [JsonProperty("id")]
            public double? Id;

            [JsonProperty("uniqid")]
            public string Uniqid;

            [JsonProperty("title")]
            public string Title;

            [JsonProperty("description")]
            public string Description;

            [JsonProperty("price_display")]
            public double? PriceDisplay;

            [JsonProperty("currency")]
            public string Currency;

            [JsonProperty("type")]
            public string Type;

            [JsonProperty("quantity_warning")]
            public double? QuantityWarning;

            [JsonProperty("stock")]
            public double? Stock;

            [JsonProperty("warranty")]
            public double? Warranty;

            [JsonProperty("gateways")]
            public string Gateways;

            [JsonProperty("block_vpn_proxies")]
            public double? BlockVpnProxies;

            [JsonProperty("max_risk_level")]
            public double? MaxRiskLevel;

            [JsonProperty("volume_discounts")]
            public string VolumeDiscounts;

            [JsonProperty("discord_integration")]
            public double? DiscordIntegration;

            [JsonProperty("discord_set_role")]
            public double? DiscordSetRole;

            [JsonProperty("discord_role_id")]
            public string DiscordRoleId;

            [JsonProperty("discord_server_id")]
            public string DiscordServerId;

            [JsonProperty("discord_remove_role")]
            public double? DiscordRemoveRole;

            [JsonProperty("discord_optional")]
            public double? DiscordOptional;

            [JsonProperty("unit_quantity")]
            public double? UnitQuantity;

            [JsonProperty("unit_price")]
            public string UnitPrice;

            [JsonProperty("unit_price_display")]
            public double? UnitPriceDisplay;

            [JsonProperty("volume_discount")]
            public string VolumeDiscount;

            [JsonProperty("volume_discount_display")]
            public string VolumeDiscountDisplay;

            [JsonProperty("affiliate_revenue_percent")]
            public string AffiliateRevenuePercent;

            [JsonProperty("status")]
            public string Status;

            [JsonProperty("image_name")]
            public string ImageName;

            [JsonProperty("image_storage")]
            public string ImageStorage;

            [JsonProperty("cloudflare_image_id")]
            public string CloudflareImageId;

            [JsonProperty("price_conversions")]
            public PriceConversions PriceConversions;
        }

        public class Crypto
        {
            [JsonProperty("BTC")]
            public string BTC;

            [JsonProperty("DOGE")]
            public string DOGE;

            [JsonProperty("BNB")]
            public string BNB;

            [JsonProperty("ETH")]
            public string ETH;

            [JsonProperty("LTC")]
            public string LTC;

            [JsonProperty("BCH")]
            public string BCH;

            [JsonProperty("NANO")]
            public string NANO;

            [JsonProperty("XMR")]
            public string XMR;

            [JsonProperty("SOL")]
            public string SOL;

            [JsonProperty("XRP")]
            public string XRP;

            [JsonProperty("CRO")]
            public string CRO;

            [JsonProperty("USDC")]
            public string USDC;

            [JsonProperty("USDC_NATIVE")]
            public string USDC_NATIVE;

            [JsonProperty("USDT")]
            public string USDT;

            [JsonProperty("TRX")]
            public string TRX;

            [JsonProperty("CCD")]
            public string CCD;

            [JsonProperty("MATIC")]
            public string MATIC;

            [JsonProperty("APE")]
            public string APE;

            [JsonProperty("PEPE")]
            public string PEPE;

            [JsonProperty("DAI")]
            public string DAI;

            [JsonProperty("WETH")]
            public string WETH;

            [JsonProperty("SHIB")]
            public string SHIB;
        }

        public class PriceConversions
        {
            [JsonProperty("CAD")]
            public double CAD;

            [JsonProperty("HKD")]
            public double HKD;

            [JsonProperty("ISK")]
            public double ISK;

            [JsonProperty("PHP")]
            public double PHP;

            [JsonProperty("DKK")]
            public double DKK;

            [JsonProperty("HUF")]
            public double HUF;

            [JsonProperty("CZK")]
            public double CZK;

            [JsonProperty("GBP")]
            public double GBP;

            [JsonProperty("RON")]
            public double RON;

            [JsonProperty("SEK")]
            public double SEK;

            [JsonProperty("IDR")]
            public double IDR;

            [JsonProperty("INR")]
            public double INR;

            [JsonProperty("BRL")]
            public double BRL;

            [JsonProperty("RUB")]
            public double RUB;

            [JsonProperty("HRK")]
            public double HRK;

            [JsonProperty("JPY")]
            public double JPY;

            [JsonProperty("THB")]
            public double THB;

            [JsonProperty("CHF")]
            public double CHF;

            [JsonProperty("EUR")]
            public double EUR;

            [JsonProperty("MYR")]
            public double MYR;

            [JsonProperty("BGN")]
            public double BGN;

            [JsonProperty("TRY")]
            public double TRY;

            [JsonProperty("CNY")]
            public double CNY;

            [JsonProperty("NOK")]
            public double NOK;

            [JsonProperty("NZD")]
            public double NZD;

            [JsonProperty("ZAR")]
            public double ZAR;

            [JsonProperty("USD")]
            public double USD;

            [JsonProperty("MXN")]
            public double MXN;

            [JsonProperty("SGD")]
            public double SGD;

            [JsonProperty("AUD")]
            public double AUD;

            [JsonProperty("ILS")]
            public double ILS;

            [JsonProperty("KRW")]
            public double KRW;

            [JsonProperty("PLN")]
            public double PLN;

            [JsonProperty("crypto")]
            public Crypto crypto;
        }
    }
}

#pragma warning restore CS8618

