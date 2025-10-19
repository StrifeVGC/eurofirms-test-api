namespace PruebaEurofirms.Domain.Entities;

/// <summary>
/// The episode entity.
/// </summary>
public class Episode
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
    /// Gets or sets the URL.
    /// </summary>
    /// <value>
    /// The URL.
    /// </value>
    public string Url { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the characters (many-to-many rel).
    /// </summary>
    /// <value>
    /// The characters.
    /// </value>
    public ICollection<Character> Characters { get; set; } = new List<Character>();
}
