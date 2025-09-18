using AutoMapper;
using App.CMS.Data.Entities;
using App.CMS.Features.Categories.DTOs;

namespace App.CMS.Features.Categories.Mappings;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.ParentCategoryName,
                opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null))
            .ForMember(dest => dest.IconUrl,
                opt => opt.MapFrom(src => src.Icon != null ? $"/api/media/{src.Icon.Id}" : null));
    }
}