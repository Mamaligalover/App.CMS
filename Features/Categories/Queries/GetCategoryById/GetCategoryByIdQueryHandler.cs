using MediatR;
using Microsoft.EntityFrameworkCore;
using App.CMS.Data.Context;
using App.CMS.Features.Categories.DTOs;
using AutoMapper;

namespace App.CMS.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public GetCategoryByIdQueryHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.Icon)
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        return _mapper.Map<CategoryDto?>(category);
    }
}