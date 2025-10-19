using MediatR;
using Microsoft.Extensions.Logging;
using PruebaEurofirms.Domain.Entities;
using PruebaEurofirms.Domain.Repositories.Contract;

namespace PruebaEurofirms.Infrastructure.Handlers
{
    /// <summary>
    /// Query for retrieving characters filtered by status (Alive, Dead, Unknown).
    /// </summary>
    public record GetCharactersByStatusQuery(string Status) : IRequest<List<Character>>;

    /// <summary>
    /// Handler for <see cref="GetCharactersByStatusQuery"/>.
    /// </summary>
    public class GetCharactersByStatusQueryHandler : IRequestHandler<GetCharactersByStatusQuery, List<Character>>
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly ILogger<GetCharactersByStatusQueryHandler> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="GetCharactersByStatusQueryHandler"/>.
        /// </summary>
        /// <param name="characterRepository">The character repository.</param>
        /// <param name="logger">Logger instance.</param>
        public GetCharactersByStatusQueryHandler(ICharacterRepository characterRepository, ILogger<GetCharactersByStatusQueryHandler> logger)
        {
            _characterRepository = characterRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles the query to get characters by status.
        /// </summary>
        /// <param name="request">The query request containing the status filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of characters matching the given status.</returns>
        /// <exception cref="Exception">Throws if an unexpected error occurs.</exception>
        public async Task<List<Character>> Handle(GetCharactersByStatusQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting retrieval of characters with status: {Status}", request.Status);

                var characters = await _characterRepository.GetByStatusAsync(request.Status);

                _logger.LogInformation("{Count} characters matched the status '{Status}'", characters.Count, request.Status);

                return characters;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving characters with status: {Status}", request.Status);
                throw;
            }
        }
    }
}
