﻿namespace Udup;

public interface IUdupHandler<T> : IUdupHandler where T : IUdupMessage;
public interface IUdupHandler;
public interface IUdupMessage;