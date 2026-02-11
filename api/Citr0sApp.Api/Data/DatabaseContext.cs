using Microsoft.EntityFrameworkCore;

namespace Citr0sApp.Api.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Directory.CreateDirectory("assets");
        optionsBuilder.UseSqlite("Data Source=assets/citr0s-app.db");
        
        var value = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (value == "Development")
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}