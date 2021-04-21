using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    private bool _ShowConsole;
    private bool _ShowHelp;
    private string _Input;
    private Vector2 _Scroll;

    public static DebugCommand TEST;
    public static DebugCommand HELP;
    public static DebugCommand HIDEHELP;
    public static DebugCommand<float> SETVALUE;

    public List<object> commandList;

    private void Awake()
    {
        HELP = new DebugCommand("help", "Shows a list of commands", "help", () =>
        {
            _ShowHelp = !_ShowHelp;
        });

        HIDEHELP = new DebugCommand("hidehelp", "hide help info", "hidehelp", () =>
        {
            _ShowHelp = false;
        });

        TEST = new DebugCommand("test", "example command", "test", () =>
        {
            Debug.Log("test command triggered");
        });

        SETVALUE = new DebugCommand<float>("setvalue", "example set value", "setvalue <value>", (x) =>
        {
            Debug.Log("Value added: " + x.ToString());
        });

        commandList = new List<object>
        {         
            HELP,
            HIDEHELP,
            TEST,
            SETVALUE
        };
    }

    private void OnGUI()
    {
        //Check input
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F1)
        {
            _ShowConsole = !_ShowConsole;
        }

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return && _ShowConsole)
        {
            HandleInput();
            _Input = "";
        }

        //Console active
        if (!_ShowConsole) return;

        GUI.FocusControl("FOCUS");
        
        float y = 0f;

        if(_ShowHelp)
        {
            GUI.Box(new Rect(0, y, Screen.width, 100), "");
            Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * commandList.Count);
            _Scroll = GUI.BeginScrollView(new Rect(0, y + 5, Screen.width, 90), _Scroll, viewport);

            for (int i=0; i<commandList.Count; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;
                string label = $"{command.CommandFormat} - {command.CommandDescription}";
                Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);
                GUI.Label(labelRect, label);
            }

            GUI.EndScrollView();
            y += 100;
        }

        GUI.Box(new Rect(0, y, Screen.width, 30), "");

        GUI.backgroundColor = new Color(0,0,0,0);
        GUI.SetNextControlName("FOCUS");
        _Input = GUI.TextField(new Rect(10, y + 5, Screen.width - 20, 20), _Input);
    }

    private void HandleInput()
    {
        string[] properties = _Input.Split(' ');

        for(int i=0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            if (_Input.Contains(commandBase.CommandID))
            {
                if (commandList[i] as DebugCommand != null)
                    (commandList[i] as DebugCommand).Invoke();
                else if (commandList[i] as DebugCommand<int> != null && properties.Length > 1)
                    if (CheckInput(properties[1]))
                        (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
            }
        }
    }

    private bool CheckInput(string str)
    {
        foreach (char c in str)
        {
            if (c < '0' || c > '9')
                return false;
        }
        return true;
    }
}