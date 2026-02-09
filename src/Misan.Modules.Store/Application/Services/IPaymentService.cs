using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading.Tasks;

namespace Misan.Modules.Store.Application.Services;

public interface IPaymentService
{
    Task<string> GeneratePaymentUrl(Guid transactionId, decimal amount, string email);
    Task<bool> VerifyPayment(Guid transactionId);
}
