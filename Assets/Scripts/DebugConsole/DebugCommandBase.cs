using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCommandBase
{
    private string _CommandID;
    private string _CommandDescription;
    private string _CommandFormat;

    public string CommandID { get { return _CommandID; } }
    public string CommandDescription { get { return _CommandDescription; } }
    public string CommandFormat { get { return _CommandFormat; } }

    public DebugCommandBase(string id, string description, string format)
    {
        _CommandID = id;
        _CommandDescription = description;
        _CommandFormat = format;
    }
}

public class DebugCommand : DebugCommandBase
{
    private Action command;

    public DebugCommand(string id, string description, string format, Action command) : base (id, description, format)
    {
        this.command = command;
    }

    public void Invoke()
    {
        command.Invoke();
    }
}

public class DebugCommand<T1> : DebugCommandBase
{
    private Action<T1> command;

    public DebugCommand(string id, string description, string format, Action<T1> command) : base (id, description, format)
    {
        this.command = command;
    }

    public void Invoke(T1 value)
    {
        command.Invoke(value);
    }
}