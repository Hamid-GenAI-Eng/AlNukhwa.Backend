using System;
using Misan.Shared.Kernel.Abstractions;
using Misan.Modules.Practitioner.Domain.Enums;

namespace Misan.Modules.Practitioner.Domain.Entities;

public sealed class ClinicService : Entity
{
    internal ClinicService(Guid id, Guid clinicId, string serviceName) : base(id)
    {
        ClinicId = clinicId;
        ServiceName = serviceName;
    }
    private ClinicService() { }

    public Guid ClinicId { get; private set; }
    public string ServiceName { get; private set; } = string.Empty;
}

public sealed class ClinicFee : Entity
{
    internal ClinicFee(Guid id, Guid clinicId, ClinicFeeType type, decimal amount, string currency) : base(id)
    {
        ClinicId = clinicId;
        Type = type;
        Amount = amount;
        Currency = currency;
    }
    private ClinicFee() { }

    public Guid ClinicId { get; private set; }
    public ClinicFeeType Type { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "PKR";
}

public sealed class ClinicSchedule : Entity
{
    internal ClinicSchedule(
        Guid id, 
        Guid clinicId, 
        DayOfWeek dayOfWeek, 
        TimeSpan morningStart, 
        TimeSpan morningEnd, 
        TimeSpan afternoonStart, 
        TimeSpan afternoonEnd,
        bool isClosed) 
        : base(id)
    {
        ClinicId = clinicId;
        DayOfWeek = dayOfWeek;
        MorningStart = morningStart;
        MorningEnd = morningEnd;
        AfternoonStart = afternoonStart;
        AfternoonEnd = afternoonEnd;
        IsClosed = isClosed;
    }
    private ClinicSchedule() { }

    public Guid ClinicId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeSpan MorningStart { get; private set; }
    public TimeSpan MorningEnd { get; private set; }
    public TimeSpan AfternoonStart { get; private set; }
    public TimeSpan AfternoonEnd { get; private set; }
    public bool IsClosed { get; private set; }
}
