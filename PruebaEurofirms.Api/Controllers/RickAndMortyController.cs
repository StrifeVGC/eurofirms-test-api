using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaEurofirms.Infrastructure.Handlers;

namespace PruebaEurofirms.Controllers
{
    /// <summary>
    /// The controller for Rick and Morty characters.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RickAndMortyController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RickAndMortyController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RickAndMortyController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        /// <param name="logger">The logger.</param>
        public RickAndMortyController(IMediator mediator, ILogger<RickAndMortyController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Imports characters that are not present in the database. If present, they are skipped.
        /// </summary>
        /// <returns>Number of inserted characters.</returns>
        /// <response code="200">Returns the count of characters successfully inserted.</response>
        /// <response code="500">Returns an error message if an unexpected error occurred during the import.</response>
        [HttpPost("import")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ImportCharacters()
        {
            _logger.LogInformation("ImportCharacters endpoint called.");

            try
            {
                var insertedCount = await _mediator.Send(new ImportCharactersCommand());

                _logger.LogInformation("ImportCharacters endpoint finished successfully. {InsertedCount} characters inserted.", insertedCount);

                return Ok(new
                {
                    inserted = insertedCount
                });
            }
            catch
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while importing characters. Please try again later."
                });
            }
        }

        /// <summary>
        /// Gets the characters filtered by their status (Alive, Dead, Unknown).
        /// </summary>
        /// <param name="status">The status of the characters to retrieve (Alive, Dead, or Unknown).</param>
        /// <returns>Returns a list of characters matching the provided status.</returns>
        /// <response code="200">Returns the list of characters.</response>
        /// <response code="400">Returned if the status provided is invalid.</response>
        /// <response code="500">Returned if an unexpected error occurs.</response>
        [HttpGet("status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCharactersByStatus([FromQuery] string status)
        {
            try
            {
                _logger.LogInformation("Received request to get characters with status: {Status}", status);

                var validator = new CharacterStatusValidator();
                var validationResult = validator.Validate(status);

                if (!validationResult.IsValid)
                {
                    _logger.LogError("Validation failed for status: {Status}. Errors: {Errors}",
                        status, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                    return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                }

                var characters = await _mediator.Send(new GetCharactersByStatusQuery(status));

                _logger.LogInformation("Retrieved {Count} characters with status {Status}", characters.Count, status);

                var result = characters.Select(c => new
                {
                    c.Name,
                    c.Status,
                    c.Gender,
                    Episodes = c.Episodes.Select(e => e.Url).ToList()
                });

                return Ok(result);
            }
            catch
            {
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }

        /// <summary>
        /// Deletes the character associated with the provided ID.
        /// </summary>
        /// <param name="id">The Id for the character to be deleted.</param>
        /// <returns>Success message in case the character was deleted</returns>
        /// <response code="200">Character deleted successfully</response>
        /// <response code="404">Character not found</response>
        /// <response code="500">unexpected error occurred</response>

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteCharacter(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete character with Id: {CharacterId}", id);

                var deleted = await _mediator.Send(new DeleteCharacterCommand(id));

                if (!deleted)
                {
                    _logger.LogWarning("Character with Id {CharacterId} not found", id);
                    return NotFound(new
                    {
                        message = "Character not found"
                    });
                }

                _logger.LogInformation("Character with Id {CharacterId} deleted successfully", id);

                return Ok(new
                {
                    message = "Character deleted successfully"
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Character with Id {CharacterId} not found", id);
                return NotFound(new
                {
                    message = $"Character with Id {id} not found"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting character with Id {CharacterId}", id);
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while deleting the character. Please try again later."
                });
            }
        }
    }
}
