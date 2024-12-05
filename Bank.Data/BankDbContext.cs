using Bank.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bank.Data;

public class BankDbContext(DbContextOptions<BankDbContext> options) : DbContext(options)
{
    public virtual DbSet<BankAccount> BankAccounts { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BankAccount>()
            .ToTable("BankAccount");
        
        modelBuilder.Entity<BankAccount>().HasKey(c => c.Id);
        
        base.OnModelCreating(modelBuilder);
    }
}