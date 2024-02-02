using DiscordSaga;
using DiscordSaga.Components.Events;
using DiscordSaga.Components.StateMachineInstance;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace xUnitTest.DiscordSaga
{
    public class UnitTest1
    {

        [Fact]
        public async Task Test1()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(cfg =>
                {
                    cfg.AddSagaStateMachine<DiscordStateMachine, SagaDiscord>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetRequiredService<ITestHarness>();

            await harness.Start();

            var sagaId = Guid.NewGuid();
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            var orderNumber = "{\"event\":\"order:paid\",\"data\":{\"id\":7879641,\"uniqid\":\"06711d-43f01f44c8-4ca4e4\",\"recurring_billing_id\":null,\"type\":\"PRODUCT\",\"subtype\":null,\"total\":1.36,\"total_display\":1.25,\"product_variants\":null,\"exchange_rate\":0.91,\"crypto_exchange_rate\":0,\"crypto_exchange_rate_fix\":\"0.000000000000\",\"currency\":\"EUR\",\"shop_id\":241980,\"shop_image_name\":\"f6df1dd66dc49693235999bb19767c27d0f555961d920a3c3f0a307cc58ff4bb.png\",\"shop_image_storage\":\"SHOPS\",\"shop_cloudflare_image_id\":\"e0f95ea3-a6cd-430c-1906-a7413a1b9900\",\"name\":\"BC\",\"customer_email\":\"pixila@gmail.com\",\"affiliate_revenue_customer_id\":null,\"paypal_email_delivery\":false,\"product_id\":\"63de88910dad3\",\"product_title\":\"AIOSubscriptionTime\",\"product_type\":\"SERVICE\",\"subscription_id\":null,\"subscription_time\":null,\"gateway\":\"STRIPE\",\"blockchain\":null,\"paypal_apm\":null,\"stripe_apm\":null,\"paypal_email\":null,\"paypal_order_id\":null,\"paypal_payer_email\":null,\"paypal_fee\":0,\"paypal_subscription_id\":null,\"paypal_subscription_link\":null,\"lex_order_id\":null,\"lex_payment_method\":null,\"paydash_paymentID\":null,\"virtual_payments_id\":null,\"stripe_client_secret\":\"pi_3NKUusFV0tK3hMgN01r3iNq2_secret_S2DVUJn7h48AphryJ7glVB6Me\",\"stripe_price_id\":null,\"skrill_email\":null,\"skrill_sid\":null,\"skrill_link\":null,\"perfectmoney_id\":null,\"binance_invoice_id\":null,\"binance_qrcode\":null,\"binance_checkout_url\":null,\"crypto_address\":null,\"crypto_amount\":0,\"crypto_received\":0,\"crypto_uri\":null,\"crypto_confirmations_needed\":0,\"crypto_scheduled_payout\":false,\"crypto_payout\":0,\"fee_billed\":true,\"bill_info\":null,\"cashapp_qrcode\":null,\"cashapp_note\":null,\"cashapp_cashtag\":null,\"country\":\"US\",\"location\":\"TomsRiver,NJ(America\\/New_York)\",\"ip\":\"73.195.195.89\",\"is_vpn_or_proxy\":false,\"user_agent\":\"Mozilla\\/5.0(WindowsNT10.0;Win64;x64)AppleWebKit\\/537.36(KHTML,likeGecko)Chrome\\/114.0.0.0Safari\\/537.36\",\"quantity\":1,\"coupon_id\":null,\"custom_fields\":{\"HWID\":\"1051-5C38-78A3-455C-24C2-14C6-8754-6B2B\",\"name\":\"Priscilla\",\"surname\":\"BarbinDynarski\",\"acf_name\":\"Priscilla\",\"acf_surname\":\"BarbinDynarski\",\"discord_user\":\"_bound_#0\",\"discord_id\":\"178146383948283904\"},\"developer_invoice\":false,\"developer_title\":null,\"developer_webhook\":null,\"developer_return_url\":null,\"status\":\"COMPLETED\",\"status_details\":null,\"void_details\":null,\"discount\":0,\"fee_percentage\":3,\"fee_breakdown\":\"{\\\"service_fee\\\":{\\\"amount\\\":0.04,\\\"currency\\\":\\\"USD\\\",\\\"breakdown\\\":{\\\"flat\\\":{\\\"amount\\\":0,\\\"currency\\\":\\\"USD\\\"},\\\"percentage\\\":{\\\"plan\\\":\\\"PRO\\\",\\\"value\\\":3,\\\"amount\\\":0.04,\\\"currency\\\":\\\"USD\\\"}}},\\\"aml_analysis\\\":{\\\"amount\\\":0,\\\"currency\\\":\\\"USD\\\",\\\"breakdown\\\":{\\\"wallet\\\":{\\\"amount\\\":0,\\\"currency\\\":\\\"USD\\\"},\\\"transaction\\\":{\\\"amount\\\":0,\\\"currency\\\":\\\"USD\\\"}}}}\",\"discount_breakdown\":{\"log\":{\"coupon\":{\"total\":1.09,\"coupon\":0,\"total_display\":1,\"coupon_display\":0},\"bundle_discount\":[],\"volume_discount\":{\"total\":1.09,\"total_display\":1,\"volume_discount\":0,\"volume_discount_display\":0}},\"tax\":{\"percentage\":\"25.00\"},\"addons\":[],\"coupon\":[],\"tax_log\":{\"vat\":\"25.00\",\"type\":\"EXCLUSIVE\",\"vat_total\":0.2725,\"total_pre_vat\":1.09,\"total_with_vat\":1.36,\"vat_percentage\":\"25.00\",\"vat_total_display\":0.25,\"total_pre_vat_display\":1,\"total_with_vat_display\":1.25},\"currencies\":{\"default\":\"USD\",\"display\":\"EUR\"},\"gateway_fee\":[],\"price_discount\":[],\"bundle_discounts\":[],\"volume_discounts\":{\"63de88910dad3\":{\"type\":\"FIXED\",\"amount\":0,\"percentage\":\"200\",\"amount_display\":0}}},\"day_value\":18,\"day\":\"Sun\",\"month\":\"Jun\",\"year\":2023,\"product_addons\":null,\"bundle_config\":null,\"created_at\":1687130514,\"updated_at\":1687130536,\"updated_by\":0,\"ip_info\":{\"id\":2480534,\"request_id\":\"6KSa038DDM\",\"ip\":\"73.195.195.89\",\"user_agent\":\"Mozilla\\/5.0(WindowsNT10.0;Win64;x64)AppleWebKit\\/537.36(KHTML,likeGecko)Chrome\\/104.0.0.0Safari\\/537.36\",\"user_language\":\"en-US,en;q=0.9,pt-BR;q=0.8,pt;q=0.7\",\"fraud_score\":0,\"country_code\":\"US\",\"region\":\"NJ\",\"city\":\"TomsRiver\",\"isp\":\"ComcastCable\",\"asn\":7922,\"organization\":\"ComcastCable\",\"latitude\":\"39.95000\",\"longitude\":\"-74.20000\",\"is_crawler\":0,\"timezone\":\"America\\/New_York\",\"mobile\":0,\"host\":\"c-73-195-195-89.hsd1.nj.comcast.net\",\"proxy\":0,\"vpn\":0,\"tor\":0,\"active_vpn\":0,\"active_tor\":0,\"recent_abuse\":0,\"bot_status\":0,\"connection_type\":\"Residential\",\"abuse_velocity\":\"none\",\"operating_system\":\"Windows10\",\"browser\":\"Chrome104.0\",\"device_brand\":\"N\\/A\",\"device_model\":\"N\\/A\",\"created_at\":1633125959,\"updated_at\":1660446613},\"service_text\":\"\",\"webhooks\":[{\"uniqid\":\"648f91a8cf5bf\",\"url\":\"https:\\/\\/www.oerumtechnologies.com\\/gateway\\/API\\/BC\\/BoundBot\\/GrantLicenseOrder\",\"event\":\"order:paid\",\"retries\":0,\"response_code\":0,\"created_at\":1687130536}],\"paypal_dispute\":null,\"product_downloads\":[],\"license\":false,\"status_history\":[{\"id\":23102204,\"invoice_id\":\"06711d-43f01f44c8-4ca4e4\",\"status\":\"PENDING\",\"details\":\"Theinvoicehasbeencreated(gatewaySTRIPE)andwearenowwaitingtoreceiveapayment.\",\"created_at\":1687130454},{\"id\":23102214,\"invoice_id\":\"06711d-43f01f44c8-4ca4e4\",\"status\":\"COMPLETED\",\"details\":\"Theinvoicehasbeenflaggedascompletedandtheproducthasbeenshipped.ReceivedStripewebhookpayment_intent.succeeded(evt_3NKUusFV0tK3hMgN0lCgSLDF)forpi_3NKUusFV0tK3hMgN01r3iNq2.\",\"created_at\":1687130598}],\"aml_wallet\":null,\"crypto_transactions\":[],\"stripe_user_id\":\"acct_1JwRbAFV0tK3hMgN\",\"stripe_publishable_key\":\"\",\"products\":[null],\"gateways_available\":[\"SHIB:ERC20\",\"USDC:MATIC\",\"SHIB:BEP20\",\"APE:ERC20\",\"WETH:ERC20\",\"DAI:MATIC\",\"DAI:ERC20\",\"DAI:BEP20\",\"PEPE:ERC20\",\"USDT:MATIC\",\"USDT:TRC20\",\"USDT:ERC20\",\"USDT:BEP20\",\"USDC:ERC20\",\"USDC:BEP20\",\"BITCOIN\",\"ETHEREUM\",\"LITECOIN\",\"BITCOIN_CASH\",\"TRON\",\"BINANCE_COIN\",\"POLYGON\",\"SOLANA\",\"MONERO\",\"NANO\",\"STRIPE\",\"BINANCE\"],\"country_regulations\":\"DK\",\"available_stripe_apm\":[{\"id\":\"bancontact\",\"name\":\"Bancontact\"},{\"id\":\"eps\",\"name\":\"EPS\"},{\"id\":\"giropay\",\"name\":\"Giropay\"},{\"id\":\"ideal\",\"name\":\"iDEAL\"},{\"id\":\"klarna\",\"name\":\"Klarna\"},{\"id\":\"p24\",\"name\":\"P24\"},{\"id\":\"sofort\",\"name\":\"Sofort\"}],\"shop_payment_gateways_fees\":[],\"shop_paypal_credit_card\":false,\"shop_force_paypal_email_delivery\":false,\"shop_walletconnect_id\":null}}";
#pragma warning restore CS0219 // Variable is assigned but its value is never used

            await harness.Bus.Publish(new LicenseGrantEvent()
            {
                CorrelationId = sagaId,
                Payload = orderNumber
            });

            var sagaHarness = harness.GetSagaStateMachineHarness<DiscordStateMachine, SagaDiscord>();

            Assert.True(await sagaHarness.Consumed.Any<LicenseGrantEvent>());


            var instance = sagaHarness.Created.ContainsInState(sagaId, sagaHarness.StateMachine, sagaHarness.StateMachine.NotificationReadyState);
            Assert.NotNull(instance);
            Assert.NotNull(instance);
            // Test side effects of OrderState here
        }
    }
}