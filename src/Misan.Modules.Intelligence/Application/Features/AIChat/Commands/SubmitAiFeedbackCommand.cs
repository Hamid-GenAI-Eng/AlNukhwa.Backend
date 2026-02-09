using MediatR;
using Misan.Modules.Intelligence.Domain.Entities;
using Misan.Modules.Intelligence.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Intelligence.Application.Features.AIChat.Commands;

public record SubmitAiFeedbackCommand(Guid MessageId, bool IsPositive) : IRequest<Result>;

public class SubmitAiFeedbackCommandHandler : IRequestHandler<SubmitAiFeedbackCommand, Result>
{
    private readonly IntelligenceDbContext _dbContext;
    public SubmitAiFeedbackCommandHandler(IntelligenceDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result> Handle(SubmitAiFeedbackCommand request, CancellationToken cancellationToken)
    {
        var message = await _dbContext.AIChatMessages.FindAsync(new object[] { request.MessageId }, cancellationToken);
        if (message == null) return Result.Failure(new Error("Message.NotFound", "Message not found"));

        message.AddFeedback(request.IsPositive);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
