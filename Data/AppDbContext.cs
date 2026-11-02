using BuildWorkflowConnect.Models;
using Microsoft.EntityFrameworkCore;

namespace BuildWorkflowConnect.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<EnvironmentEntity> Environments => Set<EnvironmentEntity>();
    public DbSet<EnvironmentVariable> EnvironmentVariables => Set<EnvironmentVariable>();
    public DbSet<ApiGroup> Groups => Set<ApiGroup>();
    public DbSet<ApiRequest> Requests => Set<ApiRequest>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<EnvironmentEntity>()
            .HasMany(e => e.Variables)
            .WithOne(v => v.Environment)
            .HasForeignKey(v => v.EnvironmentId)
            .OnDelete(DeleteBehavior.Cascade);
        b.Entity<ApiRequest>()
            .HasOne(r => r.Group)
            .WithMany(g => g.Requests)
            .HasForeignKey(r => r.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}