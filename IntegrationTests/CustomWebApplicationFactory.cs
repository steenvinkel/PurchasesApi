using DataAccess.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using AutoFixture;
using System.Linq;
using Microsoft.Extensions.Hosting;

namespace Purchases.IntegrationTests
{
    public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
    {
        public string AuthToken { get; private set; }
        public string SubCategoryName { get; private set; }
        public int SubCategoryId { get; private set; }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            // Prevent the real app from running its Run() call
            builder.UseEnvironment("Testing");
            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registration (if any)
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<PurchasesContext>));
                if (descriptor != null)
                    services.Remove(descriptor);
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
                    .GetRequiredService<ILogger<CustomWebApplicationFactory>>();

                // Ensure the database is created.
                db.Database.EnsureCreated();

                try
                {
                    // Seed the database with test data.
                    var fixture = new Fixture();
                    var user = fixture.Create<User>();
                    user.UserId = 1;
                    user.AuthExpire = DateTime.Now.AddDays(99);
                    AuthToken = user.AuthToken;
                    db.User.Add(user);

                    var category = fixture.Create<Category>();
                    category.CategoryId = 2;
                    category.UserId = user.UserId;
                    category.Type = "in";
                    db.Category.Add(category);

                    var category2 = fixture.Create<Category>();
                    category2.CategoryId = 3;
                    category2.UserId = user.UserId;
                    category2.Type = "out";
                    db.Category.Add(category2);

                    var subcategory = fixture.Create<SubCategory>();
                    subcategory.SubcategoryId = 4;
                    subcategory.CategoryId = category.CategoryId;
                    db.SubCategory.Add(subcategory);
                    SubCategoryName = subcategory.Name;
                    SubCategoryId = subcategory.SubcategoryId;

                    var subcategory2 = fixture.Create<SubCategory>();
                    subcategory2.SubcategoryId = 5;
                    subcategory2.CategoryId = category2.CategoryId;
                    subcategory2.Type = "variable";
                    db.SubCategory.Add(subcategory2);

                    var posting = fixture.Create<Posting>();
                    posting.PostingId = 6;
                    posting.Amount = 10000;
                    posting.SubcategoryId = subcategory.SubcategoryId;
                    posting.UserId = user.UserId;
                    db.Posting.Add(posting);

                    var posting2 = fixture.Create<Posting>();
                    posting2.PostingId = 7;
                    posting2.Amount = 10000;
                    posting2.SubcategoryId = subcategory.SubcategoryId;
                    posting2.UserId = user.UserId;
                    db.Posting.Add(posting2);

                    var accountType = fixture.Create<AccumulatedCategory>();
                    accountType.AccumulatedCategoryId = 8;
                    accountType.UserId = user.UserId;
                    db.AccumulatedCategory.Add(accountType);

                    var account = fixture.Create<Account>();
                    account.AccountId = 9;
                    account.AccumulatedCategoryId = accountType.AccumulatedCategoryId;
                    account.UserId = user.UserId;
                    db.Account.Add(account);

                    var accountStatus = fixture.Create<AccountStatus>();
                    accountStatus.AccountStatusId = 10;
                    accountStatus.AccountId = account.AccountId;
                    accountStatus.Date = posting.Date;
                    accountStatus.Amount = 10000;
                    accountStatus.UserId = user.UserId;
                    db.AccountStatus.Add(accountStatus);

                    var accountStatus2 = fixture.Create<AccountStatus>();
                    accountStatus2.AccountStatusId = 11;
                    accountStatus2.AccountId = account.AccountId;
                    accountStatus2.Date = posting2.Date;
                    accountStatus2.Amount = 10000;
                    accountStatus2.UserId = user.UserId;
                    db.AccountStatus.Add(accountStatus2);

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the database with test messages. Error: {Message}. StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                    throw;
                }
            });
        }
    }
}
