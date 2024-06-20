using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Crosscutting.TLS.Configuration
{
    public static class TLSConfiguration
    {
        public static IWebHostBuilder AddTLS(this IWebHostBuilder builder, bool isDevelopment, IConfiguration configuration)
        {
            var certificate = new X509Certificate2(configuration["Cert:Gateway"]!, configuration["Cert:Gateway:Password"]) ?? throw new Exception("Certificate not found");

            //builder.Services.AddDataProtection().ProtectKeysWithCertificate(certificate);

            builder.UseKestrel(options =>
            {
                options.Listen(IPAddress.Any, 80, listenOptions =>
                {
                    listenOptions.UseConnectionLogging();
                });

                options.Listen(IPAddress.Any, 443, listenOptions =>
                {
                    if (!isDevelopment)
                    {
                        listenOptions.UseHttps(certificate);
                    }
                    listenOptions.UseConnectionLogging();
                });
            });

            return builder;
        }
    }
}
