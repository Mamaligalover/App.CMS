using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Result;
using Microsoft.Extensions.Options;
using App.CMS.Application.Configuration;
using System.Reactive.Linq;
using Minio.ApiEndpoints;

namespace App.CMS.Application.Services;

public class MinIOFileStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly MinIOSettings _settings;
    private readonly ILogger<MinIOFileStorageService> _logger;

    public MinIOFileStorageService(IOptions<MinIOSettings> settings, ILogger<MinIOFileStorageService> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        _minioClient = new MinioClient()
            .WithEndpoint(_settings.Endpoint)
            .WithCredentials(_settings.AccessKey, _settings.SecretKey)
            .WithSSL(_settings.UseSSL)
            .Build();

        InitializeBucketAsync().GetAwaiter().GetResult();
    }

    private async Task InitializeBucketAsync()
    {
        try
        {
            var beArgs = new BucketExistsArgs()
                .WithBucket(_settings.BucketName);

            bool found = await _minioClient.BucketExistsAsync(beArgs);

            if (!found)
            {
                var mbArgs = new MakeBucketArgs()
                    .WithBucket(_settings.BucketName);
                await _minioClient.MakeBucketAsync(mbArgs);
                _logger.LogInformation("Created MinIO bucket: {BucketName}", _settings.BucketName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing MinIO bucket");
            throw;
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, Dictionary<string, string>? metadata = null)
    {
        try
        {
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(fileName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType);

            if (metadata != null && metadata.Any())
            {
                putObjectArgs = putObjectArgs.WithHeaders(metadata);
            }

            await _minioClient.PutObjectAsync(putObjectArgs);

            _logger.LogInformation("Uploaded file: {FileName} to bucket: {BucketName}", fileName, _settings.BucketName);
            return fileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", fileName);
            throw;
        }
    }

    public async Task<Stream> DownloadFileAsync(string fileName)
    {
        try
        {
            var memoryStream = new MemoryStream();

            var getObjectArgs = new GetObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(fileName)
                .WithCallbackStream(async (stream) =>
                {
                    await stream.CopyToAsync(memoryStream);
                });

            await _minioClient.GetObjectAsync(getObjectArgs);
            memoryStream.Position = 0;

            _logger.LogInformation("Downloaded file: {FileName} from bucket: {BucketName}", fileName, _settings.BucketName);
            return memoryStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file: {FileName}", fileName);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string fileName)
    {
        try
        {
            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(fileName);

            await _minioClient.RemoveObjectAsync(removeObjectArgs);

            _logger.LogInformation("Deleted file: {FileName} from bucket: {BucketName}", fileName, _settings.BucketName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FileName}", fileName);
            return false;
        }
    }

    public async Task<string> GetPresignedUrlAsync(string fileName, int expiryInSeconds = 3600)
    {
        try
        {
            var presignedGetObjectArgs = new PresignedGetObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(fileName)
                .WithExpiry(expiryInSeconds);

            var url = await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);

            _logger.LogInformation("Generated presigned URL for file: {FileName}", fileName);
            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URL for file: {FileName}", fileName);
            throw;
        }
    }

    public async Task<bool> FileExistsAsync(string fileName)
    {
        try
        {
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(fileName);

            await _minioClient.StatObjectAsync(statObjectArgs);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<FileInfo> GetFileInfoAsync(string fileName)
    {
        try
        {
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(fileName);

            var objectStat = await _minioClient.StatObjectAsync(statObjectArgs);

            return new FileInfo
            {
                FileName = objectStat.ObjectName,
                Size = objectStat.Size,
                LastModified = objectStat.LastModified,
                ContentType = objectStat.ContentType,
                Metadata = objectStat.MetaData ?? new Dictionary<string, string>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file info: {FileName}", fileName);
            throw;
        }
    }

    public async Task<IEnumerable<FileInfo>> ListFilesAsync(string? prefix = null)
    {
        try
        {
            var files = new List<FileInfo>();

            var listObjectsArgs = new ListObjectsArgs()
                .WithBucket(_settings.BucketName)
                .WithRecursive(true);

            if (!string.IsNullOrEmpty(prefix))
            {
                listObjectsArgs = listObjectsArgs.WithPrefix(prefix);
            }

            var observable =  _minioClient.ListObjectsAsync(listObjectsArgs);

            await observable.ForEachAsync(item =>
            {
                files.Add(new FileInfo
                {
                    FileName = item.Key,
                    Size = (long)item.Size,
                    LastModified = DateTime.Parse(item.LastModified),
                    ContentType = "application/octet-stream"
                });
            });

            _logger.LogInformation("Listed {Count} files from bucket: {BucketName}", files.Count, _settings.BucketName);
            return files;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing files");
            throw;
        }
    }
}