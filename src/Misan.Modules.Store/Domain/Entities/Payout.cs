using Misan.Shared.Kernel.Abstractions;
using System;

namespace Misan.Modules.Store.Domain.Entities;

public sealed class Payout : Entity
{
    private Payout(Guid id, Guid hakeemId, decimal amount, DateTime scheduledDate) : base(id)
    {
        HakeemId = hakeemId;
        Amount = amount;
        Status = PayoutStatus.Pending;
        ScheduledDate = scheduledDate;
        CreatedAt = DateTime.UtcNow;
    }
    
    private Payout() { }

    public Guid HakeemId { get; private set; }
    public decimal Amount { get; private set; }
    public PayoutStatus Status { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public static Payout Create(Guid hakeemId, decimal amount, DateTime scheduledDate)
    {
        return new Payout(Guid.NewGuid(), hakeemId, amount, scheduledDate);
    }

    public void MarkAsProcessing()
    {
        Status = PayoutStatus.Processing;
    }

    public void Complete()
    {
        Status = PayoutStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }
}

public enum PayoutStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2
}
