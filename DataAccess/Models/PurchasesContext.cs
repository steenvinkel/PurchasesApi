using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models
{
    public partial class PurchasesContext : DbContext
    {
        public PurchasesContext()
        {
        }

        public PurchasesContext(DbContextOptions<PurchasesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<AccountStatus> AccountStatus { get; set; }
        public virtual DbSet<AccumulatedCategory> AccumulatedCategory { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Posting> Posting { get; set; }
        public virtual DbSet<SubCategory> SubCategory { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("account");

                entity.Property(e => e.AccountId)
                    .HasColumnName("account_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AccumulatedCategoryId)
                    .HasColumnName("accumulated_category_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<AccountStatus>(entity =>
            {
                entity.ToTable("account_status");

                entity.Property(e => e.AccountStatusId)
                    .HasColumnName("account_status_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AccountId)
                    .HasColumnName("account_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("created_on")
                    .HasColumnType("datetime");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("date");

                entity.Property(e => e.UpdatedOn)
                    .HasColumnName("updated_on")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<AccumulatedCategory>(entity =>
            {
                entity.ToTable("accumulated_category");

                entity.Property(e => e.AccumulatedCategoryId)
                    .HasColumnName("accumulated_category_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasColumnName("color")
                    .HasColumnType("varchar(6)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category");

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("user_id");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasColumnName("color")
                    .HasColumnType("varchar(6)");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("created_on")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("text");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasColumnType("enum('in','out')")
                    .HasDefaultValueSql("'out'");

                entity.Property(e => e.UpdatedOn)
                    .HasColumnName("updated_on")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Posting>(entity =>
            {
                entity.ToTable("posting");

                entity.HasIndex(e => e.SubcategoryId)
                    .HasDatabaseName("subcategory_id");

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("user_id");

                entity.Property(e => e.PostingId)
                    .HasColumnName("posting_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Accuracy).HasColumnName("accuracy");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetime");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("date");

                entity.Property(e => e.Latitude).HasColumnName("latitude");

                entity.Property(e => e.Longitude).HasColumnName("longitude");

                entity.Property(e => e.SubcategoryId)
                    .HasColumnName("subcategory_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdatedOn)
                    .HasColumnName("updatedOn")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<SubCategory>(entity =>
            {
                entity.ToTable("subcategory");

                entity.HasIndex(e => e.CategoryId)
                    .HasDatabaseName("category_id");

                entity.HasIndex(e => new { e.Name, e.CategoryId })
                    .HasDatabaseName("name")
                    .IsUnique();

                entity.Property(e => e.SubcategoryId)
                    .HasColumnName("subcategory_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Color)
                    .HasColumnName("color")
                    .HasColumnType("varchar(6)");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("created_on")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.UpdatedOn)
                    .HasColumnName("updated_on")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasColumnType("smallint(6)");

                entity.Property(e => e.Admin)
                    .HasColumnName("admin")
                    .HasColumnType("smallint(6)");

                entity.Property(e => e.AuthExpire)
                    .HasColumnName("auth_expire")
                    .HasColumnType("datetime");

                entity.Property(e => e.AuthToken)
                    .IsRequired()
                    .HasColumnName("auth_token")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.JoinDate)
                    .HasColumnName("join_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.ObjectId)
                    .IsRequired()
                    .HasColumnName("object_id")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasColumnType("varchar(255)");
            });
        }
    }
}
