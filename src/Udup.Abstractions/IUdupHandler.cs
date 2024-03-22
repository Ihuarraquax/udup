namespace Udup.Abstractions;

public interface IUdupHandler<T> : IUdupHandler where T : IUdupMessage;

public interface IUdupHandler;