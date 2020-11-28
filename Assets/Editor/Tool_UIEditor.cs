using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

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

        GameObject button_start = Create_Button("Button_Start", "Start", new Vector2(40,45.5f), new Vector2(200,30), 10);



        button_start.transform.parent = canvasobj.transform;
    }

    GameObject Create_Button(string name, string buttontext, Vector2 pos, Vector2 size, float textoffset)
    {
        GameObject buttontemplate = new GameObject();
        DestroyImmediate(buttontemplate.GetComponent<Transform>());
        RectTransform buttontransform = buttontemplate.AddComponent<RectTransform>();

        buttontransform.sizeDelta = size;

        buttontransform.anchoredPosition = pos;
        buttontransform.pivot = new Vector2(0,0);
        buttontransform.anchorMin = new Vector2(0,0);
        buttontransform.anchorMax = new Vector2(0,0);

        buttontemplate.AddComponent<CanvasRenderer>();
        Image buttonimage = buttontemplate.AddComponent<Image>();
        buttonimage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        buttonimage.type = Image.Type.Sliced;
        Button buttonbutton = buttontemplate.AddComponent<Button>();
        buttonbutton.targetGraphic = buttonimage;
        buttontemplate.name = name;

        GameObject buttontextemplate = new GameObject();
        DestroyImmediate(buttontextemplate.GetComponent<Transform>());
        RectTransform buttontextrect = buttontextemplate.AddComponent<RectTransform>();

        buttontextrect.anchoredPosition = new Vector2(pos.x + textoffset, pos.y);
        buttontextrect.sizeDelta = size;
        buttontextrect.pivot = new Vector2(0,0);

        TextMeshProUGUI buttontexttmpro = buttontextemplate.AddComponent<TextMeshProUGUI>();
        buttontexttmpro.text = buttontext;
        buttontexttmpro.fontSize = 15;
        buttontexttmpro.alignment = TextAlignmentOptions.MidlineLeft;
        buttontexttmpro.color = Color.black;

        buttontextemplate.name = name + "text";



        buttontextemplate.transform.parent = buttontemplate.transform;

        return buttontemplate;
    }
}