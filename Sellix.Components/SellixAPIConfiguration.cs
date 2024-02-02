using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;


namespace Sellix.Components
{
    public static class SellixApiConfiguration
    {
        public static IServiceCollection SellixApi(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpClient("SellixAPI", config =>
            {
                config.BaseAddress = new Uri(configuration["SellixApi:ConnStr"] ?? string.Empty);
                config.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration["SellixApi:Token"]);
            });

            return service;
        }
    }
}