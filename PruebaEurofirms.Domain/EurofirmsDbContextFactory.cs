using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PruebaEurofirms.Domain
{
    /// <summary>
    /// Factory used for design time DbContext creation
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory&lt;PruebaEurofirms.Domain.EurofirmsDbContext&gt;" />
    public class EurofirmsDbContextFactory : IDesignTimeDbContextFactory<EurofirmsDbContext>
    {
        /// <summary>
        /// Creates a new instance of a derived context.
        /// </summary>
        /// <param name="args">Arguments provided by the design-time service.</param>
        /// <returns>
        /// An instance of <typeparamref name="TContext" />.
        /// </returns>
        public EurofirmsDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..\\PruebaEurofirms.Api"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("EurofirmsSqlite");

            var optionsBuilder = new DbContextOptionsBuilder<EurofirmsDbContext>();
            optionsBuilder.UseSqlite(connectionString);

            return new EurofirmsDbContext(optionsBuilder.Options);
        }
    }
}
