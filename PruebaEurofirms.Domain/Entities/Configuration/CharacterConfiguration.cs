using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PruebaEurofirms.Domain.Entities.Configuration;

/// <summary>
/// Configuration for the Character entity
/// </summary>
/// <seealso cref="Microsoft.EntityFrameworkCore.IEntityTypeConfiguration&lt;PruebaEurofirms.Domain.Entities.Character&gt;" />
public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    /// <summary>
    /// Configures the Character entity.
    /// </summary>
    /// <param name="character">The character entity.</param>
    public void Configure(EntityTypeBuilder<Character> character)
    {
        character.HasKey(c => c.Id);
        character.Property(c => c.Id)
            .ValueGeneratedOnAdd();
        character.Property(c => c.Name).IsRequired();
        character.Property(c => c.Status).IsRequired();
        character.Property(c => c.Gender).IsRequired();
        character.HasIndex(c => c.Status);
        character.HasIndex(c => c.ApiId).IsUnique();

        character.HasMany(c => c.Episodes)
            .WithMany(e => e.Characters);
    }
}