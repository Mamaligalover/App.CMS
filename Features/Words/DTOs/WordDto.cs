using App.CMS.Data.Enums;

namespace App.CMS.Features.Words.DTOs;

public class WordDto
{
    public Guid Id { get; set; }
    public string Term { get; set; } = string.Empty;
    public string? PartOfSpeech { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public WordStatus Status { get; set; }
    public WordDifficulty Difficulty { get; set; }
    public Guid? ImageId { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? AudioId { get; set; }
    public string? AudioUrl { get; set; }
    public Guid? VideoId { get; set; }
    public string? VideoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedByUserName { get; set; }
    public List<TranslationDto> Translations { get; set; } = new();
}