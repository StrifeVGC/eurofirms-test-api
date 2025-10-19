using PruebaEurofirms.Infrastructure.Models.DTO;
using System.Net.Http.Json;

namespace PruebaEurofirms.Infrastructure.Clients.Implementation
{
    /// <summary>
    /// The Client used to call the Rick and Morty API via HTTP Client.
    /// </summary>
    /// <seealso cref="IRickAndMortyApiClient" />
    public class RickAndMortyApiClient : IRickAndMortyApiClient
    {
        /// <summary>
        /// The HTTP client
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RickAndMortyApiClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        public RickAndMortyApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Gets all characters asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of retrieved characters from the Rick and Morty API.</returns>
        public async Task<List<RickAndMortyCharacterDto>> GetAllCharactersAsync(CancellationToken cancellationToken = default)
        {
            var allCharacters = new List<RickAndMortyCharacterDto>();
            string? nextUrl = _httpClient.BaseAddress?.ToString() + RickAndMortyApiClientConstants.CharacterEndpoint;

            while (!string.IsNullOrEmpty(nextUrl))
            {
                var response = await _httpClient.GetFromJsonAsync<RickAndMortyCharacterApiResponseDTO>(nextUrl, cancellationToken);
                if (response == null || response.Results == null || response.Results.Count == 0)
                    break;

                allCharacters.AddRange(response.Results);
                nextUrl = response.Info?.Next;
            }

            return allCharacters;
        }
    }
}
