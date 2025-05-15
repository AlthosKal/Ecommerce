using Microsoft.EntityFrameworkCore;
using LIBRARY.Shared.Entity;
namespace BACK_END.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<State> States{ get; set; }
        public DbSet<Category> Categories{ get; set; }
        public DbSet<Product> Products{ get; set; }
        public DbSet<ProductImage> ProductImages{ get; set; }
        public DbSet<ProdCategory> ProdCategories{ get; set; }
        public DbSet<User> Users{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<State>()
                .HasIndex(s => new { s.Name, s.CountryId })
                .IsUnique();

            modelBuilder.Entity<City>()
                .HasIndex(c => new { c.Name, c.StateId })
                .IsUnique();

            modelBuilder.Entity<ProdCategory>()
                .HasIndex(pc => new { pc.Name, pc.CategoryId })
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.Name, p.ProdCategoryId })
                .IsUnique();

            modelBuilder.Entity<ProductImage>()
                .HasIndex(pi => new { pi.Name, pi.ProductId })
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => new { u.Document, u.CityId })
                .IsUnique();
        }

    }
}
