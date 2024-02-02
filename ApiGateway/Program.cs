using System.Net;
using System.Security.Cryptography.X509Certificates;
using LoggingService.Components.SerilogConfiguration;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Docker
builder.Configuration.AddEnvironmentVariables();

builder.Logging.AddLoggerConfig(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();
builder.Services.AddHttpForwarder();

//YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHttpForwarder();

//TLS
if (!builder.Environment.IsDevelopment())
{
    var certificate = new X509Certificate2(builder.Configuration["Cert:Gateway"]!, builder.Configuration["Cert:Gateway:Password"]);

    //builder.Services.AddDataProtection().ProtectKeysWithCertificate(certificate);

    builder.WebHost.UseKestrel(options =>
    {
        options.Listen(IPAddress.Any, 80, listenOptions =>
        {
            listenOptions.UseConnectionLogging();
        });

        options.Listen(IPAddress.Any, 443, listenOptions =>
        {
            listenOptions.UseHttps(certificate);
            listenOptions.UseConnectionLogging();
        });
    });
}

var app = builder.Build();

app.MapHealthChecks("/health");

app.MapReverseProxy();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
    app.UseForwardedHeaders();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
