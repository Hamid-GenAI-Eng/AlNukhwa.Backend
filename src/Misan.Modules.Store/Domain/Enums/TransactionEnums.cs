namespace Misan.Modules.Store.Domain.Enums;

public enum TransactionType
{
    ProductSale = 1,
    ConsultationFee = 2
}

public enum TransactionStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2,
    Refunded = 3
}
