using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Intelligence.Domain.Entities;
using Misan.Modules.Intelligence.Infrastructure.Database;
using Misan.Modules.Intelligence.Infrastructure.Services;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Intelligence.Application.Features.AIChat.Commands;

public record SendAiChatCommand(Guid UserId, Guid? SessionId, string Message, bool TibbMode) : IRequest<Result<AiResponseDto>>;

public record AiResponseDto(Guid SessionId, Guid UserMessageId, Guid AiMessageId, string AiResponse);

public class SendAiChatCommandHandler : IRequestHandler<SendAiChatCommand, Result<AiResponseDto>>
{
    private readonly IntelligenceDbContext _dbContext;
    private readonly IFastApiService _fastApiService;
    private readonly IPatientHealthService _healthService; // Injected from Shared.Kernel

    public SendAiChatCommandHandler(IntelligenceDbContext dbContext, IFastApiService fastApiService, IPatientHealthService healthService)
    {
        _dbContext = dbContext;
        _fastApiService = fastApiService;
        _healthService = healthService;
    }

    public async Task<Result<AiResponseDto>> Handle(SendAiChatCommand request, CancellationToken cancellationToken)
    {
        AIChatSession session;

        if (request.SessionId.HasValue)
        {
            session = await _dbContext.AIChatSessions.FindAsync(new object[] { request.SessionId.Value }, cancellationToken);
            if (session == null) return Result.Failure<AiResponseDto>(new Error("Session.NotFound", "Session not found"));
            
            // Update preferences if changed
            if (session.TibbMode != request.TibbMode) session.UpdatePreferences(request.TibbMode);
        }
        else
        {
            // Create new session
            // Generate title from first few words
            var title = request.Message.Length > 30 ? request.Message.Substring(0, 30) + "..." : request.Message;
            session = AIChatSession.Create(request.UserId, title, request.TibbMode);
            _dbContext.AIChatSessions.Add(session);
        }

        // Context Building
        object? context = null;
        if (session.TibbMode)
        {
             var healthContext = await _healthService.GetHealthContextAsync(request.UserId, cancellationToken);
             context = new { healthProfile = healthContext };
        }

        // Save User Message
        var userMsg = AIChatMessage.Create(session.Id, "user", request.Message);
        _dbContext.AIChatMessages.Add(userMsg);

        // Call AI
        // Construct prompt with history? 
        // For simple proxy, we send current message + context. 
        // Ideally we send history too, but keeping it simple as per requirement "Proxy".
        string aiResponseText = await _fastApiService.ChatAsync(request.Message, context);

        // Save AI Message
        var aiMsg = AIChatMessage.Create(session.Id, "assistant", aiResponseText);
        _dbContext.AIChatMessages.Add(aiMsg);

        session.AddMessage(userMsg); // Just to update timestamp
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new AiResponseDto(session.Id, userMsg.Id, aiMsg.Id, aiResponseText));
    }
}
