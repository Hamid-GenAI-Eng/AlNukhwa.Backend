namespace Misan.Modules.Clinical.Domain.Enums;

public enum ConsultationStatus
{
    Scheduled,
    Ongoing,
    Completed,
    Cancelled,
    NoShow
}

public enum ConsultationType
{
    InPerson,
    VideoCall,
    AudioCall,
    Chat
}
