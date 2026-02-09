using Misan.Shared.Kernel.Abstractions;
using System;

namespace Misan.Modules.Clinical.Domain.Entities;

public sealed class ConsultationNote : Entity
{
    internal ConsultationNote(Guid id, Guid consultationId, string text, bool isPrivate) : base(id)
    {
        ConsultationId = consultationId;
        Text = text;
        IsPrivate = isPrivate;
        CreatedAt = DateTime.UtcNow;
    }
    private ConsultationNote() { }

    public Guid ConsultationId { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public bool IsPrivate { get; private set; }
    public DateTime CreatedAt { get; private set; }
}

public sealed class ConsultationAttachment : Entity
{
    internal ConsultationAttachment(Guid id, Guid consultationId, string fileUrl, string fileName, string fileType) : base(id)
    {
        ConsultationId = consultationId;
        FileUrl = fileUrl;
        FileName = fileName;
        FileType = fileType;
        UploadedAt = DateTime.UtcNow;
    }
    private ConsultationAttachment() { }

    public Guid ConsultationId { get; private set; }
    public string FileUrl { get; private set; } = string.Empty;
    public string FileName { get; private set; } = string.Empty;
    public string FileType { get; private set; } = string.Empty;
    public DateTime UploadedAt { get; private set; }
}
