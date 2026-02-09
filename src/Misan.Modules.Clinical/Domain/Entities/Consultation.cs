using Misan.Shared.Kernel.Abstractions;
using Misan.Modules.Clinical.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Misan.Modules.Clinical.Domain.Entities;

public sealed class Consultation : Entity
{
    private readonly List<ConsultationNote> _notes = new();
    private readonly List<ConsultationAttachment> _attachments = new();

    private Consultation(
        Guid id,
        Guid patientId,
        Guid hakeemId,
        DateTime scheduledAt,
        ConsultationType type,
        decimal feeAmount)
        : base(id)
    {
        PatientId = patientId;
        HakeemId = hakeemId;
        ScheduledAt = scheduledAt;
        Type = type;
        FeeAmount = feeAmount;
        Status = ConsultationStatus.Scheduled;
        CreatedAt = DateTime.UtcNow;
    }

    private Consultation() { }

    public Guid PatientId { get; private set; }
    public Guid HakeemId { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public ConsultationType Type { get; private set; }
    public ConsultationStatus Status { get; private set; }
    public decimal FeeAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public IReadOnlyCollection<ConsultationNote> Notes => _notes.AsReadOnly();
    public IReadOnlyCollection<ConsultationAttachment> Attachments => _attachments.AsReadOnly();
    
    // Prescription is 1-to-0..1 relation usually, or 1-to-many? Stick to 1-to-1 main prescription for now or collection.
    // Let's keep it separate aggregate or child? 
    // Aggregate Root logic: Prescription is usually independent document but tightly coupled.
    // Let's make Prescription a separate Aggregate or Entity referenced here? 
    // Plan said "Prescription: Linked to consultation".
    // I will add a property here if it's 1-to-1, or just let Prescription hold the FK.
    // Let's assume Prescription holds FK.

    public static Consultation Schedule(Guid patientId, Guid hakeemId, DateTime scheduledAt, ConsultationType type, decimal fee)
    {
        // Validations can go here (business rules)
        return new Consultation(Guid.NewGuid(), patientId, hakeemId, scheduledAt, type, fee);
    }

    public void Complete()
    {
        Status = ConsultationStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = ConsultationStatus.Cancelled;
    }

    public void Reschedule(DateTime newDate)
    {
        ScheduledAt = newDate;
        // Reset status if needed?
    }

    public void AddNote(string text, bool isPrivate)
    {
        _notes.Add(new ConsultationNote(Guid.NewGuid(), Id, text, isPrivate));
    }

    public void AddAttachment(string url, string name, string type)
    {
        _attachments.Add(new ConsultationAttachment(Guid.NewGuid(), Id, url, name, type));
    }
}
