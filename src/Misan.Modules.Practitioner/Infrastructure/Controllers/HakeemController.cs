using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Misan.Modules.Practitioner.Application.Features.Practitioner.Commands;
using Misan.Modules.Practitioner.Application.Features.Practitioner.Queries;
using Misan.Shared.Kernel.Abstractions;
using System;
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
        // Extract from JWT Claims
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
    }

    // GET /api/hakeems - Search/filter hakeems
    [HttpGet("hakeems")]
    public async Task<IActionResult> GetHakeems([FromQuery] GetHakeemsQuery query)
    {
        try 
        {
            var result = await _sender.Send(query);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.ToString(), statusCode: 500);
        }
    }

    // GET /api/hakeems/:id - Public profile
    [HttpGet("hakeems/{id}")]
    public async Task<IActionResult> GetHakeemById(Guid id)
    {
        // Implement Query: GetHakeemDetailQuery
        // Placeholder return
        return Ok(new { Message = "Hakeem Profile", Id = id });
    }

    // GET /api/hakeems/recommended
    [HttpGet("hakeems/recommended")]
    public async Task<IActionResult> GetRecommendedHakeems()
    {
        // Could reuse GetHakeemsQuery with special sort
        var query = new GetHakeemsQuery(null, null, null, 1, 5);
        var result = await _sender.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    // GET /api/hakeem/dashboard
    [Authorize]
    [HttpGet("hakeem/dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        return Ok(new { Stats = "Dashboard placeholders" });
    }

    // PUT /api/hakeem/profile
    [Authorize]
    [HttpPut("hakeem/profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] object updateDto)
    {
        // Implement UpdateHakeemProfileCommand
        return Ok(new { Message = "Profile Updated" });
    }

    // GET /api/hakeem/patients
    [Authorize]
    [HttpGet("hakeem/patients")]
    public async Task<IActionResult> GetPatients()
    {
        return Ok(new[] { "Patient A", "Patient B" });
    }

    // GET /api/hakeem/patients/:id
    [Authorize]
    [HttpGet("hakeem/patients/{id}")]
    public async Task<IActionResult> GetPatient(Guid id)
    {
        return Ok(new { PatientId = id });
    }

    // GET /api/hakeem/schedule
    [Authorize]
    [HttpGet("hakeem/schedule")]
    public async Task<IActionResult> GetSchedule()
    {
        // Get Schedule Query
        return Ok(new { Message = "Schedule Logic" });
    }

    // PUT /api/hakeem/schedule
    [Authorize]
    [HttpPut("hakeem/schedule")]
    public async Task<IActionResult> UpdateSchedule([FromBody] UpdateScheduleCommand command)
    {
        // Secure: Force HakeemId from Token?
        // Ideally command should be mapped from DTO + UserId
        return Ok(new { Message = "Schedule Updated" });
    }

    // GET /api/hakeem/earnings
    [Authorize]
    [HttpGet("hakeem/earnings")]
    public async Task<IActionResult> GetEarnings()
    {
        return Ok(new { Total = 1000, Currency = "PKR" });
    }

    // GET /api/hakeem/transactions
    [Authorize]
    [HttpGet("hakeem/transactions")]
    public async Task<IActionResult> GetTransactions()
    {
        return Ok(new[] { "Tx 1", "Tx 2" });
    }

    // POST /api/hakeem/payout/request
    [Authorize]
    [HttpPost("hakeem/payout/request")]
    public async Task<IActionResult> RequestPayout()
    {
        return Ok(new { Message = "Payout Requested" });
    }

    // GET /api/hakeem/reputation
    [Authorize]
    [HttpGet("hakeem/reputation")]
    public async Task<IActionResult> GetReputation()
    {
        return Ok(new { Rating = 4.5, Reviews = 10 });
    }

    // PUT /api/hakeem/clinic
    [Authorize]
    [HttpPut("hakeem/clinic")]
    public async Task<IActionResult> UpdateClinic([FromBody] object clinicDto)
    {
        return Ok(new { Message = "Clinic Updated" });
    }

    // POST /api/hakeem/documents
    [Authorize]
    [HttpPost("hakeem/documents")]
    public async Task<IActionResult> UploadDocument([FromBody] object docDto)
    {
        return Ok(new { Message = "Document Uploaded" });
    }
}
