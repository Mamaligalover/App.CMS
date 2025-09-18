using MediatR;
using App.CMS.Features.Categories.DTOs;

namespace App.CMS.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommand : IRequest<CategoryDto>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public Guid? IconId { get; set; }
    public bool IsActive { get; set; } = true;
}