using Microsoft.EntityFrameworkCore;

namespace Assignment3.Entities;

public class KanbanContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost:6521;Database=TEST;Username=psd;Password=1234");

}
