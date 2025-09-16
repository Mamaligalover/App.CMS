namespace App.CMS.Data.Entities;

public class Media
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string? Description { get; set; }
    public FileType FileType { get; set; }
    public Guid? UserId { get; set; }
    public User? User { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public enum FileType
{
    Image,
    Document,
    Video,
    Audio,
    Other
}