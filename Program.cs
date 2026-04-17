using Platform.Application.DependencyInjection;
using Platform.Api.Extensions;
using Platform.Catalog.API.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(typeof(Program).Assembly);
builder.Services.AddCatalogInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddPlatformAuthentication(builder.Configuration);
builder.Services.AddPlatformSwaggerJwt("Platform Catalog API");

var app = builder.Build();

app.UseHttpsRedirection();
app.UsePlatformSwagger();
app.UsePlatformAuthentication();

app.MapControllers();

app.Run();
