using Misan.Shared.Kernel.Abstractions;
using System;
using System.Collections.Generic;

namespace Misan.Modules.Clinical.Domain.Entities;

public sealed class Prescription : Entity
{
    private readonly List<PrescriptionItem> _items = new();

    private Prescription(Guid id, Guid consultationId) : base(id)
    {
        ConsultationId = consultationId;
        CreatedAt = DateTime.UtcNow;
    }

    private Prescription() { }

    public Guid ConsultationId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? PdfUrl { get; private set; } // Generated PDF URL

    public IReadOnlyCollection<PrescriptionItem> Items => _items.AsReadOnly();

    public static Prescription Create(Guid consultationId)
    {
        return new Prescription(Guid.NewGuid(), consultationId);
    }

    public void AddItem(string remedy, string dosage, string instructions)
    {
        _items.Add(new PrescriptionItem(Guid.NewGuid(), Id, remedy, dosage, instructions));
    }

    public void SetPdfUrl(string url)
    {
        PdfUrl = url;
    }
}

public sealed class PrescriptionItem : Entity
{
    internal PrescriptionItem(Guid id, Guid prescriptionId, string remedy, string dosage, string instructions) : base(id)
    {
        PrescriptionId = prescriptionId;
        Remedy = remedy;
        Dosage = dosage;
        Instructions = instructions;
    }
    private PrescriptionItem() { }

    public Guid PrescriptionId { get; private set; }
    public string Remedy { get; private set; } = string.Empty;
    public string Dosage { get; private set; } = string.Empty;
    public string Instructions { get; private set; } = string.Empty;
}
