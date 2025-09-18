using System.ComponentModel.DataAnnotations;
using App.CMS.Data.Enums;

namespace App.CMS.Data.Entities;

public class Word
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Term { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? PartOfSpeech { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public WordStatus Status { get; set; } = WordStatus.Draft;

    public WordDifficulty Difficulty { get; set; } = WordDifficulty.Medium;

    public Guid? ImageId { get; set; }
    public Media? Image { get; set; }

    public Guid? AudioId { get; set; }
    public Media? Audio { get; set; }

    public Guid? VideoId { get; set; }
    public Media? Video { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public User? CreatedByUser { get; set; }

    public ICollection<Translation> Translations { get; set; } = new List<Translation>();
}