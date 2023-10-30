using DataAccess.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;

namespace Purchases.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
    {
        public string AuthToken { get; private set; }
        public string SubCategoryName { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                // Add a database context (ApplicationDbContext) using an in-memory 
                // database for testing.
                services.AddDbContext<PurchasesContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using var scope = sp.CreateScope();

                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<PurchasesContext>();
                var logger = scopedServices
                    .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                // Ensure the database is created.
                db.Database.EnsureCreated();

                try
                {
                    // Seed the database with test data.
                    var fixture = new Fixture();
                    var user = fixture.Create<User>();
                    user.AuthExpire = DateTime.Now.AddDays(99);
                    AuthToken = user.AuthToken;
                    db.User.Add(user);

                    var category = fixture.Create<Category>();
                    category.UserId = user.UserId;
                    category.Type = "in";
                    db.Category.Add(category);

                    var subcategory = fixture.Create<Subcategory>();
                    subcategory.CategoryId = category.CategoryId;
                    db.Subcategory.Add(subcategory);
                    SubCategoryName = subcategory.Name;

                    var posting = fixture.Create<Posting>();
                    posting.Amount = 10000;
                    posting.SubcategoryId = subcategory.SubcategoryId;
                    posting.UserId = user.UserId;
                    db.Posting.Add(posting);

                    var posting2 = fixture.Create<Posting>();
                    posting2.Amount = 10000;
                    posting2.SubcategoryId = subcategory.SubcategoryId;
                    posting2.UserId = user.UserId;
                    db.Posting.Add(posting2);

                    var accountStatus = fixture.Create<AccountStatus>();
                    accountStatus.Date = posting.Date;
                    accountStatus.Amount = 10000;
                    accountStatus.UserId = user.UserId;
                    db.AccountStatus.Add(accountStatus);

                    var accountStatus2 = fixture.Create<AccountStatus>();
                    accountStatus2.Date = posting2.Date;
                    accountStatus2.Amount = 10000;
                    accountStatus2.UserId = user.UserId;
                    db.AccountStatus.Add(accountStatus2);

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the database with test messages. Error: {Message}", ex.Message);
                }
            });
        }
    }
}
