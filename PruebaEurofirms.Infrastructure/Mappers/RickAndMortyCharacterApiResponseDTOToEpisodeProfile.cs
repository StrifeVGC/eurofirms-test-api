using AutoMapper;
using PruebaEurofirms.Domain.Entities;

namespace EurofirmsTest.Infrastructure.MappingProfiles
{
    /// <summary>
    ///   The profile to map from an Episode URL to Episode object
    /// </summary>
    public class RickAndMortyCharacterApiResponseDTOToEpisodeProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RickAndMortyCharacterApiResponseDTOToEpisodeProfile"/> class.
        /// </summary>
        public RickAndMortyCharacterApiResponseDTOToEpisodeProfile()
        {
            CreateMap<string, Episode>().ConvertUsing<StringToEpisodeConverter>();
        }
    }
}
