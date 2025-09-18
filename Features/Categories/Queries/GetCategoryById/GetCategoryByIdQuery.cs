using MediatR;
using App.CMS.Features.Categories.DTOs;

namespace App.CMS.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQuery : IRequest<CategoryDto?>
{
    public Guid Id { get; set; }

    public GetCategoryByIdQuery(Guid id)
    {
        Id = id;
    }
}