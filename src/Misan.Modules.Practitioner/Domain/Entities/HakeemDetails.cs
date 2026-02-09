using System;
using Misan.Shared.Kernel.Abstractions;
using Misan.Modules.Practitioner.Domain.Enums;

namespace Misan.Modules.Practitioner.Domain.Entities;

public sealed class HakeemSpecialization : Entity
{
    internal HakeemSpecialization(Guid id, Guid hakeemId, string specializationName) : base(id)
    {
        HakeemId = hakeemId;
        SpecializationName = specializationName;
    }
    private HakeemSpecialization() { }

    public Guid HakeemId { get; private set; }
    public string SpecializationName { get; private set; } = string.Empty;
}

public sealed class HakeemLanguage : Entity
{
    internal HakeemLanguage(Guid id, Guid hakeemId, string language) : base(id)
    {
        HakeemId = hakeemId;
        Language = language;
    }
    private HakeemLanguage() { }

    public Guid HakeemId { get; private set; }
    public string Language { get; private set; } = string.Empty;
}

public sealed class HakeemQualification : Entity
{
    internal HakeemQualification(Guid id, Guid hakeemId, string title, string institution, int year) : base(id)
    {
        HakeemId = hakeemId;
        Title = title;
        Institution = institution;
        Year = year;
    }
    private HakeemQualification() { }

    public Guid HakeemId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Institution { get; private set; } = string.Empty;
    public int Year { get; private set; }
}

public sealed class HakeemDocument : Entity
{
    internal HakeemDocument(Guid id, Guid hakeemId, HakeemDocumentType type, string fileUrl) : base(id)
    {
        HakeemId = hakeemId;
        Type = type;
        FileUrl = fileUrl;
        UploadedAt = DateTime.UtcNow;
        Verified = false;
    }
    private HakeemDocument() { }

    public Guid HakeemId { get; private set; }
    public HakeemDocumentType Type { get; private set; }
    public string FileUrl { get; private set; } = string.Empty;
    public DateTime UploadedAt { get; private set; }
    public bool Verified { get; private set; }

    public void Verify() => Verified = true;
}
