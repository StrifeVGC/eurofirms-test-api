using AutoMapper;
using PruebaEurofirms.Domain.Entities;

namespace EurofirmsTest.Infrastructure.MappingProfiles
{

    /// <summary>
    /// Helps Convert string URLs to Episode entities.
    /// </summary>
    public class StringToEpisodeConverter : ITypeConverter<string, Episode>
    {

        /// <summary>Performs conversion from String URL to Episode type</summary>
        /// <param name="source">Episode URL</param>
        /// <param name="destination">Episode Object</param>
        /// <param name="context">Resolution context</param>
        /// <returns>Converted Episode Object</returns>
        public Episode Convert(string source, Episode destination, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source))
                return new Episode { ApiId = 0, Url = source ?? string.Empty };

            var trimmed = source.TrimEnd('/');
            var lastSegment = trimmed.Split('/').LastOrDefault();

            if (int.TryParse(lastSegment, out var id))
            {
                return new Episode
                {
                    ApiId = id,
                    Url = source
                };
            }

            return new Episode
            {
                ApiId = 0,
                Url = source
            };
        }
    }
}
