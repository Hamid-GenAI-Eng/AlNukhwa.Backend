using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Misan.Modules.Intelligence.Application.Features.AIChat.Commands;
using Misan.Modules.Intelligence.Application.Features.AIChat.Queries;
using System;
using System.Threading.Tasks;

namespace Misan.Modules.Intelligence.Infrastructure.Controllers;

[Route("api/ai")]
[ApiController]
[Authorize]
public class AIChatController : ControllerBase
{
    private readonly ISender _sender;

    public AIChatController(ISender sender)
    {
        _sender = sender;
    }

    private Guid GetUserId()
    {
         var claim = User.FindFirst("userId") ?? User.FindFirst("sub") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
         return claim != null && Guid.TryParse(claim.Value, out var id) ? id : Guid.Empty;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> SendAiChat([FromBody] SendAiChatRequest request)
    {
        var command = new SendAiChatCommand(GetUserId(), request.SessionId, request.Message, request.TibbMode);
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions()
    {
        var query = new GetAiSessionsQuery(GetUserId());
        var result = await _sender.Send(query);
        return Ok(result.Value);
    }

    [HttpGet("sessions/{id}")]
    public async Task<IActionResult> GetSessionMessages(Guid id)
    {
        var query = new GetAiSessionMessagesQuery(id, GetUserId());
        var result = await _sender.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("feedback")]
    public async Task<IActionResult> SubmitFeedback([FromBody] SubmitFeedbackRequest request)
    {
        var command = new SubmitAiFeedbackCommand(request.MessageId, request.IsPositive);
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}

public record SendAiChatRequest(Guid? SessionId, string Message, bool TibbMode);
public record SubmitFeedbackRequest(Guid MessageId, bool IsPositive);
