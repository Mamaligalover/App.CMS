using MediatR;
using App.CMS.Data.Context;
using App.CMS.Data.Entities;
using App.CMS.Features.Words.DTOs;
using AutoMapper;

namespace App.CMS.Features.Words.Commands.CreateWord;

public class CreateWordCommandHandler : IRequestHandler<CreateWordCommand, WordDto>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CreateWordCommandHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<WordDto> Handle(CreateWordCommand request, CancellationToken cancellationToken)
    {
        var word = new Word
        {
            Id = Guid.NewGuid(),
            Term = request.Term,
            PartOfSpeech = request.PartOfSpeech,
            CategoryId = request.CategoryId,
            Status = request.Status,
            Difficulty = request.Difficulty,
            ImageId = request.ImageId,
            AudioId = request.AudioId,
            VideoId = request.VideoId,
            CreatedByUserId = request.CreatedByUserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Words.Add(word);

        // Add translations
        if (request.Translations.Any())
        {
            foreach (var translationDto in request.Translations)
            {
                var translation = new Translation
                {
                    Id = Guid.NewGuid(),
                    WordId = word.Id,
                    Language = translationDto.Language,
                    TranslatedText = translationDto.TranslatedText,
                    Pronunciation = translationDto.Pronunciation,
                    Definition = translationDto.Definition,
                    UsageExample = translationDto.UsageExample,
                    Notes = translationDto.Notes,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Translations.Add(translation);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Reload with includes for mapping
        await _context.Entry(word)
            .Reference(w => w.Category)
            .LoadAsync(cancellationToken);

        await _context.Entry(word)
            .Collection(w => w.Translations)
            .LoadAsync(cancellationToken);

        if (word.CreatedByUserId.HasValue)
        {
            await _context.Entry(word)
                .Reference(w => w.CreatedByUser)
                .LoadAsync(cancellationToken);
        }

        return _mapper.Map<WordDto>(word);
    }
}