using Billing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Billing.Data;

public partial class ManagementDBContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Staff> Staff => Set<Staff>();

    
    public ManagementDBContext(DbContextOptions<ManagementDBContext> options)
        : base(options)
    {
    }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<User>().ToTable("users"); // Explicitly map to the table name

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("admins");
            entity.HasKey(admin => admin.AdminId);
            entity.Property(admin => admin.AdminId).HasColumnName("AdminID");
            entity.Property(admin => admin.UserId).HasColumnName("UserID");
            entity.Property(admin => admin.Name).HasMaxLength(255);
            entity.Property(admin => admin.Phone).HasMaxLength(50);
            entity.Property(admin => admin.Wechat).HasMaxLength(100);
            entity.HasIndex(admin => admin.UserId).IsUnique();
            entity.HasOne<User>()
                .WithOne()
                .HasForeignKey<Admin>(admin => admin.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.ToTable("staff");
            entity.HasKey(staff => staff.StaffId);
            entity.Property(staff => staff.StaffId).HasColumnName("StaffID");
            entity.Property(staff => staff.UserId).HasColumnName("UserID");
            entity.Property(staff => staff.Name).HasMaxLength(255);
            entity.Property(staff => staff.Phone).HasMaxLength(50);
            entity.Property(staff => staff.Wechat).HasMaxLength(100);
            entity.HasIndex(staff => staff.UserId).IsUnique();
            entity.HasOne<User>()
                .WithOne()
                .HasForeignKey<Staff>(staff => staff.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<IdentityRole<int>>().ToTable("roles");
        modelBuilder.Entity<IdentityUserRole<int>>().ToTable("userroles");
        modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("userclaims");
        modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("userlogins");
        modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("roleclaims");
        modelBuilder.Entity<IdentityUserToken<int>>().ToTable("usertokens");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
