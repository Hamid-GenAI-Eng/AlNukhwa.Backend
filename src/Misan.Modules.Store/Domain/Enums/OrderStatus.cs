namespace Misan.Modules.Store.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,
    Processing = 1, // Paid
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}
