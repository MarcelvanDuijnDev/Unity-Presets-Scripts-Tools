using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

class Tool_UIEditor : EditorWindow
{
    int _Options_Type;
    int _Options_Style;

    [MenuItem("Tools/Tool_UIEditor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(Tool_UIEditor));
    }

    void OnGUI()
    {
        _Options_Type = GUILayout.Toolbar(_Options_Type, new string[] { "All" ,"Menu", "HUD" });

        if(_Options_Type == 0)
        {
            _Options_Style = GUILayout.Toolbar(_Options_Style, new string[] { "Default", "CSGO", "Overwatch", "Minecraft", "RocketLeague" });
        }
        else
        {
            _Options_Style = GUILayout.Toolbar(_Options_Style, new string[] { "Default", "CSGO", "Overwatch", "Minecraft", "RocketLeague" });
        }




        if (GUILayout.Button("Create"))
            CreateUI();

    }

    void CreateUI()
    {
        switch (_Options_Style)
        {
            case 0:
                CreateUI_Default();
                break;
        }
    }

    void CreateUI_Default()
    {
        GameObject canvasobj = new GameObject();
        canvasobj.name = "TestCanvas";
        canvasobj.AddComponent<Canvas>();
        canvasobj.AddComponent<CanvasScaler>();
        canvasobj.AddComponent<GraphicRaycaster>();

        GameObject eventsystemobj = new GameObject();
        eventsystemobj.name = "EventSytem";
        eventsystemobj.AddComponent<EventSystem>();
        eventsystemobj.AddComponent<StandaloneInputModule>();

        Canvas canvascomponent = canvasobj.GetComponent<Canvas>();
        canvascomponent.renderMode = RenderMode.ScreenSpaceCamera;

    }
}