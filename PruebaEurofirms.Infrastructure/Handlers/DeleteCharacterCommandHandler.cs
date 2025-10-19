using MediatR;
using Microsoft.Extensions.Logging;
using PruebaEurofirms.Domain.Repositories.Contract;

namespace PruebaEurofirms.Infrastructure.Handlers
{
    /// <summary>
    ///   Command that deletes a character based on their Id
    /// </summary>
    public record DeleteCharacterCommand(int CharacterApiId) : IRequest<bool>;

    /// <summary>
    /// The DeleteCharacterHandler
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;PruebaEurofirms.Infrastructure.Handlers.DeleteCharacterCommand, System.Boolean&gt;" />
    public class DeleteCharacterCommandHandler : IRequestHandler<DeleteCharacterCommand, bool>
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly ILogger<DeleteCharacterCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCharacterCommandHandler"/> class.
        /// </summary>
        /// <param name="characterRepository">The character repository.</param>
        /// <param name="logger">The logger.</param>
        public DeleteCharacterCommandHandler(ICharacterRepository characterRepository, ILogger<DeleteCharacterCommandHandler> logger)
        {
            _characterRepository = characterRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Character with ApiId {request.CharacterApiId} was not found.</exception>
        /// <exception cref="System.Exception">An unexpected error occurred while deleting the character. Please try again later.</exception>
        public async Task<bool> Handle(DeleteCharacterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to delete character with ApiId: {CharacterApiId}", request.CharacterApiId);

                var character = await _characterRepository.GetByApiIdAsync(request.CharacterApiId, includeEpisodes: true);

                if (character == null)
                {
                    throw new KeyNotFoundException($"Character with ApiId {request.CharacterApiId} was not found.");
                }

                _characterRepository.Remove(character);
                await _characterRepository.SaveChangesAsync();

                _logger.LogInformation("Character with ApiId {CharacterApiId} deleted successfully", request.CharacterApiId);

                return true;
            }
            catch (KeyNotFoundException)
            {

                _logger.LogError("Character with ApiId {CharacterApiId} not found", request.CharacterApiId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting character with ApiId {CharacterApiId}", request.CharacterApiId);
                throw new Exception("An unexpected error occurred while deleting the character. Please try again later.", ex);
            }
        }
    }
}
