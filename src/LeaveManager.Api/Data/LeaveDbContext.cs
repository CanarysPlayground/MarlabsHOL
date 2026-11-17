using LeaveManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManager.Api.Data;

public class LeaveDbContext(DbContextOptions<LeaveDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<Holiday> Holidays => Set<Holiday>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Email)
            .IsUnique();

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Manager)
            .WithMany()
            .HasForeignKey(e => e.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LeaveBalance>()
            .HasIndex(b => new { b.EmployeeId, b.LeaveType })
            .IsUnique();

        modelBuilder.Entity<LeaveRequest>()
            .Property(r => r.Status)
            .HasConversion<string>();

        modelBuilder.Entity<LeaveRequest>()
            .Property(r => r.LeaveType)
            .HasConversion<string>();

        modelBuilder.Entity<LeaveBalance>()
            .Property(b => b.LeaveType)
            .HasConversion<string>();
    }
}
