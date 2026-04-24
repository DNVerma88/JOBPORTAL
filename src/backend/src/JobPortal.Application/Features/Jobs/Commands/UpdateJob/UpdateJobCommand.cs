using FluentValidation;
using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Jobs.Commands.UpdateJob;

public sealed record UpdateJobCommand(
    Guid Id,
    string Title,
    string Description,
    string? Responsibilities,
    string? Requirements,
    string? NiceToHave,
    JobType JobType,
    WorkMode WorkMode,
    ExperienceLevel ExperienceLevel,
    short? MinExperienceYears,
    short? MaxExperienceYears,
    decimal? MinSalary,
    decimal? MaxSalary,
    string? CurrencyCode,
    bool IsSalaryHidden,
    Guid? CategoryId,
    Guid? SubCategoryId,
    Guid? CityId,
    Guid? StateId,
    Guid? CountryId,
    short OpeningsCount,
    DateTimeOffset? ExpiresAt,
    bool IsUrgent,
    bool IsFeatured,
    bool IsRemote,
    string? ApplicationEmail,
    string? ApplicationUrl
) : IRequest;

public sealed class UpdateJobCommandValidator : AbstractValidator<UpdateJobCommand>
{
    public UpdateJobCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.OpeningsCount).GreaterThan((short)0);
    }
}

public sealed class UpdateJobCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateJobCommand>
{
    public async Task Handle(UpdateJobCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var job = await db.JobPostings.FirstOrDefaultAsync(j => j.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Job {request.Id} not found.");

        job.Update(request.Title, request.Description, request.Responsibilities, request.Requirements,
            request.NiceToHave, request.JobType, request.WorkMode, request.ExperienceLevel,
            request.MinExperienceYears, request.MaxExperienceYears, request.MinSalary, request.MaxSalary,
            request.CurrencyCode, request.IsSalaryHidden, request.CategoryId, request.SubCategoryId,
            request.CityId, request.StateId, request.CountryId, request.OpeningsCount,
            request.ExpiresAt, request.IsUrgent, request.IsFeatured, request.IsRemote,
            request.ApplicationEmail, request.ApplicationUrl, userId);

        await db.SaveChangesAsync(ct);
    }
}
