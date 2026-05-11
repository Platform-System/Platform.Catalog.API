using Platform.Application.DependencyInjection;
using Platform.Api.Extensions;
using Platform.Catalog.API.Infrastructure.DependencyInjection;
using Platform.Catalog.API.Infrastructure.Data;
using Platform.Catalog.API.Presentation.Grpc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(typeof(Program).Assembly);
builder.Services.AddCatalogInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddPlatformAuthentication(builder.Configuration);
builder.Services.AddPlatformSwaggerJwt("Platform Catalog API");

var app = builder.Build();

app.ApplyMigrations<CatalogDbContext>();

app.UseHttpsRedirection();
app.UsePlatformSwagger();
app.UsePlatformAuthentication();

app.MapControllers();
app.MapGrpcService<CatalogIntegrationService>();

app.Run();
