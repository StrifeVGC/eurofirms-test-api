using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PruebaEurofirms.Domain.Entities.Configuration;

/// <summary>
/// Configuration for the episode entity
/// </summary>
/// <seealso cref="Microsoft.EntityFrameworkCore.IEntityTypeConfiguration&lt;PruebaEurofirms.Domain.Entities.Episode&gt;" />
public class EpisodeConfiguration : IEntityTypeConfiguration<Episode>
{
    /// <summary>
    /// Configures the episode entity.
    /// </summary>
    /// <param name="episode">The episode entity.</param>
    public void Configure(EntityTypeBuilder<Episode> episode)
    {
        episode.HasKey(c => c.Id);
        episode.Property(c => c.Id)
            .ValueGeneratedOnAdd();
        episode.Property(e => e.Url).IsRequired();
        episode.HasIndex(e => e.Url).IsUnique(false);
        episode.HasIndex(e => e.ApiId).IsUnique();
    }
}
