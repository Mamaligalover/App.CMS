using MediatR;
using Microsoft.EntityFrameworkCore;
using App.CMS.Data.Context;
using App.CMS.Features.Words.DTOs;
using AutoMapper;

namespace App.CMS.Features.Words.Queries.GetWordById;

public class GetWordByIdQueryHandler : IRequestHandler<GetWordByIdQuery, WordDto?>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public GetWordByIdQueryHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<WordDto?> Handle(GetWordByIdQuery request, CancellationToken cancellationToken)
    {
        var word = await _context.Words
            .Include(w => w.Category)
            .Include(w => w.CreatedByUser)
            .Include(w => w.Image)
            .Include(w => w.Audio)
            .Include(w => w.Video)
            .Include(w => w.Translations)
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        return _mapper.Map<WordDto?>(word);
    }
}