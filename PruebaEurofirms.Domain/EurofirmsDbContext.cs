using Microsoft.EntityFrameworkCore;
using PruebaEurofirms.Domain.Entities;

namespace PruebaEurofirms.Domain;

/// <summary>
///   The Db Context for the Eurofirms database.
/// </summary>
public class EurofirmsDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EurofirmsDbContext"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public EurofirmsDbContext(DbContextOptions<EurofirmsDbContext> options) : base(options) { }

    /// <summary>
    /// Gets the characters.
    /// </summary>
    /// <value>
    /// The characters.
    /// </value>
    public DbSet<Character> Characters => Set<Character>();

    /// <summary>
    /// Gets the episodes.
    /// </summary>
    /// <value>
    /// The episodes.
    /// </value>
    public DbSet<Episode> Episodes => Set<Episode>();

    /// <summary>
    /// Override this method to further configure the model that was discovered by convention from the entity types
    /// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
    /// and re-used for subsequent instances of your derived context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
    /// define extension methods on this object that allow you to configure aspects of the model that are specific
    /// to a given database.</param>
    /// <remarks>
    /// <para>
    /// If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
    /// then this method will not be run. However, it will still run when creating a compiled model.
    /// </para>
    /// <para>
    /// See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see> for more information and
    /// examples.
    /// </para>
    /// </remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EurofirmsDbContext).Assembly);
    }
}
