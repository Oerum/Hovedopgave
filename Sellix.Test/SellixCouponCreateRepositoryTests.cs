using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Sellix.Application.Interfaces;
using Xunit;
using DotNetEnv;
using DotNetEnv.Configuration;

namespace Sellix.Infrastructure.Tests
{
    public class SellixCouponCreateRepositoryTests
    {
        [Fact]
        public async Task CreateCoupon_Success()
        {
            // Arrange
            Env.Load();

            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddDotNetEnv()
                .Build();

            var loggerMock = new Mock<ILogger<SellixCouponCreateRepository>>();
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var sellixCouponCreateRepositoryMock = new Mock<ISellixCouponCreateRepository>();

            var httpClientHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpClientHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"data\":{\"coupons\":[{\"uniqid\":\"123\",\"code\":\"TestCoupon\"}]}}"),
                });

            var httpClient = new HttpClient(httpClientHandlerMock.Object)
            {
                BaseAddress = new Uri(configuration["SellixApi:ConnStr"] ?? "https://dev.sellix.io/v1/"),
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration["SellixApi:Token"]);


            httpClientFactoryMock.Setup(factory => factory.CreateClient("SellixAPI"))
                .Returns(httpClient);

            sellixCouponCreateRepositoryMock.Setup(repo => repo.CreateCoupon(It.IsAny<string>()))
                .ReturnsAsync("Code: TestCoupon\nDiscount: 10%\nExpires: " + DateTime.UtcNow.AddHours(1).ToString("MMM d, yyyy"));

            // Act
            var result = await sellixCouponCreateRepositoryMock.Object.CreateCoupon("TestCoupon");

            // Assert
            Assert.Equal("Code: TestCoupon\nDiscount: 10%\nExpires: " + DateTime.UtcNow.AddHours(1).ToString("MMM d, yyyy"), result);
        }
    }
}
