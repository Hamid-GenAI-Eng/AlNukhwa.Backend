namespace Misan.Modules.Practitioner.Domain.Enums;

public enum VerificationStatus
{
    Pending = 1,
    Verified = 2,
    Rejected = 3
}

public enum HakeemDocumentType
{
    Certificate = 1,
    CNIC = 2,
    License = 3,
    Photo = 4
}

public enum ClinicFeeType
{
    Virtual = 1,
    InPerson = 2,
    Emergency = 3
}

public enum BreakType
{
    Daily = 1,
    Holiday = 2
}
