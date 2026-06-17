using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Leave__Management_System.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole 
                { 
                    Id = "98b1bfb4-3b1c-4d31-9858-56324e082b6d",
                    ConcurrencyStamp = "a21ba486-bddf-4903-958c-513b756bdf5b",
                    Name = "Employee", 
                    NormalizedName = "EMPLOYEE" 
                },
                new IdentityRole 
                { 
                    Id = "b3c4b1d5-2a9c-4e01-8f67-4d321e987654",
                    ConcurrencyStamp = "1f5b2ec5-10d8-4978-833f-01dbd9e5a3eb",
                    Name = "Supervisor", 
                    NormalizedName = "SUPERVISOR" 
                },
                new IdentityRole 
                { 
                    Id = "c4d5c2e6-3b0d-4f12-9g78-5e432f109876",
                    ConcurrencyStamp = "d3bda9b9-4f73-44d0-b9e9-fd03dde884ea",
                    Name = "Administrator", 
                    NormalizedName = "ADMINISTRATOR" 
                });
            var hasher = new PasswordHasher<ApplicationUser>();
            builder.Entity<ApplicationUser>().HasData(new ApplicationUser
                {
                    Id = "a1b2c3d4-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                    AccessFailedCount = 0,
                    ConcurrencyStamp = "db1eaee3-82d5-4588-80e9-64540fc68818",
                    Email = "employee@example.com",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    LockoutEnd = null,
                    NormalizedEmail = "EMPLOYEE@EXAMPLE.COM",
                    NormalizedUserName = "EMPLOYEE",
                    PasswordHash = "AQAAAAIAAYagAAAAEAiJwz8DpkLC0eLYPgqbEOl/bMmzLY/qkcIdVn8F13QAj87leX5qsHZn3GgU8oGeJg==",
                    PhoneNumber = null,
                    PhoneNumberConfirmed = false,
                    SecurityStamp = "417c46fd-2fea-4209-9597-95ac13b5303a",
                    TwoFactorEnabled = false,
                    UserName = "employee",
                    FirstName = "Default",
                    LastName = "User",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Today)
                });

            builder.Entity<IdentityUserRole<string>>().HasData(
           new IdentityUserRole<string>
           {
               UserId = "a1b2c3d4-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
               RoleId = "98b1bfb4-3b1c-4d31-9858-56324e082b6d" // Employee role
           });

        }

       
        public DbSet<LeaveType> LeaveTypes { get; set; }
    }
}
