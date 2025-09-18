using MediatR;
using Microsoft.EntityFrameworkCore;
using App.CMS.Data.Context;
using App.CMS.Data.Entities;
using App.CMS.Features.Words.DTOs;
using AutoMapper;

namespace App.CMS.Features.Words.Commands.UpdateWord;

public class UpdateWordCommandHandler : IRequestHandler<UpdateWordCommand, WordDto?>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UpdateWordCommandHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<WordDto?> Handle(UpdateWordCommand request, CancellationToken cancellationToken)
    {
        var word = await _context.Words
            .Include(w => w.Category)
            .Include(w => w.CreatedByUser)
            .Include(w => w.Image)
            .Include(w => w.Audio)
            .Include(w => w.Video)
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (word == null)
            return null;

        word.Term = request.Term;
        word.PartOfSpeech = request.PartOfSpeech;
        word.CategoryId = request.CategoryId;
        word.Status = request.Status;
        word.Difficulty = request.Difficulty;
        word.ImageId = request.ImageId;
        word.AudioId = request.AudioId;
        word.VideoId = request.VideoId;
        word.UpdatedAt = DateTime.UtcNow;

        // Update translations
        // Remove existing translations
        var existingTranslations = await _context.Translations
            .Where(t => t.WordId == word.Id)
            .ToListAsync(cancellationToken);
        _context.Translations.RemoveRange(existingTranslations);

        // Add new translations
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

        // Reload relationships for mapping
        await _context.Entry(word)
            .Reference(w => w.Category)
            .LoadAsync(cancellationToken);

        await _context.Entry(word)
            .Collection(w => w.Translations)
            .LoadAsync(cancellationToken);

        return _mapper.Map<WordDto>(word);
    }
}