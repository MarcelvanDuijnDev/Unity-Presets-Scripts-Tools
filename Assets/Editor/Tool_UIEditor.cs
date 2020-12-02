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
    GameObject _CreatedCanvas;

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

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create"))
            CreateUI();
        if (GUILayout.Button("Delete"))
            if(_CreatedCanvas != null)
            {
                DestroyImmediate(_CreatedCanvas);
                _CreatedCanvas = null;

            }
        GUILayout.EndHorizontal();

        GUILayout.Label("Info:");
        _CreatedCanvas = (GameObject)EditorGUILayout.ObjectField("Created object", _CreatedCanvas, typeof(GameObject), true);

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
        //Add canvas
        GameObject canvasobj = CreateCanvas();

        //Add setting tabs
        GameObject tab_display = Create_Tab(canvasobj, "Display");
        GameObject tab_graphics = Create_Tab(canvasobj, "Graphics");
        GameObject tab_gameplay = Create_Tab(canvasobj, "Gameplay");
        GameObject tab_controls = Create_Tab(canvasobj, "Controls");
        GameObject tab_keybinding = Create_Tab(canvasobj, "Keybinding");

        //Add Buttons
        GameObject main = new GameObject();
        RectTransform mainrect = main.AddComponent<RectTransform>();
        SetRect(mainrect, "bottomleft");
        GameObject button_start = Create_Button(main, "Button_Start", "Start", new Vector2(40,450), new Vector2(700,100), 30, 60, "bottomleft");
        GameObject button_options = Create_Button(main, "Button_Options", "Options", new Vector2(40, 330), new Vector2(700, 100), 30, 60, "bottomleft");
        GameObject button_quit = Create_Button(main, "Button_Quit", "Quit", new Vector2(40, 210), new Vector2(700, 100), 30, 60, "bottomleft");
        main.name = "Main";
        main.transform.SetParent(canvasobj.transform);

        _CreatedCanvas = canvasobj;
    }

    GameObject CreateCanvas()
    {
        GameObject canvasobj = new GameObject();
        canvasobj.name = "TestCanvas";
        canvasobj.AddComponent<Canvas>();
        CanvasScaler canvasscale = canvasobj.AddComponent<CanvasScaler>();
        canvasscale.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasscale.referenceResolution = new Vector2(1920, 1080);
        canvasobj.AddComponent<GraphicRaycaster>();

        if (GameObject.Find("EventSystem") == null)
        {
            GameObject eventsystemobj = new GameObject();
            eventsystemobj.name = "EventSytem";
            eventsystemobj.AddComponent<EventSystem>();
            eventsystemobj.AddComponent<StandaloneInputModule>();
        }

        Canvas canvascomponent = canvasobj.GetComponent<Canvas>();
        canvascomponent.renderMode = RenderMode.ScreenSpaceCamera;
        
        return canvasobj;
    }
    GameObject Create_Button(GameObject parentobj ,string name, string buttontext, Vector2 pos, Vector2 size, float textoffset, float textsize, string anchorpos)
    {
        GameObject buttontemplate = new GameObject();
        RectTransform buttontransform = buttontemplate.AddComponent<RectTransform>();

        buttontransform.sizeDelta = size;
        buttontransform.anchoredPosition = pos;

        SetRect(buttontransform, anchorpos);

        buttontemplate.AddComponent<CanvasRenderer>();
        Image buttonimage = buttontemplate.AddComponent<Image>();
        buttonimage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        buttonimage.type = Image.Type.Sliced;
        Button buttonbutton = buttontemplate.AddComponent<Button>();
        buttonbutton.targetGraphic = buttonimage;
        buttontemplate.name = name;

        GameObject buttontextemplate = new GameObject();
        RectTransform buttontextrect = buttontextemplate.AddComponent<RectTransform>();

        buttontextrect.anchoredPosition = new Vector2(pos.x + textoffset, pos.y);
        buttontextrect.sizeDelta = size;
        buttontextrect.pivot = new Vector2(0,0);

        TextMeshProUGUI buttontexttmpro = buttontextemplate.AddComponent<TextMeshProUGUI>();
        buttontexttmpro.text = buttontext;
        buttontexttmpro.fontSize = textsize;
        buttontexttmpro.alignment = TextAlignmentOptions.MidlineLeft;
        buttontexttmpro.color = Color.black;

        buttontextemplate.name = name + "text";



        buttontextemplate.transform.SetParent(buttontemplate.transform);

        buttontemplate.transform.SetParent(parentobj.transform);

        return buttontemplate;
    }
    GameObject Create_Dropdown(GameObject parentobj, string name, string buttontext, Vector2 pos, Vector2 size, float textoffset, float textsize, string anchorpos)
    {
        // DropDown Begin
        GameObject dropdowntemplate = new GameObject();
        RectTransform buttontransform = dropdowntemplate.AddComponent<RectTransform>();
        buttontransform.sizeDelta = size;
        buttontransform.anchoredPosition = pos;
        SetRect(buttontransform, anchorpos);
        dropdowntemplate.AddComponent<CanvasRenderer>();
        Image buttonimage = dropdowntemplate.AddComponent<Image>();
        buttonimage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        buttonimage.type = Image.Type.Sliced;
        Dropdown buttonbutton = dropdowntemplate.AddComponent<Dropdown>();
        buttonbutton.targetGraphic = buttonimage;
        dropdowntemplate.name = name;
        // DropDown End

        // Label Begin
        GameObject dropdown_label = Create_Text(dropdowntemplate, "Text_DropDown", pos, size, "Option A", textsize, Color.black);
        // Label End

        // Arrow Begin
        GameObject dropdown_arrow = new GameObject();
        RectTransform dropdown_arrowrect = dropdown_arrow.AddComponent<RectTransform>();
        SetRect(dropdown_arrowrect, "rightmiddle");
        dropdown_arrowrect.position = new Vector2(0,0);
        dropdown_arrow.AddComponent<CanvasRenderer>();
        Image dropdown_arrowimage = dropdown_arrow.AddComponent<Image>();
        dropdown_arrowimage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/DropdownArrow.psd");
        // Arrow End



        dropdown_label.transform.SetParent(dropdowntemplate.transform);
        dropdown_arrow.transform.SetParent(dropdowntemplate.transform);

        dropdowntemplate.transform.SetParent(parentobj.transform);

        return dropdowntemplate;
    }
    GameObject Create_Text(GameObject parentobj, string name, Vector2 pos, Vector2 size, string textcontent, float fontsize, Color textcolor)
    {
        GameObject newtextobj = new GameObject();
        RectTransform buttontextrect = newtextobj.AddComponent<RectTransform>();
        newtextobj.name = name;
        buttontextrect.sizeDelta = size;
        buttontextrect.anchoredPosition = pos;
        SetRect(buttontextrect, "bottomleft");

        TextMeshProUGUI newtext = newtextobj.AddComponent<TextMeshProUGUI>();
        newtext.text = textcontent;
        newtext.fontSize = fontsize;
        newtext.alignment = TextAlignmentOptions.MidlineLeft;
        newtext.color = textcolor;

        newtext.transform.SetParent(parentobj.transform);
        return newtextobj;
    }
    GameObject Create_Slider(GameObject parentobj, string name, string buttontext, Vector2 pos, Vector2 size, float textoffset, float textsize, string anchorpos)
    {

        GameObject newobj = new GameObject();
        return newobj;
    }
    GameObject Create_Tab(GameObject parentobj, string name)
    {
        GameObject tab_new = new GameObject();
        RectTransform tab_newrect = tab_new.AddComponent<RectTransform>();
        SetRect(tab_newrect, "bottomleft");
        tab_new.name = name;

        GameObject textobj = Create_Text(tab_new, "Title_" + name, new Vector2(800,800), new Vector2(1000,200), name, 100, Color.white);

        switch (name)
        {
            case "Display":
                GameObject button_Resolution = Create_Dropdown(tab_new, "Button_Resolution", "Resolution", new Vector2(800, 700), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_Fullscreen = Create_Button(tab_new, "Button_Fullscreen", "Fullscreen", new Vector2(800, 630), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_Quality = Create_Button(tab_new, "Button_Quality", "Quality", new Vector2(800, 560), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_VSync = Create_Button(tab_new, "Button_VSync", "VSync", new Vector2(800, 490), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_MaxFPS = Create_Button(tab_new, "Button_MaxFPS", "MaxFPS", new Vector2(800, 420), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_Gamma = Create_Button(tab_new, "Button_Gamma", "Gamma", new Vector2(800, 350), new Vector2(500, 60), 10, 40, "bottomleft");
                // Resolution
                // Fullscreen
                // Quality
                // V-Sync
                // Max fps
                // Gamma
                break;
            case "Graphics":
                GameObject button_Antialiasing = Create_Button(tab_new, "Button_Antialiasing", "Antialiasing", new Vector2(800, 700), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_Shadows = Create_Button(tab_new, "Button_Shadows", "Shadows", new Vector2(800, 630), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_ViewDistance = Create_Button(tab_new, "Button_ViewDistance", "ViewDistance", new Vector2(800, 560), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_TextureQuality = Create_Button(tab_new, "Button_TextureQuality", "TextureQuality", new Vector2(800, 490), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_ViolageDistance = Create_Button(tab_new, "Button_ViolageDistance", "ViolageDistance", new Vector2(800, 420), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_ViolageDensity = Create_Button(tab_new, "Button_ViolageDensity", "ViolageDensity", new Vector2(800, 350), new Vector2(500, 60), 10, 40, "bottomleft");
                // Antialiasing
                // Shadows
                // ViewDistance
                // TextureQuality
                // ViolageDistance
                // ViolageDensity
                break;
            case "Gameplay":
                GameObject button_SoundEffects = Create_Button(tab_new, "Button_SoundEffects", "SoundEffects", new Vector2(800, 700), new Vector2(500, 60), 10, 40, "bottomleft");
                GameObject Button_Music = Create_Button(tab_new, "Button_Music", "Music", new Vector2(800, 630), new Vector2(500, 60), 10, 40, "bottomleft");
                // SoundEffects
                // Music
                break;
            case "Controls":

                break;
            case "Keybinding":

                break;
        }


        tab_new.transform.SetParent(parentobj.transform);
        tab_new.SetActive(false);
        return tab_new;
    }
    

    void SetRect(RectTransform rect, string anchorpos)
    {

        switch (anchorpos)
        {
            case "topleft":
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
                break;
            case "topmiddle":
                rect.anchorMin = new Vector2(0.5f, 1);
                rect.anchorMax = new Vector2(0.5f, 1);
                rect.pivot = new Vector2(0.5f, 1);
                break;
            case "topright":
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1, 1);
                break;
            case "rightmiddle":
                rect.anchorMin = new Vector2(1, 0.5f);
                rect.anchorMax = new Vector2(1, 0.5f);
                rect.pivot = new Vector2(1, 0.5f);
                break;
            case "bottomright":
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(1, 0);
                break;
            case "bottommiddle":
                rect.anchorMin = new Vector2(0.5f, 0);
                rect.anchorMax = new Vector2(0.5f, 0);
                rect.pivot = new Vector2(0.5f, 0);
                break;
            case "bottomleft":
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 0);
                rect.pivot = new Vector2(0, 0);
                break;
            case "leftmiddle":
                rect.anchorMin = new Vector2(0, 0.5f);
                rect.anchorMax = new Vector2(0, 0.5f);
                rect.pivot = new Vector2(0, 0.5f);
                break;
            case "middle":
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                break;
        }
    }
}