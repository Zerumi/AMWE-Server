// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;

using AMWE_RealTime_Server.Models;

using Microsoft.EntityFrameworkCore;

namespace AMWE_RealTime_Server
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Client> GlobalClientsList { get; set; }
        public DbSet<ClientState> GlobalClientStatesList { get; set; }
        public DbSet<ReportHubState> ReportHubState { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Role adminRole = new Role { Id = 1, Name = Role.GlobalAdminRole };
            Role userRole = new Role { Id = 2, Name = Role.GlobalUserRole };
            Role devRole = new Role { Id = 3, Name = Role.GlobalDeveloperRole };

            _ = modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole, devRole });

            _ = modelBuilder.Entity<ClientState>()
                .Property(e => e.LastLoginDateTime)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                );
            _ = modelBuilder.Entity<ClientState>()
                .Property(e => e.LastLogoutDateTime)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                    );

            _ = modelBuilder.Entity<ReportHubState>()
                    .HasKey(x => x.Id);

            _ = modelBuilder.Entity<ReportHubState>()
                .HasData(new ReportHubState
                {
                    Id = true,
                    WorkdayValue = false,
                    BaseRepInterval = TimeSpan.FromMinutes(1)
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}