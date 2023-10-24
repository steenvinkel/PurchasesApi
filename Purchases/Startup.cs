using System;
using Business.Repositories;
using Business.Services;
using DataAccess.Models;
using DataAccess.Repositories;
using Legacy.Dashboard;
using Legacy.Repositories;
using Legacy.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using Purchases.Middleware;

namespace Purchases
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddMvc()
                .AddJsonOptions(configure => 
                        configure.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals | JsonNumberHandling.AllowReadingFromString
                    );

            var connection = Environment.GetEnvironmentVariable("sql_connection");
            services.AddDbContext<PurchasesContext>(options => options.UseMySql(connection, new MySqlServerVersion(new Version(8, 0))));
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAccountStatusRepository, AccountStatusRepository>();
            services.AddScoped<ILegacyPostingRepository, LegacyPostingRepository>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILegacyPostingSumupService, LegacyPostingSumupService>();
            services.AddScoped<ILegacyAccountStatusSumupService, LegacyAccountStatusSumupService>();
            services.AddScoped<ILegacySumupService, LegacySumupService>();
            services.AddScoped<ILegacyAccountStatusQueryRepository, LegacyMonthlyAccountStatusRepository>();
            services.AddScoped<ILegacyAccountStatusRepository, LegacyAccountStatusRepository>();
            services.AddScoped<ILegacyPostingQueryRepository, LegacyPostingQueryRepository>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<ILegacyLossRepository, LegacyLossRepository>();
            services.AddScoped<ILegacyLossService, LegacyLossService>();
            services.AddScoped<ILegacyPostingQueryService, LegacyPostingQueryService>();
            services.AddScoped<IDashboardService, DashboardService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseMiddleware<CustomAuthenticationMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "api/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
