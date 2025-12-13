using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AMWE_RealTime_Server
{
    public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
    {
        public ApplicationContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<ApplicationContext> optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();

            _ = optionsBuilder.UseNpgsql("Host=localhost;Port=55432;Database=postgres;Username=postgres;Password=changeit");

            return new ApplicationContext(optionsBuilder.Options);
        }
    }
}