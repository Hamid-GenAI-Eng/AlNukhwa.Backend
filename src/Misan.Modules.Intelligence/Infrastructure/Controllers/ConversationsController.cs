using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Misan.Modules.Intelligence.Application.Features.Messaging.Commands;
using Misan.Modules.Intelligence.Application.Features.Messaging.Queries;
using System;
using System.Threading.Tasks;

namespace Misan.Modules.Intelligence.Infrastructure.Controllers;

[Route("api/conversations")]
[ApiController]
[Authorize]
public class ConversationsController : ControllerBase
{
    private readonly ISender _sender;

    public ConversationsController(ISender sender)
    {
        _sender = sender;
    }

    private Guid GetUserId()
    {
         var claim = User.FindFirst("userId") ?? User.FindFirst("sub") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
         return claim != null && Guid.TryParse(claim.Value, out var id) ? id : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> GetConversations()
    {
        var query = new GetConversationsQuery(GetUserId());
        var result = await _sender.Send(query);
        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMessages(Guid id)
    {
        var query = new GetMessagesQuery(id, GetUserId());
        var result = await _sender.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{id}/messages")]
    public async Task<IActionResult> SendMessage(Guid id, [FromBody] SendMessageRequest request)
    {
        var command = new SendMessageCommand(GetUserId(), id, null, request.Content, request.AttachmentUrl);
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(new { MessageId = result.Value }) : BadRequest(result.Error);
    }
    
    [HttpPost]
    public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request)
    {
        var command = new SendMessageCommand(GetUserId(), null, request.RecipientId, request.Content, null);
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(new { MessageId = result.Value }) : BadRequest(result.Error);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var command = new MarkConversationReadCommand(id, GetUserId());
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}

public record SendMessageRequest(string Content, string? AttachmentUrl);
public record StartConversationRequest(Guid RecipientId, string Content);
