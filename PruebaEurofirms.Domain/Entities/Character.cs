namespace PruebaEurofirms.Domain.Entities;

/// <summary>
/// The Character Entity
/// </summary>
public class Character
{
    /// <summary>
    /// Primary Key
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the API identifier.
    /// </summary>
    /// <value>
    /// The API identifier.
    /// </value>
    public int ApiId { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    /// <value>
    /// The status.
    /// </value>
    public string Status { get; set; } = null!;

    /// <summary>
    /// Gets or sets the gender.
    /// </summary>
    /// <value>
    /// The gender.
    /// </value>
    public string Gender { get; set; } = null!;

    /// <summary>
    /// Gets or sets the created at.
    /// </summary>
    /// <value>
    /// The created at.
    /// </value>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the updated at.
    /// </summary>
    /// <value>
    /// The updated at.
    /// </value>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the episodes.
    /// </summary>
    /// <value>
    /// The episodes.
    /// </value>
    public ICollection<Episode> Episodes { get; set; } = new List<Episode>();

}
