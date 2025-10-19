using PruebaEurofirms.Infrastructure.Models.DTO;

/// <summary>
/// The Rick and Morty character API response DTO.
/// </summary>
public class RickAndMortyCharacterApiResponseDTO
{
    /// <summary>
    /// Gets or sets the information.
    /// </summary>
    /// <value>
    /// The information.
    /// </value>
    public RickAndMortyCharacterApiInfoDTO Info { get; set; } = new();

    /// <summary>
    /// Gets or sets the results.
    /// </summary>
    /// <value>
    /// The results.
    /// </value>
    public List<RickAndMortyCharacterDto> Results { get; set; } = new();
}
