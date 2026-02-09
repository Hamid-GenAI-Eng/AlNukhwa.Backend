using MediatR;
using System;

namespace Misan.Shared.Kernel.IntegrationEvents;

public record UserRegisteredEvent(Guid UserId, string Email, string Role) : INotification;
