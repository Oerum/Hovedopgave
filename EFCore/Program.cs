using Auth.Database.DbContextConfiguration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

//builder.Logging.AddLoggerConfig(builder.Configuration);

builder.Services.AddAuthDbContext(builder.Configuration);
builder.Services.AddSagaDbContext(builder.Configuration);
builder.Services.AddOAuthContext(builder.Configuration);

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.Run();
