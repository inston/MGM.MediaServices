using Microsoft.EntityFrameworkCore;
using MGM.MediaServices.Core.Models;

namespace MGM.MediaServices.Core.Data;

public class AppDbContext : DbContext
{
    public DbSet<Job> Jobs { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Job>()
            .HasKey(j => j.Id);

        modelBuilder.Entity<Job>()
            .Property(j => j.Metadata)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => string.IsNullOrEmpty(v) 
                    ? new Dictionary<string, string>() 
                    : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null)!);
    }
}