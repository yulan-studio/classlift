
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;
using Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;





namespace Core.Contexts
{


    //public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int> //DbContext
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int,
        IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    // Explicitly define the primary key for Identity tables
        //    builder.Entity<IdentityUserLogin<int>>().HasKey(x => new { x.LoginProvider, x.ProviderKey });
        //    builder.Entity<IdentityUserRole<int>>().HasKey(x => new { x.UserId, x.RoleId });
        //    builder.Entity<IdentityUserToken<int>>().HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
        //}


        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Coach> Coaches { get; set; }

        
        public DbSet<Child> Children { get; set; }

       

        public DbSet<EmergencyContact> EmergencyContacts { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>()
        //        .HasOne(a => a.Admin)
        //        .WithOne(a => a.User)
        //        .HasForeignKey<Admin>(a => a.UserId);

        //    modelBuilder.Entity<User>()
        //        .HasOne(s => s.Staff)
        //        .WithOne(s => s.User)
        //        .HasForeignKey<Staff>(s => s.UserId);

        //    modelBuilder.Entity<User>()
        //        .HasOne(c => c.Coach)
        //        .WithOne(c => c.User)
        //        .HasForeignKey<Coach>(c => c.UserId);

        //    modelBuilder.Entity<User>()
        //        .HasOne(ch => ch.Child)
        //        .WithOne(ch => ch.User)
        //        .HasForeignKey<Child>(ch => ch.UserId);
        //}
        //}





        public DbSet<Course> Courses { get; set; }
        public DbSet<Core.Models.Activity> Activities { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<ParentChild> ParentChild { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ActivityNotification> ActivityNotifications { get; set; }
        public DbSet<CourseNotification> CourseNotifications { get; set; }
        public DbSet<Coach> CoacheIncomes { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
        public DbSet<ActivityEnrollment> ActivityEnrollments { get; set; }
        public DbSet<PaymentPackage> PaymentPackages { get; set; }

        public DbSet<Specialty> Specialties { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<CoachSpecialty> CoachSpecialties { get; set; }

        public DbSet<CoachIncome> CoachIncomes { get; set; }

        public DbSet<ChildBalance> ChildBalances { get; set; }

        public DbSet<Fee> Fees { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
        .ToTable("users"); // Explicitly map to the table name


            modelBuilder.Entity<IdentityRole<int>>().ToTable("roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("userroles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("userclaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("userlogins");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("roleclaims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("usertokens");
            // Explicitly define the primary key for Identity tables
            modelBuilder.Entity<IdentityUserLogin<int>>().HasKey(x => new { x.LoginProvider, x.ProviderKey });
            modelBuilder.Entity<IdentityUserRole<int>>().HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<IdentityUserToken<int>>().HasKey(x => new { x.UserId, x.LoginProvider, x.Name });

            //modelBuilder.Entity<ApplicationUser>()
            //    .ToTable("users"); // Explicitly map to "users" table

            //modelBuilder.Entity<ApplicationUser>()
            //    .HasKey(u => u.Id); // Set primary key

            // Ensure relationships between User and your custom tables
            modelBuilder.Entity<Admin>().HasOne(a => a.User).WithOne().HasForeignKey<Admin>(a => a.UserID);
            modelBuilder.Entity<Staff>().HasOne(s => s.User).WithOne().HasForeignKey<Staff>(s => s.UserID);
            modelBuilder.Entity<Coach>().HasOne(c => c.User).WithOne().HasForeignKey<Coach>(c => c.UserID);
            modelBuilder.Entity<Child>().HasOne(c => c.User).WithOne().HasForeignKey<Child>(c => c.UserID);

            modelBuilder.Entity<Core.Models.Activity>()
            .ToTable("activities"); // Explicitly map to the table name

            modelBuilder.Entity<ActivityEnrollment>()
                .ToTable("activity_enrollments"); // Explicitly map to the table name


            modelBuilder.Entity<ActivityFeedback>()
           .ToTable("activity_feedback"); // Explicitly map to the table name

            modelBuilder.Entity<Admin>()
          .ToTable("admins"); // Explicitly map to the table name

            modelBuilder.Entity<Child>()
          .ToTable("children"); // Explicitly map to the table name

            modelBuilder.Entity<EmergencyContact>()
         .ToTable("emergency_contacts"); // Explicitly map to the table name

            modelBuilder.Entity<ChildBalance>()
          .ToTable("child_balance"); // Explicitly map to the table name

            modelBuilder.Entity<Fee>()
         .ToTable("fees"); // Explicitly map to the table name

            modelBuilder.Entity<Coach>()
          .ToTable("coaches"); // Explicitly map to the table name

            modelBuilder.Entity<CoachIncome>()
          .ToTable("coach_income"); // Explicitly map to the table name

            modelBuilder.Entity<Course>()
          .ToTable("courses"); // Explicitly map to the table name


            modelBuilder.Entity<CourseEnrollment>()
          .ToTable("course_enrollments"); // Explicitly map to the table name

            modelBuilder.Entity<CourseNotification>()
          .ToTable("course_notifications"); // Explicitly map to the table name

            modelBuilder.Entity<Parent>()
          .ToTable("parents"); // Explicitly map to the table name

            modelBuilder.Entity<ParentChild>()
         .ToTable("parent_child"); // Explicitly map to the table name

            modelBuilder.Entity<Fee>()
         .ToTable("fees"); // Explicitly map to the table name

            modelBuilder.Entity<Payment>()
         .ToTable("payments"); // Explicitly map to the table name

            modelBuilder.Entity<PaymentPackage>()
         .ToTable("payment_package"); // Explicitly map to the table name

            
            modelBuilder.Entity<Staff>()
         .ToTable("staff"); // Explicitly map to the table name

           

            modelBuilder.Entity<Specialty>()
        .ToTable("specialties"); // Explicitly map to the table name

            modelBuilder.Entity<City>()
        .ToTable("cities"); // Explicitly map to the table name

            modelBuilder.Entity<CoachSpecialty>()
        .ToTable("coach_specialty"); // Explicitly map to the table name

            modelBuilder.Entity<CoachSpecialty>()
        .HasKey(cs => new { cs.CoachID, cs.SpecialtyID }); // Composite Primary Key

            modelBuilder.Entity<CoachSpecialty>()
                .HasOne(cs => cs.Coach)
                .WithMany(c => c.CoachSpecialties)
                .HasForeignKey(cs => cs.CoachID);

           


            //modelBuilder.Entity<CoachSpecialty>()
            //.HasKey(cs => new { cs.CoachId, cs.SpecialtyId });

            //modelBuilder.Entity<CoachSpecialty>()
            //    .HasOne(cs => cs.Coach)
            //    .WithMany(c => c.CoachSpecialties)
            //    .HasForeignKey(cs => cs.CoachId);

            //modelBuilder.Entity<CoachSpecialty>()
            //    .HasOne(cs => cs.Specialty)
            //    .WithMany(s => s.CoachSpecialties)
            //    .HasForeignKey(cs => cs.SpecialtyId);
        }
    }

}
