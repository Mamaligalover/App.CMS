using MediatR;
using App.CMS.Features.Words.DTOs;
using App.CMS.Data.Enums;

namespace App.CMS.Features.Words.Commands.UpdateWord;

public class UpdateWordCommand : IRequest<WordDto?>
{
    public Guid Id { get; set; }
    public string Term { get; set; } = string.Empty;
    public string? PartOfSpeech { get; set; }
    public Guid CategoryId { get; set; }
    public WordStatus Status { get; set; }
    public WordDifficulty Difficulty { get; set; }
    public Guid? ImageId { get; set; }
    public Guid? AudioId { get; set; }
    public Guid? VideoId { get; set; }
    public List<TranslationDto> Translations { get; set; } = new();
}