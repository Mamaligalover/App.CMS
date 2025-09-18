using AutoMapper;
using App.CMS.Data.Entities;
using App.CMS.Features.Words.DTOs;

namespace App.CMS.Features.Words.Mappings;

public class TranslationMappingProfile : Profile
{
    public TranslationMappingProfile()
    {
        CreateMap<Translation, TranslationDto>();
        CreateMap<TranslationDto, Translation>();
    }
}