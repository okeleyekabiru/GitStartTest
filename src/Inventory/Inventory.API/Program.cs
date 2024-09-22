using GitStartFramework.Shared.Extensions;
using GitStartFramework.Shared.Middlewares;
using Inventory.API.Domain.Entities;
using Inventory.API.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration, typeof(InventoryItem).Assembly);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddRabbitMq(builder.Configuration);
builder.Services.AddLogging(builder.Configuration);
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Host.UseSerilog();
var app = builder.Build();
app.MigrateDatabase();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionMiddleware();
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();