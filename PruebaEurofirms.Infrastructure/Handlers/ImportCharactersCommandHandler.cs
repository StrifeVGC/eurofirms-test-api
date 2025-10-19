using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PruebaEurofirms.Domain.Entities;
using PruebaEurofirms.Domain.Repositories.Contract;

namespace PruebaEurofirms.Infrastructure.Handlers
{
    /// <summary>
    /// Command to import characters from the Rick and Morty API into the database.
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Int32&gt;" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;PruebaEurofirms.Infrastructure.Handlers.ImportCharactersCommand&gt;" />
    public record ImportCharactersCommand() : IRequest<int>;

    /// <summary>
    /// The Import Characters Handler.
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;PruebaEurofirms.Infrastructure.Handlers.ImportCharactersCommand, System.Int32&gt;" />
    public class ImportCharactersCommandHandler : IRequestHandler<ImportCharactersCommand, int>
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IEpisodeRepository _episodeRepository;
        private readonly IRickAndMortyApiClient _apiClient;
        private readonly IMapper _mapper;
        private readonly ILogger<ImportCharactersCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCharactersCommandHandler"/> class.
        /// </summary>
        /// <param name="characterRepository">The character repository.</param>
        /// <param name="episodeRepository">The episode repository.</param>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="apiClient">The API client.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="logger">The logger.</param>
        public ImportCharactersCommandHandler(
            ICharacterRepository characterRepository,
            IEpisodeRepository episodeRepository,
            HttpClient httpClient,
            IRickAndMortyApiClient apiClient,
            IMapper mapper,
            ILogger<ImportCharactersCommandHandler> logger)
        {
            _characterRepository = characterRepository;
            _episodeRepository = episodeRepository;
            _apiClient = apiClient;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the import of characters. If a character already exists in the database (based on ApiId), it is skipped. Episodes are also imported if they do not already exist.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The count of inserted characters.</returns>
        public async Task<int> Handle(ImportCharactersCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting import of characters from API...");

                var apiCharacters = await _apiClient.GetAllCharactersAsync(cancellationToken);
                _logger.LogInformation("Retrieved {Count} characters from API.", apiCharacters.Count);

                var apiCharactersWrapper = apiCharacters
                    .Select(dto => new
                    {
                        Character = _mapper.Map<Character>(dto),
                        Episodes = _mapper.Map<List<Episode>>(dto.Episode ?? new List<string>())
                    })
                    .ToList();

                var existingCharacters = await _characterRepository.GetAllAsync(includeEpisodes: false, cancellationToken);
                var charactersToInsertIds = apiCharacters.Select(c => c.Id).Except(existingCharacters.Select(x => x.ApiId));

                var charactersToInsert = apiCharactersWrapper
                    .Where(w => charactersToInsertIds.Contains(w.Character.ApiId))
                    .ToList();

                if (!charactersToInsert.Any())
                {
                    _logger.LogInformation("No new characters to insert.");
                    return 0;
                }

                _logger.LogInformation("Preparing episodes for insertion...");
                var allEpisodes = apiCharactersWrapper.SelectMany(w => w.Episodes).ToList();
                var existingEpisodes = await _episodeRepository.GetAllAsync(cancellationToken);
                var episodesToInsertIds = allEpisodes.Select(e => e.ApiId)
                    .Except(existingEpisodes.Select(x => x.ApiId));

                var episodesToInsert = allEpisodes
                    .Where(e => episodesToInsertIds.Contains(e.ApiId))
                    .DistinctBy(x => x.ApiId)
                    .ToList();

                await _episodeRepository.AddRangeAsync(episodesToInsert);

                var allEpisodesDict = (existingEpisodes.Concat(episodesToInsert)).ToDictionary(e => e.ApiId);

                var charactersPrepared = charactersToInsert.Select(characterWrapper =>
                {
                    characterWrapper.Character.Episodes = characterWrapper.Episodes
                        .Select(ep => allEpisodesDict[ep.ApiId])
                        .ToList();
                    _logger.LogDebug("Prepared character {CharacterId} for insertion.", characterWrapper.Character.ApiId);
                    return characterWrapper.Character;
                }).ToList();

                await _characterRepository.AddRangeAsync(charactersPrepared);

                await _characterRepository.SaveChangesAsync();
                _logger.LogInformation("Finished importing {Count} characters.", charactersToInsert.Count);

                return charactersToInsert.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while importing characters.");
                throw new ApplicationException("Failed to import characters from API.", ex);
            }
        }
    }
}
