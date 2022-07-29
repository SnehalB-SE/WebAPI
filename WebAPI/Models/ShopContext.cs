using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    public class ShopContext : DbContext
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options) { }
        //On-model creation - run once model is created
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //here need to tell the system, how product and category are related to each other
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(a => a.Category)
                .HasForeignKey(a => a.CategoryId);

            modelBuilder.Seed();                     //modelBuilder.Seed is filling the database with sample data
        }
        //Need to set DbSets
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
