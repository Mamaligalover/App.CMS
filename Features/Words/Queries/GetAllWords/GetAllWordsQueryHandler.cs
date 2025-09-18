using MediatR;
using Microsoft.EntityFrameworkCore;
using App.CMS.Data.Context;
using App.CMS.Features.Words.DTOs;
using AutoMapper;

namespace App.CMS.Features.Words.Queries.GetAllWords;

public class GetAllWordsQueryHandler : IRequestHandler<GetAllWordsQuery, List<WordDto>>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public GetAllWordsQueryHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<WordDto>> Handle(GetAllWordsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Words
            .Include(w => w.Category)
            .Include(w => w.CreatedByUser)
            .Include(w => w.Image)
            .Include(w => w.Audio)
            .Include(w => w.Video)
            .Include(w => w.Translations)
            .AsQueryable();

        if (request.CategoryId.HasValue)
        {
            query = query.Where(w => w.CategoryId == request.CategoryId.Value);
        }

        if (!request.IncludeAllStatuses)
        {
            if (request.Status.HasValue)
            {
                query = query.Where(w => w.Status == request.Status.Value);
            }
            else
            {
                query = query.Where(w => w.Status == Data.Enums.WordStatus.Published);
            }
        }

        var words = await query
            .OrderBy(w => w.Term)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<WordDto>>(words);
    }
}