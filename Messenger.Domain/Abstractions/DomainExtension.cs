namespace Messenger.Domain.Abstractions;

public class DomainException(string message) : Exception(message);
