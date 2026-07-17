using Billing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Billing.Data;

public partial class ManagementDBContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    
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
