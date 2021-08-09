using INotificator.Context.Models;
using Microsoft.EntityFrameworkCore;

namespace INotificator.Context
{
    public sealed class ApplicationContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated(); 
        }
    }
}