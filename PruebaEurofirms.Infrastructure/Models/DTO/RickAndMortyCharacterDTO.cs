namespace PruebaEurofirms.Infrastructure.Models.DTO
{
    /// <summary>
    /// The RickAndMortyCharacterDto
    /// </summary>
    public class RickAndMortyCharacterDto
    {
        /// <summary>
        /// Gets or sets the Api Id.
        /// </summary>
        /// <value>
        /// The Api Id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public string Gender { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the episode.
        /// </summary>
        /// <value>
        /// The episode.
        /// </value>
        public List<string> Episode { get; set; } = new();
    }
}
