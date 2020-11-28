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
        CanvasScaler canvasscale = canvasobj.AddComponent<CanvasScaler>();
        canvasscale.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasscale.referenceResolution = new Vector2(1920,1080);
        canvasobj.AddComponent<GraphicRaycaster>();

        GameObject eventsystemobj = new GameObject();
        eventsystemobj.name = "EventSytem";
        eventsystemobj.AddComponent<EventSystem>();
        eventsystemobj.AddComponent<StandaloneInputModule>();

        Canvas canvascomponent = canvasobj.GetComponent<Canvas>();
        canvascomponent.renderMode = RenderMode.ScreenSpaceCamera;



        //Add Buttons
        GameObject button_start = Create_Button(canvasobj ,"Button_Start", "Start", new Vector2(40,450), new Vector2(700,100), 30, "bottomleft");
        GameObject button_options = Create_Button(canvasobj ,"Button_Options", "Options", new Vector2(40, 330), new Vector2(700, 100), 30, "bottomleft");
        GameObject button_quit = Create_Button(canvasobj ,"Button_Quit", "Quit", new Vector2(40, 210), new Vector2(700, 100), 30, "bottomleft");
    }

    GameObject Create_Button(GameObject canvas ,string name, string buttontext, Vector2 pos, Vector2 size, float textoffset, string anchorpos)
    {
        GameObject buttontemplate = new GameObject();
        DestroyImmediate(buttontemplate.GetComponent<Transform>());
        RectTransform buttontransform = buttontemplate.AddComponent<RectTransform>();

        buttontransform.sizeDelta = size;

        buttontransform.anchoredPosition = pos;

        switch (anchorpos)
        {
            case "topleft":
                buttontransform.anchorMin = new Vector2(0, 1);
                buttontransform.anchorMax = new Vector2(0, 1);
                buttontransform.pivot = new Vector2(0, 1);
                break;
            case "topmiddle":
                buttontransform.anchorMin = new Vector2(0.5f, 1);
                buttontransform.anchorMax = new Vector2(0.5f, 1);
                buttontransform.pivot = new Vector2(0.5f, 1);
                break;
            case "topright":
                buttontransform.anchorMin = new Vector2(1, 1);
                buttontransform.anchorMax = new Vector2(1, 1);
                buttontransform.pivot = new Vector2(1, 1);
                break;
            case "rightmiddle":
                buttontransform.anchorMin = new Vector2(1, 0.5f);
                buttontransform.anchorMax = new Vector2(1, 0.5f);
                buttontransform.pivot = new Vector2(1, 0.5f);
                break;
            case "bottomright":
                buttontransform.anchorMin = new Vector2(1, 0);
                buttontransform.anchorMax = new Vector2(1, 0);
                buttontransform.pivot = new Vector2(1, 0);
                break;
            case "bottommiddle":
                buttontransform.anchorMin = new Vector2(0.5f, 0);
                buttontransform.anchorMax = new Vector2(0.5f, 0);
                buttontransform.pivot = new Vector2(0.5f, 0);
                break;
            case "bottomleft":
                buttontransform.anchorMin = new Vector2(0, 0);
                buttontransform.anchorMax = new Vector2(0, 0);
                buttontransform.pivot = new Vector2(0, 0);
                break;
            case "leftmiddle":
                buttontransform.anchorMin = new Vector2(0, 0.5f);
                buttontransform.anchorMax = new Vector2(0, 0.5f);
                buttontransform.pivot = new Vector2(0, 0.5f);
                break;
            case "middle":
                buttontransform.anchorMin = new Vector2(0.5f, 0.5f);
                buttontransform.anchorMax = new Vector2(0.5f, 0.5f);
                buttontransform.pivot = new Vector2(0.5f, 0.5f);
                break;
        }


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
        buttontexttmpro.fontSize = 60;
        buttontexttmpro.alignment = TextAlignmentOptions.MidlineLeft;
        buttontexttmpro.color = Color.black;

        buttontextemplate.name = name + "text";



        buttontextemplate.transform.parent = buttontemplate.transform;

        buttontemplate.transform.parent = canvas.transform;

        return buttontemplate;
    }
}