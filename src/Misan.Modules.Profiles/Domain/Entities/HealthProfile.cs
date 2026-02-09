using System;
using System.Collections.Generic;
using Misan.Modules.Profiles.Domain.Enums;
using Misan.Shared.Kernel.Abstractions;

namespace Misan.Modules.Profiles.Domain.Entities;

public sealed class HealthProfile : Entity
{
    private readonly List<Condition> _conditions = new();
    private readonly List<Allergy> _allergies = new();
    private readonly List<Medication> _medications = new();
    private readonly List<MedicalHistory> _medicalHistories = new();

    private HealthProfile(Guid id, Guid profileId, BodyType? bodyType, BloodGroup? bloodGroup) 
        : base(id)
    {
        ProfileId = profileId;
        BodyType = bodyType;
        BloodGroup = bloodGroup;
    }

    private HealthProfile() { }

    public Guid ProfileId { get; private set; }
    public BodyType? BodyType { get; private set; }
    public BloodGroup? BloodGroup { get; private set; }

    public IReadOnlyCollection<Condition> Conditions => _conditions.AsReadOnly();
    public IReadOnlyCollection<Allergy> Allergies => _allergies.AsReadOnly();
    public IReadOnlyCollection<Medication> Medications => _medications.AsReadOnly();
    public IReadOnlyCollection<MedicalHistory> MedicalHistories => _medicalHistories.AsReadOnly();

    public static HealthProfile Create(Guid profileId, BodyType? bodyType, BloodGroup? bloodGroup)
    {
        return new HealthProfile(Guid.NewGuid(), profileId, bodyType, bloodGroup);
    }
    
    public void Update(BodyType? bodyType, BloodGroup? bloodGroup)
    {
        BodyType = bodyType;
        BloodGroup = bloodGroup;
    }

    public void AddCondition(Condition condition) => _conditions.Add(condition);
    public void RemoveCondition(Condition condition) => _conditions.Remove(condition);
    
    public void AddAllergy(Allergy allergy) => _allergies.Add(allergy);
    
    // ... Methods for other collections
}
