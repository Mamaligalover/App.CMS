using MediatR;
using Microsoft.EntityFrameworkCore;
using App.CMS.Data.Context;
using App.CMS.Features.Categories.DTOs;
using AutoMapper;

namespace App.CMS.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto?>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UpdateCategoryCommandHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CategoryDto?> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
            return null;

        category.Name = request.Name;
        category.Slug = request.Slug;
        category.Description = request.Description;
        category.ParentCategoryId = request.ParentCategoryId;
        category.IconId = request.IconId;
        category.IsActive = request.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CategoryDto>(category);
    }
}