using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repository;

namespace KweezR
{
    public class ContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
    {
        public RepositoryContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<RepositoryContext>()
                .UseSqlServer(config.GetConnectionString("sqlConnection"), b => b.MigrationsAssembly("KweezR"));

            return new RepositoryContext(optionsBuilder.Options);
        }
    }
}
