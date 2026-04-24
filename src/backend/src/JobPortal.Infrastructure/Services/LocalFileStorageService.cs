using JobPortal.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace JobPortal.Infrastructure.Services;

/// <summary>
/// Local disk file storage for development.
/// Replace with AzureBlobStorageService / S3FileStorageService in production.
/// Switching is transparent via IFileStorageService abstraction.
/// </summary>
public sealed class LocalFileStorageService(
    IWebHostEnvironment env,
    ILogger<LocalFileStorageService> logger) : IFileStorageService
{
    private string UploadsRoot => Path.Combine(env.ContentRootPath, "uploads");

    public async Task<FileUploadResult> UploadAsync(FileUploadRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Content.Length > request.MaxSizeBytes)
            throw new InvalidOperationException($"File exceeds maximum allowed size of {request.MaxSizeBytes / 1024 / 1024} MB.");

        var folderPath = Path.Combine(UploadsRoot, request.Folder);
        Directory.CreateDirectory(folderPath);

        var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(request.FileName)}";
        var relativePath = Path.Combine(request.Folder, uniqueName);
        var absolutePath = Path.Combine(UploadsRoot, relativePath);

        await using var fs = File.Create(absolutePath);
        await request.Content.CopyToAsync(fs, cancellationToken);

        logger.LogInformation("File uploaded: {FileKey} ({Bytes} bytes)", relativePath, fs.Length);

        return new FileUploadResult(relativePath, $"/uploads/{relativePath.Replace('\\', '/')}", fs.Length);
    }

    public Task<Stream> DownloadAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        var absolutePath = Path.Combine(UploadsRoot, fileKey);
        if (!File.Exists(absolutePath))
            throw new FileNotFoundException("File not found.", fileKey);

        return Task.FromResult<Stream>(File.OpenRead(absolutePath));
    }

    public Task DeleteAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        var absolutePath = Path.Combine(UploadsRoot, fileKey);
        if (File.Exists(absolutePath))
            File.Delete(absolutePath);
        return Task.CompletedTask;
    }

    public Task<string> GetPreSignedUrlAsync(string fileKey, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        // Local dev: return direct URL (no signing needed)
        return Task.FromResult($"/uploads/{fileKey.Replace('\\', '/')}");
    }
}
