using MediatR;
using App.CMS.Features.Categories.DTOs;

namespace App.CMS.Features.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQuery : IRequest<List<CategoryDto>>
{
    public bool IncludeInactive { get; set; }
}