using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Misan.Modules.Practitioner.Application.Features.Practitioner.Commands;
using Misan.Modules.Practitioner.Application.Features.Practitioner.Queries;
using Misan.Modules.Practitioner.Domain.Enums;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Misan.Modules.Practitioner.Infrastructure.Controllers;

[Route("api")]
[ApiController]
public class HakeemController : ControllerBase
{
    private readonly ISender _sender;

    public HakeemController(ISender sender)
    {
        _sender = sender;
    }

    private Guid GetUserId() 
    {
        var claim = User.FindFirst("userId") ?? User.FindFirst("sub") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return claim != null && Guid.TryParse(claim.Value, out var id) ? id : Guid.Empty;
    }

    // GET /api/hakeems - Search/filter hakeems
    [HttpGet("hakeems")]
    public async Task<IActionResult> GetHakeems([FromQuery] GetHakeemsQuery query)
    {
        var result = await _sender.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    // GET /api/hakeems/:id - Public profile
    [HttpGet("hakeems/{id}")]
    public async Task<IActionResult> GetHakeemById(Guid id)
    {
        var result = await _sender.Send(new GetHakeemDetailQuery(id));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    // GET /api/hakeems/recommended
    [HttpGet("hakeems/recommended")]
    public async Task<IActionResult> GetRecommendedHakeems()
    {
        var query = new GetHakeemsQuery(null, null, null, 1, 5);
        var result = await _sender.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    // GET /api/hakeem/dashboard
    [Authorize]
    [HttpGet("hakeem/dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var result = await _sender.Send(new GetHakeemDashboardQuery(GetUserId()));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    // PUT /api/hakeem/profile
    [Authorize]
    [HttpPut("hakeem/profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateHakeemProfileRequest request)
    {
        var command = new UpdateHakeemProfileCommand(GetUserId(), request.Specializations, request.Languages, request.Qualifications);
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(new { Message = "Profile Updated" }) : BadRequest(result.Error);
    }

    // GET /api/hakeem/patients - Mock (Requires Clinical Module Integration)
    [Authorize]
    [HttpGet("hakeem/patients")]
    public async Task<IActionResult> GetPatients()
    {
        // Placeholder until Clinical Module integration
        return await Task.FromResult(Ok(new[] { new { Name = "Patient A", Id = Guid.NewGuid() } }));
    }

    // GET /api/hakeem/patients/:id
    [Authorize]
    [HttpGet("hakeem/patients/{id}")]
    public async Task<IActionResult> GetPatient(Guid id)
    {
        return await Task.FromResult(Ok(new { PatientId = id, Name = "Patient A Details" }));
    }

    // GET /api/hakeem/schedule
    [Authorize]
    [HttpGet("hakeem/schedule")]
    public async Task<IActionResult> GetSchedule()
    {
        // Ideally this should be a query: GetHakeemScheduleQuery
        // For now returning mock as query not fully defined in plan, but Entity exists.
        return await Task.FromResult(Ok(new { Message = "Use /api/hakeems/recommended or detail to see availability." }));
    }

    // PUT /api/hakeem/schedule
    [Authorize]
    [HttpPut("hakeem/schedule")]
    public async Task<IActionResult> UpdateSchedule([FromBody] UpdateScheduleRequest request)
    {
        var command = new UpdateScheduleCommand(GetUserId(), request.ClinicId, request.Schedules);
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(new { Message = "Schedule Updated" }) : BadRequest(result.Error);
    }

    // GET /api/hakeem/earnings
    [Authorize]
    [HttpGet("hakeem/earnings")]
    public async Task<IActionResult> GetEarnings()
    {
        var result = await _sender.Send(new GetHakeemDashboardQuery(GetUserId()));
        return result.IsSuccess ? Ok(new { Total = result.Value.TotalEarnings, Currency = "PKR" }) : BadRequest(result.Error);
    }

    // GET /api/hakeem/transactions
    [Authorize]
    [HttpGet("hakeem/transactions")]
    public async Task<IActionResult> GetTransactions()
    {
        return await Task.FromResult(Ok(new[] { new { Id = Guid.NewGuid(), Amount = 1000, Date = DateTime.UtcNow } }));
    }

    // POST /api/hakeem/payout/request
    [Authorize]
    [HttpPost("hakeem/payout/request")]
    public async Task<IActionResult> RequestPayout([FromBody] RequestPayoutCommand command)
    {
        var cmd = command with { HakeemId = GetUserId() };
        var result = await _sender.Send(cmd);
        return result.IsSuccess ? Ok(new { Message = "Payout Requested" }) : BadRequest(result.Error);
    }

    // GET /api/hakeem/reputation
    [Authorize]
    [HttpGet("hakeem/reputation")]
    public async Task<IActionResult> GetReputation()
    {
        var result = await _sender.Send(new GetHakeemDashboardQuery(GetUserId()));
        return result.IsSuccess ? Ok(new { Rating = result.Value.Rating, Reviews = 50 }) : BadRequest(result.Error);
    }

    // PUT /api/hakeem/clinic
    [Authorize]
    [HttpPut("hakeem/clinic")]
    public async Task<IActionResult> UpdateClinic([FromBody] UpdateClinicRequest request)
    {
        var command = new UpdateClinicCommand(GetUserId(), request.ClinicName, request.Address, request.City, request.MapCoordinates, request.PhoneNumber);
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(new { Message = "Clinic Updated" }) : BadRequest(result.Error);
    }

    // POST /api/hakeem/documents
    [Authorize]
    [HttpPost("hakeem/documents")]
    public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentRequest request) // FromForm for File
    {
        if (request.File == null || request.File.Length == 0)
             return BadRequest("No file uploaded.");

        var command = new UploadHakeemDocumentCommand(GetUserId(), request.File, request.DocumentType);
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(new { Url = result.Value }) : BadRequest(result.Error);
    }
}

// Request DTOs
public record UpdateHakeemProfileRequest(List<string>? Specializations, List<string>? Languages, List<QualificationDto>? Qualifications);
public record UpdateScheduleRequest(Guid ClinicId, List<ScheduleDto> Schedules);
public record UpdateClinicRequest(string ClinicName, string Address, string City, string MapCoordinates, string PhoneNumber);
public record UploadDocumentRequest(IFormFile File, HakeemDocumentType DocumentType);
