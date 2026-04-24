using JobPortal.API.Models;
using JobPortal.Application.Common.Models;
using JobPortal.Application.Features.Companies.Commands;
using JobPortal.Application.Features.Companies.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class CompaniesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetCompaniesQuery(pageNumber, pageSize, search), ct);
        return Ok(ApiResponse<PagedList<CompanySummaryDto>>.Ok(result, "Companies retrieved."));
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine(CancellationToken ct)
    {
        var result = await mediator.Send(new GetMyCompanyQuery(), ct);
        return Ok(ApiResponse<CompanyDetailDto?>.Ok(result, "Company retrieved."));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCompanyByIdQuery(id), ct);
        if (result is null) return NotFound(ApiResponse.Fail("Company not found."));
        return Ok(ApiResponse<CompanyDetailDto>.Ok(result, "Company retrieved."));
    }

    [HttpPut("mine")]
    public async Task<IActionResult> UpdateMine([FromBody] UpdateMyCompanyCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return Ok(ApiResponse<Guid>.Ok(id, "Company updated."));
    }

    [HttpGet("{id:guid}/branches")]
    [AllowAnonymous]
    public IActionResult GetBranches(Guid id) =>
        Ok(ApiResponse<List<object>>.Ok([], "Branches retrieved."));

    [HttpGet("{id:guid}/reviews")]
    [AllowAnonymous]
    public IActionResult GetReviews(Guid id) =>
        Ok(ApiResponse<List<object>>.Ok([], "Reviews retrieved."));
}
