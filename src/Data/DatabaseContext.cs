using Microsoft.EntityFrameworkCore;

namespace dotnet_jwt_refresh_token.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>(entity =>
        {
            entity.Property(user => user.Id).ValueGeneratedOnAdd();
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.Property(refreshToken => refreshToken.Id).ValueGeneratedOnAdd();
        });

        base.OnModelCreating(builder);
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
}