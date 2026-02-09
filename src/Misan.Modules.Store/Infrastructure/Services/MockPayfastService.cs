using System;
using System.Threading.Tasks;

namespace Misan.Modules.Store.Infrastructure.Services;

public class MockPayfastService : Misan.Modules.Store.Application.Services.IPaymentService
{
    public Task<string> GeneratePaymentUrl(Guid transactionId, decimal amount, string email)
    {
        // Return a dummy link or just success URL
        return Task.FromResult($"http://localhost:5050/api/payments/callback?tx={transactionId}&status=success");
    }

    public Task<bool> VerifyPayment(Guid transactionId)
    {
        return Task.FromResult(true);
    }
}
