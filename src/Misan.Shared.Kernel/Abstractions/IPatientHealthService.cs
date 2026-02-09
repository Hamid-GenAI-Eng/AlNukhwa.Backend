using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Shared.Kernel.Abstractions;

public interface IPatientHealthService
{
    Task<string> GetHealthContextAsync(Guid userId, CancellationToken cancellationToken);
}
