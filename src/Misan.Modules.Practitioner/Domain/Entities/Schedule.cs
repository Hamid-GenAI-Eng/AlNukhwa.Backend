using System;
using Misan.Shared.Kernel.Abstractions;
using Misan.Modules.Practitioner.Domain.Enums;

namespace Misan.Modules.Practitioner.Domain.Entities;

public sealed class ScheduleBreak : Entity
{
    public ScheduleBreak(Guid id, Guid hakeemId, string name, TimeSpan startTime, TimeSpan endTime, BreakType type, DateTime? date = null)
        : base(id)
    {
        HakeemId = hakeemId;
        Name = name;
        StartTime = startTime;
        EndTime = endTime;
        Type = type;
        Date = date;
    }
    private ScheduleBreak() { }

    public Guid HakeemId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public BreakType Type { get; private set; }
    public DateTime? Date { get; private set; } // Null for daily recurring breaks? Or logic handles it?
}

public sealed class ScheduleConfig : Entity
{
    public ScheduleConfig(Guid id, Guid hakeemId, int slotDurationMinutes, int bufferTimeMinutes, int noticePeriodHours, bool autoAccept, string timezone)
        : base(id)
    {
        HakeemId = hakeemId;
        SlotDurationMinutes = slotDurationMinutes;
        BufferTimeMinutes = bufferTimeMinutes;
        NoticePeriodHours = noticePeriodHours;
        AutoAccept = autoAccept;
        Timezone = timezone;
    }
    private ScheduleConfig() { }

    public Guid HakeemId { get; private set; }
    public int SlotDurationMinutes { get; private set; }
    public int BufferTimeMinutes { get; private set; }
    public int NoticePeriodHours { get; private set; }
    public bool AutoAccept { get; private set; }
    public string Timezone { get; private set; } = "UTC";

    public static ScheduleConfig CreateDefault(Guid hakeemId)
    {
        return new ScheduleConfig(Guid.NewGuid(), hakeemId, 30, 0, 24, false, "UTC");
    }
    
    public void Update(int slotDuration, int buffer, int notice, bool autoAccept, string timezone)
    {
        SlotDurationMinutes = slotDuration;
        BufferTimeMinutes = buffer;
        NoticePeriodHours = notice;
        AutoAccept = autoAccept;
        Timezone = timezone;
    }
}
