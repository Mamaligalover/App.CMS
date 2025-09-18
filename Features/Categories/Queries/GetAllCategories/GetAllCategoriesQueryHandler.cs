using MediatR;
using Microsoft.EntityFrameworkCore;
using App.CMS.Data.Context;
using App.CMS.Features.Categories.DTOs;
using AutoMapper;

namespace App.CMS.Features.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public GetAllCategoriesQueryHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.Icon)
            .Include(c => c.SubCategories)
            .AsQueryable();

        if (!request.IncludeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        var categories = await query
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<CategoryDto>>(categories);
    }
}