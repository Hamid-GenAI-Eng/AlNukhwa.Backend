using System;
using Misan.Modules.Profiles.Domain.Enums;
using Misan.Shared.Kernel.Abstractions;

namespace Misan.Modules.Profiles.Domain.Entities;

public sealed class Condition : Entity
{
    public Condition(Guid id, Guid healthProfileId, string name, Severity severity, DateTime diagnosedDate) : base(id)
    {
        HealthProfileId = healthProfileId;
        Name = name;
        Severity = severity;
        DiagnosedDate = diagnosedDate;
    }
    private Condition() { }
    
    public Guid HealthProfileId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Severity Severity { get; private set; }
    public DateTime DiagnosedDate { get; private set; }
}

public sealed class Allergy : Entity
{
    public Allergy(Guid id, Guid healthProfileId, string name, Severity severity, string description) : base(id)
    {
        HealthProfileId = healthProfileId;
        Name = name;
        Severity = severity;
        Description = description;
    }
    private Allergy() { }

    public Guid HealthProfileId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Severity Severity { get; private set; }
    public string Description { get; private set; } = string.Empty;
}

public sealed class Medication : Entity
{
    public Medication(Guid id, Guid healthProfileId, string name, string dosage, MedicationType type, string? imageUrl) : base(id)
    {
        HealthProfileId = healthProfileId;
        Name = name;
        Dosage = dosage;
        Type = type;
        ImageUrl = imageUrl;
        IsActive = true;
    }
    private Medication() { }

    public Guid HealthProfileId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Dosage { get; private set; } = string.Empty;
    public MedicationType Type { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }
}

public sealed class MedicalHistory : Entity
{
    public MedicalHistory(Guid id, Guid healthProfileId, string title, string description, DateTime date) : base(id)
    {
        HealthProfileId = healthProfileId;
        Title = title;
        Description = description;
        Date = date;
        IsActive = true;
    }
    private MedicalHistory() { }

    public Guid HealthProfileId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime Date { get; private set; }
    public bool IsActive { get; private set; }
}

public sealed class EmergencyContact : Entity
{
    public EmergencyContact(Guid id, Guid profileId, string name, string relationship, string phone) : base(id)
    {
        ProfileId = profileId;
        Name = name;
        Relationship = relationship;
        Phone = phone;
    }
    private EmergencyContact() { }

    public Guid ProfileId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Relationship { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
}
