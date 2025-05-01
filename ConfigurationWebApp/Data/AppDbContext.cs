using Microsoft.EntityFrameworkCore;
using ConfigurationWebApp.Models;

namespace ConfigurationWebApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ConfigurationItem> ConfigurationSettings { get; set; }
    }
}