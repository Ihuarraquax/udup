using MediatR;
using Udup.Abstractions;

namespace Udup.WebApp;


public record DomainEventAHappened : INotification, IUdupMessage;


public record DomainEventBHappened : INotification, IUdupMessage;


public record DomainEventCHappened : INotification, IUdupMessage;


public record DomainEventDHappened : INotification, IUdupMessage;


