using Microsoft.EntityFrameworkCore;

namespace Assignment3.Entities;

public sealed class KanbanContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Task> Tasks => Set<Task>();
    public DbSet<Tag> Tags => Set<Tag>();


    public KanbanContext(DbContextOptions<KanbanContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TASK
        modelBuilder.Entity<Task>()
        .Property(e => e.state)
        .HasConversion(
                    v => v.ToString(),
                    v => (State)Enum.Parse(typeof(State), v));

        modelBuilder.Entity<Task>().HasMany(v => v.tags).WithMany(c => c.Tasks);

        // USER
        modelBuilder.Entity<User>().HasIndex(c => c.Email).IsUnique();


        // TAG
        modelBuilder.Entity<Tag>().HasIndex(c => c.Name).IsUnique();
        modelBuilder.Entity<Tag>().HasMany(v => v.Tasks).WithMany(c => c.tags);
    }


}
