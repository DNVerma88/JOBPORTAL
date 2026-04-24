using JobPortal.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Candidates.Commands.UploadResume;

public sealed record UploadResumeCommand(
    Stream FileStream,
    string FileName,
    string ContentType,
    long FileSize
) : IRequest<string>;

public sealed class UploadResumeCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser,
    IFileStorageService fileStorage)
    : IRequestHandler<UploadResumeCommand, string>
{
    private static readonly string[] AllowedContentTypes =
        ["application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"];

    public async Task<string> Handle(UploadResumeCommand request, CancellationToken ct)
    {
        if (!AllowedContentTypes.Contains(request.ContentType.ToLowerInvariant()))
            throw new InvalidOperationException("Only PDF and Word documents are allowed.");

        if (request.FileSize > 5 * 1024 * 1024)
            throw new InvalidOperationException("Resume file must be 5 MB or smaller.");

        var userId = currentUser.GetRequiredUserId();
        var tenantId = currentUser.GetRequiredTenantId();

        var uploadResult = await fileStorage.UploadAsync(new FileUploadRequest(
            Content: request.FileStream,
            FileName: request.FileName,
            ContentType: request.ContentType,
            Folder: $"resumes/{tenantId}",
            MaxSizeBytes: 5 * 1024 * 1024), ct);

        var profile = await db.JobSeekerProfiles.FirstOrDefaultAsync(p => p.UserId == userId, ct);
        if (profile is null)
        {
            var newProfile = Domain.Entities.Portal.JobSeekerProfile.Create(tenantId, userId, userId);
            newProfile.UpdateResumeUrl(uploadResult.PublicUrl, userId);
            db.JobSeekerProfiles.Add(newProfile);
        }
        else
        {
            profile.UpdateResumeUrl(uploadResult.PublicUrl, userId);
        }

        await db.SaveChangesAsync(ct);
        return uploadResult.PublicUrl;
    }
}
