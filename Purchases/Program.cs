using System;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Business.Repositories;
using Business.Services;
using DataAccess.Models;
using DataAccess.Repositories;
using Legacy.Dashboard;
using Legacy.Repositories;
using Legacy.Services;
using Purchases.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// ===== Services =====
builder.Services.AddMemoryCache();

// Controllers & JSON options
builder.Services.AddControllers()
    .AddJsonOptions(configure =>
    {
        configure.JsonSerializerOptions.NumberHandling =
            JsonNumberHandling.AllowNamedFloatingPointLiterals |
            JsonNumberHandling.AllowReadingFromString;
        configure.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Health checks
var connection = Environment.GetEnvironmentVariable("sql_connection") ?? string.Empty;
builder.Services.AddHealthChecks().AddMySql(connection);

// Database
builder.Services.AddDbContext<PurchasesContext>(options =>
    options.UseMySql(connection, new MySqlServerVersion(new Version(8, 0)))
);

// Dependency Injection
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountStatusRepository, AccountStatusRepository>();
builder.Services.AddScoped<IPostingRepository, PostingRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILegacyAccountStatusQueryRepository, LegacyMonthlyAccountStatusRepository>();
builder.Services.AddScoped<ILegacyPostingQueryRepository, LegacyPostingQueryRepository>();
builder.Services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
builder.Services.AddScoped<ILegacyLossService, LegacyLossService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// ===== Middleware =====
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();
app.UseMiddleware<CustomAuthenticationMiddleware>();

// ===== Endpoints =====
app.MapHealthChecks("/healthz");
app.MapControllers();

app.Run();

public partial class Program { }
