namespace App.CMS.Application.Services;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, Dictionary<string, string>? metadata = null);
    Task<Stream> DownloadFileAsync(string fileName);
    Task<bool> DeleteFileAsync(string fileName);
    Task<string> GetPresignedUrlAsync(string fileName, int expiryInSeconds = 3600);
    Task<bool> FileExistsAsync(string fileName);
    Task<FileInfo> GetFileInfoAsync(string fileName);
    Task<IEnumerable<FileInfo>> ListFilesAsync(string? prefix = null);
}

public class FileInfo
{
    public string FileName { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
}