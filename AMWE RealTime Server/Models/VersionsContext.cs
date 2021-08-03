//This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
//You can use & improve this code by keeping this comments
//(or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AMWE_RealTime_Server.Models
{
    public class VersionsContext : DbContext
    {
        public VersionsContext(DbContextOptions<VersionsContext> options) : base(options)
        {

        }

        public DbSet<Version> Versions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Version>(e =>
            {
                e.ToTable("Versions");
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}