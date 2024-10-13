namespace TaskForge.DBContext;
using Microsoft.EntityFrameworkCore;
using TaskForge.Models;

public class TaskForgeDBContext : DbContext
{
    public TaskForgeDBContext(DbContextOptions<TaskForgeDBContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
            modelBuilder.Entity<Account>().ToTable("Account");      
    }
}
