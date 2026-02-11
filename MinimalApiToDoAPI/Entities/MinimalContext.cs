using Microsoft.EntityFrameworkCore;

namespace MinimalApiToDoAPI.Entities;

public partial class MinimalContext : DbContext
{
    public MinimalContext()
    {
    }

    public MinimalContext(DbContextOptions<MinimalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<User> Users { get; set; }

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Games__3214EC07968D3C28");
            entity.Property(e => e.Publisher).HasMaxLength(100);
            entity.Property(e => e.ReleaseDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07CD89447A");
            entity.Property(e => e.Password).HasMaxLength(500);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
