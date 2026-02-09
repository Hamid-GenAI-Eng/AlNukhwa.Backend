namespace Misan.Modules.Profiles.Domain.Enums;

public enum MembershipTier
{
    Bronze = 1,
    Silver = 2,
    Gold = 3
}

public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3
}

public enum BodyType
{
    Sanguine = 1,
    Phlegmatic = 2,
    Choleric = 3,
    Melancholic = 4
}

public enum BloodGroup
{
    APositive,
    ANegative,
    BPositive,
    BNegative,
    ABPositive,
    ABNegative,
    OPositive,
    ONegative,
    Unknown
}

public enum Severity
{
    Mild,
    Moderate,
    Severe
}

public enum MedicationType
{
    Clinical,
    Herbal
}
