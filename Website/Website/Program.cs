using Auth.Database.Contexts;
using Auth.Database.DbContextConfiguration;
using Crosscutting.TLS.Configuration;
using Crosscutting.TransactionHandling;
using LoggingService.Components.SerilogConfiguration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Globalization;
using Website.Components;

var builder = WebApplication.CreateBuilder(args);

//Docker
builder.Configuration.AddEnvironmentVariables();

//Logging
builder.Logging.AddLoggerConfig(builder.Configuration);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

//Dependency Injection
builder.Services.Scan(scan => scan
            .FromExecutingAssembly()
            .AddClasses()
            .AsMatchingInterface()
            .WithScopedLifetime());

//TLS
builder.WebHost.AddTLS(builder.Environment.IsDevelopment(), builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddLocalization();

builder.Services.TryAddScoped<IUnitOfWork<DiscordOAuthContext>, UnitOfWork<DiscordOAuthContext>>();
builder.Services.AddOAuthContext(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders();
    app.UseHttpsRedirection();
}

app.MapControllers();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseRequestLocalization(options =>
{
    var cultures = app.Configuration.GetSection("Cultures").GetChildren().ToDictionary(x => x.Key, x => x.Value);

    var supportedCultures = cultures.Keys.ToArray();

    options.AddSupportedCultures(supportedCultures);
    options.AddSupportedUICultures(supportedCultures);

    options.FallBackToParentCultures = true;
    options.FallBackToParentUICultures = true;
});

if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Current Culture: " + CultureInfo.CurrentCulture.Name);
    Console.WriteLine("Current UI Culture: " + CultureInfo.CurrentUICulture.Name);
}

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Website.Client._Imports).Assembly);

app.Run();
