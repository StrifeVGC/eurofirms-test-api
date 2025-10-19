using AutoMapper;
using PruebaEurofirms.Domain.Entities;
using PruebaEurofirms.Infrastructure.Models.DTO;

namespace PruebaEurofirms.Infrastructure.Mappers
{
    /// <summary>
    /// Maps a RickAndMortyCharacterDTO into a CharacterProfile
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class RickAndMortyCharacterDTOToCharacterProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RickAndMortyCharacterDTOToCharacterProfile"/> class.
        /// </summary>
        public RickAndMortyCharacterDTOToCharacterProfile()
        {
            CreateMap<RickAndMortyCharacterDto, Character>()
                .ForMember(dest => dest.ApiId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender));
        }
    }
}

