using Authentication.API.Domain.Entities;
using Authentication.API.Repository;
using Authentication.API.Services;
using GitStartFramework.Shared.Extensions;
using GitStartFramework.Shared.Middlewares;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddPersistence(builder.Configuration, typeof(User).Assembly);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddRabbitMq(builder.Configuration);
builder.Services.AddLogging(builder.Configuration);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ITokenService, TokenService>();
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