using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Contexts;
using Core.Models;



namespace Core.Contexts
{
    public class BillingDbContext : DbContext
    {
        public BillingDbContext(DbContextOptions<BillingDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TenantRegistry> TenantRegistries { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public virtual DbSet<Feature> Features { get; set; }
        public virtual DbSet<PlanFeature> PlanFeatures { get; set; }
        public virtual DbSet<OrganizationSubscription> OrganizationSubscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TenantRegistry>(entity =>
            {
                entity.ToTable("tenantregistry");

                entity.HasKey(e => e.TenantRegistryId);

                entity.Property(e => e.TenantRegistryId)
                    .HasColumnName("TenantRegistryId");

                entity.Property(e => e.OrganizationId)
                    .HasColumnName("OrganizationId");

                entity.Property(e => e.DatabaseName)
                    .HasColumnName("DatabaseName");

                entity.Property(e => e.Subdomain)
                    .HasColumnName("Subdomain");

                entity.Property(e => e.CustomDomain)
                    .HasColumnName("CustomDomain");

                entity.Property(e => e.IsActive)
                    .HasColumnName("IsActive");

                entity.HasOne(e => e.Organization)
                    .WithMany(e => e.TenantRegistries)
                    .HasForeignKey(e => e.OrganizationId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TenantRegistry_Organizations");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("organizations");
                entity.HasKey(e => e.OrganizationId);

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
                entity.Property(e => e.OrganizationName).HasColumnName("OrganizationName").HasMaxLength(200);
                entity.Property(e => e.ContactName).HasColumnName("ContactName").HasMaxLength(200);
                entity.Property(e => e.ContactEmail).HasColumnName("ContactEmail").HasMaxLength(200);
                entity.Property(e => e.ContactPhone).HasColumnName("ContactPhone").HasMaxLength(50);
                entity.Property(e => e.CurrentPlanId).HasColumnName("CurrentPlanID");
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");
                entity.Property(e => e.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(50);

                entity.HasIndex(e => e.IsActive).HasDatabaseName("IDX_Organizations_IsActive");

                entity.HasOne(e => e.CurrentPlan)
                    .WithMany(e => e.CurrentPlanOrganizations)
                    .HasForeignKey(e => e.CurrentPlanId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Organizations_Plans");
            });

            modelBuilder.Entity<SubscriptionPlan>(entity =>
            {
                entity.ToTable("subscriptionplans");
                entity.HasKey(e => e.PlanId);

                entity.Property(e => e.PlanId).HasColumnName("PlanID");
                entity.Property(e => e.PlanName).HasColumnName("PlanName").HasMaxLength(100);
                entity.Property(e => e.Description).HasColumnName("Description").HasColumnType("text");
                entity.Property(e => e.PricePerCoach).HasColumnName("PricePerCoach").HasPrecision(10, 2);
                entity.Property(e => e.MinimumMonthlyPrice).HasColumnName("MinimumMonthlyPrice").HasPrecision(10, 2).HasDefaultValue(0m);
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.PlanName).IsUnique().HasDatabaseName("PlanName");
                entity.HasIndex(e => e.IsActive).HasDatabaseName("IDX_SubscriptionPlans_IsActive");
            });

            modelBuilder.Entity<Feature>(entity =>
            {
                entity.ToTable("features");
                entity.HasKey(e => e.FeatureId);

                entity.Property(e => e.FeatureId).HasColumnName("FeatureID");
                entity.Property(e => e.FeatureKey).HasColumnName("FeatureKey").HasMaxLength(100);
                entity.Property(e => e.FeatureName).HasColumnName("FeatureName").HasMaxLength(200);
                entity.Property(e => e.Description).HasColumnName("Description").HasColumnType("text");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.FeatureKey).IsUnique().HasDatabaseName("FeatureKey");
            });

            modelBuilder.Entity<PlanFeature>(entity =>
            {
                entity.ToTable("planfeatures");
                entity.HasKey(e => e.PlanFeatureId);

                entity.Property(e => e.PlanFeatureId).HasColumnName("PlanFeatureID");
                entity.Property(e => e.PlanId).HasColumnName("PlanID");
                entity.Property(e => e.FeatureId).HasColumnName("FeatureID");
                entity.Property(e => e.IsLocked).HasColumnName("IsLocked").HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => new { e.PlanId, e.FeatureId })
                    .IsUnique()
                    .HasDatabaseName("UK_Plan_Feature");

                entity.HasOne(e => e.Plan)
                    .WithMany(e => e.PlanFeatures)
                    .HasForeignKey(e => e.PlanId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PlanFeatures_Plans");

                entity.HasOne(e => e.Feature)
                    .WithMany(e => e.PlanFeatures)
                    .HasForeignKey(e => e.FeatureId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PlanFeatures_Features");
            });

            modelBuilder.Entity<OrganizationSubscription>(entity =>
            {
                entity.ToTable("organization_subscriptions");
                entity.HasKey(e => e.OrganizationSubscriptionId);

                entity.Property(e => e.OrganizationSubscriptionId).HasColumnName("OrganizationSubscriptionID");
                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
                entity.Property(e => e.PlanId).HasColumnName("PlanID");
                entity.Property(e => e.StartDate).HasColumnName("StartDate").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.EndDate).HasColumnName("EndDate");
                entity.Property(e => e.Status).HasColumnName("Status").HasColumnType("enum('Active','Trial','Cancelled','Expired')");
                entity.Property(e => e.IsTrial).HasColumnName("IsTrial").HasDefaultValue(false);
                entity.Property(e => e.TrialStartDate).HasColumnName("TrialStartDate");
                entity.Property(e => e.TrialEndDate).HasColumnName("TrialEndDate");
                entity.Property(e => e.ActivatedAt).HasColumnName("ActivatedAt");
                entity.Property(e => e.CancelledAt).HasColumnName("CancelledAt");
                entity.Property(e => e.LastBilledDate).HasColumnName("LastBilledDate");
                entity.Property(e => e.AutoRenew).HasColumnName("AutoRenew").HasDefaultValue(true);
                entity.Property(e => e.MonthlyPricePerCoach).HasColumnName("MonthlyPricePerCoach").HasPrecision(10, 2);
                entity.Property(e => e.MinimumMonthlyPrice).HasColumnName("MinimumMonthlyPrice").HasPrecision(10, 2).HasDefaultValue(0m);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");
                entity.Property(e => e.OrganizationSubscriptionsColumn)
                    .HasColumnName("organization_subscriptionscol")
                    .HasMaxLength(45);

                entity.HasIndex(e => e.OrganizationId).HasDatabaseName("IDX_OrgSub_OrganizationID");
                entity.HasIndex(e => e.PlanId).HasDatabaseName("IDX_OrgSub_PlanID");
                entity.HasIndex(e => e.Status).HasDatabaseName("IDX_OrgSub_Status");

                entity.HasOne(e => e.Organization)
                    .WithMany(e => e.OrganizationSubscriptions)
                    .HasForeignKey(e => e.OrganizationId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_OrgSub_Organizations");

                entity.HasOne(e => e.Plan)
                    .WithMany(e => e.OrganizationSubscriptions)
                    .HasForeignKey(e => e.PlanId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_OrgSub_Plans");
            });
        }
    }
}
