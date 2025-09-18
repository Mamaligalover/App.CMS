using MediatR;
using App.CMS.Data.Context;
using App.CMS.Data.Entities;
using App.CMS.Features.Categories.DTOs;
using AutoMapper;

namespace App.CMS.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Slug,
            Description = request.Description,
            ParentCategoryId = request.ParentCategoryId,
            IconId = request.IconId,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CategoryDto>(category);
    }
}