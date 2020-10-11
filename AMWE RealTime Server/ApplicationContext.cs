using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;

namespace AMWE_RealTime_Server
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string adminRoleName = "admin";
            string userRoleName = "user";

            Role adminRole = new Role { Id = 1, Name = adminRoleName };
            Role userRole = new Role { Id = 2, Name = userRoleName };

            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole });
            base.OnModelCreating(modelBuilder);
        }
    }
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }
        public Role()
        {
            Users = new List<User>();
        }
    }
    public class User
    {
        public int Id { get; set; }
        public string NameOfPC { get; set; }
        public string Password { get; set; }

        public int? RoleId { get; set; }
        public Role Role { get; set; }
    }
}