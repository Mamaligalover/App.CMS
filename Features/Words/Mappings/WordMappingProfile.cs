using AutoMapper;
using App.CMS.Data.Entities;
using App.CMS.Features.Words.DTOs;

namespace App.CMS.Features.Words.Mappings;

public class WordMappingProfile : Profile
{
    public WordMappingProfile()
    {
        CreateMap<Word, WordDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.CreatedByUserName,
                opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.Username : null))
            .ForMember(dest => dest.ImageUrl,
                opt => opt.MapFrom(src => src.Image != null ? $"/api/media/{src.Image.Id}" : null))
            .ForMember(dest => dest.AudioUrl,
                opt => opt.MapFrom(src => src.Audio != null ? $"/api/media/{src.Audio.Id}" : null))
            .ForMember(dest => dest.VideoUrl,
                opt => opt.MapFrom(src => src.Video != null ? $"/api/media/{src.Video.Id}" : null))
            .ForMember(dest => dest.Translations,
                opt => opt.MapFrom(src => src.Translations));
    }
}