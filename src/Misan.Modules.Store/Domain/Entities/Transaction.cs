using Misan.Shared.Kernel.Abstractions;
using Misan.Modules.Store.Domain.Enums;
using System;

namespace Misan.Modules.Store.Domain.Entities;

public sealed class Transaction : Entity
{
    private Transaction(Guid id, TransactionType type, Guid userId, Guid? hakeemId, Guid? orderId, Guid? consultationId, decimal amount, decimal fee, decimal net) : base(id)
    {
        Type = type;
        UserId = userId;
        HakeemId = hakeemId;
        OrderId = orderId;
        ConsultationId = consultationId;
        GrossAmount = amount;
        PlatformFee = fee;
        NetAmount = net;
        Status = TransactionStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }
    
    private Transaction() { }

    public TransactionType Type { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? HakeemId { get; private set; }
    public Guid? OrderId { get; private set; }
    public Guid? ConsultationId { get; private set; }
    
    public decimal GrossAmount { get; private set; }
    public decimal PlatformFee { get; private set; }
    public decimal NetAmount { get; private set; }
    
    public TransactionStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public static Transaction CreateSale(Guid userId, Guid orderId, decimal amount)
    {
        // Marketplace sale: Platform takes fee? Or direct? Assuming platform sale for now.
        return new Transaction(Guid.NewGuid(), TransactionType.ProductSale, userId, null, orderId, null, amount, 0, amount);
    }

    public static Transaction CreateConsultationPayment(Guid userId, Guid hakeemId, Guid consultationId, decimal amount, decimal fee)
    {
        return new Transaction(Guid.NewGuid(), TransactionType.ConsultationFee, userId, hakeemId, null, consultationId, amount, fee, amount - fee);
    }

    public void Complete()
    {
        Status = TransactionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }
}
