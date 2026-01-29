using Microsoft.EntityFrameworkCore;
// Khai báo newton
using Newtonsoft.Json;
using Website.Models;
using Website.Areas.Admin.Controllers;
namespace Website.Models
{
    public class MyDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            // Lấy tag MyConnectring bên appsetting.json
            var ConnectionString = config.GetConnectionString("MyConectionString");
            optionsBuilder.UseSqlServer(ConnectionString);
        }
       public DbSet<Users> Users { get; set; }
       public DbSet<RowCategory> Categories{ get; set; }
       public DbSet<Products> Products { get; set; }
       public DbSet<CategoriesProduct> categoriesProducts { get; set; }
       public DbSet<News> News { get; set; }
       public DbSet<Adv> Advs { get; set; }
       public DbSet<Customers> Customers { get; set; }
       public DbSet<Order> Order { get; set; }
       public DbSet<OrderDetail> OrdersDetails { get; set; }
       public DbSet<Rating> Ratings { get; set; }
       public DbSet<Color> Color { get; set; }
       public DbSet<Size> Size { get; set; }
    }
}
