namespace JobPortal.Application.Common.Interfaces;

public record FileUploadRequest(
    Stream Content,
    string FileName,
    string ContentType,
    string Folder,
    long MaxSizeBytes = 10 * 1024 * 1024); // 10 MB default

public record FileUploadResult(string FileKey, string PublicUrl, long SizeBytes);

public interface IFileStorageService
{
    Task<FileUploadResult> UploadAsync(FileUploadRequest request, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(string fileKey, CancellationToken cancellationToken = default);
    Task DeleteAsync(string fileKey, CancellationToken cancellationToken = default);
    Task<string> GetPreSignedUrlAsync(string fileKey, TimeSpan expiry, CancellationToken cancellationToken = default);
}
