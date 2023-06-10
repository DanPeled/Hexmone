using System;
using UnityEngine;

public class DebugCommandBase
{
    private string _commandId;
    private string _commandDescription;
    private string _commandFormat;

    public string CommandId
    {
        get { return _commandId; }
    }
    public string CommandDescription
    {
        get { return _commandDescription; }
    }
    public string CommandFormat
    {
        get { return _commandFormat; }
    }

    public DebugCommandBase(string commandId, string description, string format)
    {
        _commandId = commandId;
        _commandDescription = description;
        _commandFormat = format;
    }
}

public class DebugCommand : DebugCommandBase
{
    private Action command;

    public DebugCommand(
        string commandId,
        string commandDescription,
        string commandFormat,
        Action command
    )
        : base(commandId, commandDescription, commandFormat)
    {
        this.command = command;
    }

    public void Invoke()
    {
        this.command.Invoke();
    }
}

public class DebugCommand<T1> : DebugCommandBase
{
    private Action<T1> command;

    public DebugCommand(string id, string description, string format, Action<T1> command)
        : base(id, description, format)
    {
        this.command = command;
    }

    public void Invoke(T1 value)
    {
        command.Invoke(value);
    }
}
