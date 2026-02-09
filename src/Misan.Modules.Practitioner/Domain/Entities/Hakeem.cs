using System;
using System.Collections.Generic;
using Misan.Shared.Kernel.Abstractions;
using Misan.Modules.Practitioner.Domain.Enums;

namespace Misan.Modules.Practitioner.Domain.Entities;

public sealed class Hakeem : Entity
{
    private readonly List<HakeemSpecialization> _specializations = new();
    private readonly List<HakeemLanguage> _languages = new();
    private readonly List<HakeemQualification> _qualifications = new();
    private readonly List<HakeemDocument> _documents = new();

    private Hakeem(
        Guid id,
        Guid userId,
        string cnicNumber,
        string licenseNumber,
        int yearsOfExperience)
        : base(id)
    {
        UserId = userId;
        CnicNumber = cnicNumber;
        LicenseNumber = licenseNumber;
        YearsOfExperience = yearsOfExperience;
        VerificationStatus = VerificationStatus.Pending;
        Rating = 0; // Default
    }

    private Hakeem() { }

    public Guid UserId { get; private set; }
    public string CnicNumber { get; private set; } = string.Empty;
    public string LicenseNumber { get; private set; } = string.Empty;
    public int YearsOfExperience { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public decimal Rating { get; private set; } // Weighted Scoring base

    public IReadOnlyCollection<HakeemSpecialization> Specializations => _specializations.AsReadOnly();
    public IReadOnlyCollection<HakeemLanguage> Languages => _languages.AsReadOnly();
    public IReadOnlyCollection<HakeemQualification> Qualifications => _qualifications.AsReadOnly();
    public IReadOnlyCollection<HakeemDocument> Documents => _documents.AsReadOnly();

    public static Hakeem Create(Guid userId, string cnic, string license, int experience)
    {
        return new Hakeem(Guid.NewGuid(), userId, cnic, license, experience);
    }

    public void Verify()
    {
        VerificationStatus = VerificationStatus.Verified;
        VerifiedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        VerificationStatus = VerificationStatus.Rejected;
        VerifiedAt = null;
    }

    public void AddSpecialization(string name)
    {
        if (!_specializations.Exists(s => s.SpecializationName == name))
            _specializations.Add(new HakeemSpecialization(Guid.NewGuid(), Id, name));
    }

    public void AddLanguage(string language)
    {
        if (!_languages.Exists(l => l.Language == language))
            _languages.Add(new HakeemLanguage(Guid.NewGuid(), Id, language));
    }

    public void AddQualification(string title, string institution, int year)
    {
        _qualifications.Add(new HakeemQualification(Guid.NewGuid(), Id, title, institution, year));
    }

    public void AddDocument(HakeemDocumentType type, string url)
    {
        _documents.Add(new HakeemDocument(Guid.NewGuid(), Id, type, url));
    }
}
