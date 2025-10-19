namespace PruebaEurofirms.Domain.Repositories
{
    /// <summary>
    /// The Base Repository.
    /// </summary>
    public class BaseRepository
    {
        /// <summary>
        /// The db Context.
        /// </summary>
        protected readonly EurofirmsDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public BaseRepository(EurofirmsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Saves the changes asynchronous. Allows calling this method without giving external services direct access to the context.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
