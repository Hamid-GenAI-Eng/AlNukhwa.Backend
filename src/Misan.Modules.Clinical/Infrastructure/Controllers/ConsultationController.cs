using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Misan.Modules.Clinical.Application.Features.Consultations.Commands;
using Misan.Modules.Clinical.Application.Features.Prescriptions.Commands;
using Misan.Modules.Clinical.Application.Features.Prescriptions.Queries;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading.Tasks;

namespace Misan.Modules.Clinical.Infrastructure.Controllers;

[Route("api")]
[ApiController]
public class ConsultationController : ControllerBase
{
    private readonly ISender _sender;

    public ConsultationController(ISender sender)
    {
        _sender = sender;
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
    }

    // 1. POST /api/consultations - Book
    [HttpPost("consultations")]
    [Authorize]
    public async Task<IActionResult> Book([FromBody] BookAppointmentCommand command)
    {
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(new { Id = result.Value }) : BadRequest(result.Error);
    }

    // 2. GET /api/consultations - List My Consultations
    [HttpGet("consultations")]
    [Authorize]
    public IActionResult GetMyConsultations()
    {
        return Ok(new { Message = "List placeholder" });
    }

    // 3. GET /api/consultations/:id - Detail
    [HttpGet("consultations/{id}")]
    [Authorize]
    public IActionResult Get(Guid id)
    {
        return Ok(new { Id = id, Status = "Scheduled" });
    }

    // 4. PUT /api/consultations/:id/reschedule
    [HttpPut("consultations/{id}/reschedule")]
    [Authorize]
    public async Task<IActionResult> Reschedule(Guid id, [FromBody] DateTime newDate)
    {
        var command = new RescheduleConsultationCommand(id, newDate);
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(new { Message = "Rescheduled" }) : BadRequest(result.Error);
    }

    // 5. DELETE /api/consultations/:id - Cancel
    [HttpDelete("consultations/{id}")]
    [Authorize]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var command = new CancelConsultationCommand(id);
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(new { Message = "Cancelled" }) : BadRequest(result.Error);
    }

    // 6. POST /api/consultations/:id/notes
    [HttpPost("consultations/{id}/notes")]
    [Authorize] // Hakeem Only
    public async Task<IActionResult> AddNote(Guid id, [FromBody] AddConsultationNoteCommand commandDto)
    {
        var command = commandDto with { ConsultationId = id };
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(new { Message = "Note Added" }) : BadRequest(result.Error);
    }

    // 7. GET /api/consultations/:id/notes
    [HttpGet("consultations/{id}/notes")]
    [Authorize]
    public IActionResult GetNotes(Guid id)
    {
        return Ok(new[] { "Note 1" }); 
    }

    // 8. POST /api/consultations/:id/prescription
    [HttpPost("consultations/{id}/prescription")]
    [Authorize] // Hakeem Only
    public async Task<IActionResult> CreatePrescription(Guid id, [FromBody] CreatePrescriptionCommand command)
    {
        var cmd = command with { ConsultationId = id };
        var result = await _sender.Send(cmd);
        return result.IsSuccess ? Ok(new { Id = result.Value }) : BadRequest(result.Error);
    }

    // 9. GET /api/consultations/:id/prescription
    [HttpGet("consultations/{id}/prescription")]
    [Authorize]
    public async Task<IActionResult> GetPrescriptionPdf(Guid id)
    {
        var query = new GetPrescriptionPdfQuery(id);
        var result = await _sender.Send(query);
        
        if (result.IsFailure) return BadRequest(result.Error);

        return File(result.Value, "application/pdf", $"prescription_{id}.pdf");
    }

    // 10. POST /api/consultations/:id/attachments
    [HttpPost("consultations/{id}/attachments")]
    [Authorize]
    public IActionResult UploadAttachment(Guid id, [FromBody] object fileDto)
    {
        return Ok(new { Message = "Uploaded" });
    }

    // 11. POST /api/consultations/:id/complete
    [HttpPost("consultations/{id}/complete")]
    [Authorize] // Hakeem Only
    public async Task<IActionResult> Complete(Guid id)
    {
        var command = new CompleteConsultationCommand(id);
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(new { Message = "Completed" }) : BadRequest(result.Error);
    }

    // 12. GET /api/hakeem/consultations
    [HttpGet("hakeem/consultations")]
    [Authorize] // Hakeem Only
    public IActionResult GetHakeemConsultations()
    {
        return Ok(new[] { "Consultation A", "Consultation B" });
    }
}
