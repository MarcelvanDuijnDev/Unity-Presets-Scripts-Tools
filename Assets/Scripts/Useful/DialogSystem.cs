using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DialogSystem : MonoBehaviour
{
    public List<DialogSystem_DialogTree> Dialog = new List<DialogSystem_DialogTree>();

    public Vector2Int CurrentID = new Vector2Int(0,0);
    public TextMeshProUGUI DialogText;

    public List<TextMeshProUGUI> OptionsText = new List<TextMeshProUGUI>();
    public List<GameObject> OptionsButtons = new List<GameObject>();

    public Button NextButton;

    void Start()
    {
        
    }

    void Update()
    {
        DialogText.text = Dialog[CurrentID.x].DialogTree[CurrentID.y].Dialog;



        //OptionsActive
        for (int i = 0; i < OptionsButtons.Count; i++)
        {
            if (Dialog[CurrentID.x].DialogTree[CurrentID.y].Options.Count != 0)
            {
                if (i < Dialog[CurrentID.x].DialogTree[CurrentID.y].Options.Count)
                {
                    OptionsButtons[i].SetActive(true);
                    OptionsText[i].text = Dialog[CurrentID.x].DialogTree[CurrentID.y].Options[i].OptionInfo;
                }
                else
                {
                    OptionsButtons[i].SetActive(false);
                }
            }
            else
                OptionsButtons[i].SetActive(false);
        }

        //NextButton
        if (Dialog[CurrentID.x].DialogTree[CurrentID.y].Options != null)
        {
            if (Dialog[CurrentID.x].DialogTree[CurrentID.y].Options.Count == 0)
                NextButton.enabled = true;
            else
                NextButton.enabled = false;
        }
        else
            NextButton.enabled = false;
    }

    public void ButtonInput(int id)
    {
        for (int i = 0; i < Dialog[CurrentID.x].DialogTree[CurrentID.y].Options[id].Options.Count; i++)
        {
            switch (Dialog[CurrentID.x].DialogTree[CurrentID.y].Options[id].Options[i].Option)
            {
                case DialogSystem_DialogOption.Options.GOTO:
                    CurrentID = Dialog[CurrentID.x].DialogTree[CurrentID.y].Options[id].Options[i].NextID;
                    break;
                case DialogSystem_DialogOption.Options.NEXT:
                    CurrentID.y++;
                    break;
            }
            break;
        }
    }

    public void Next()
    {
        CurrentID.y++;
    }
}

[System.Serializable]
public class DialogSystem_DialogTree
{
    public string DialogTreeInfo = "";
    public List<DialogSystem_Dialog> DialogTree = new List<DialogSystem_Dialog>();
}

[System.Serializable]
public class DialogSystem_Dialog
{
    public string Dialog = "";
    public List<DialogSystem_DialogOptions> Options = new List<DialogSystem_DialogOptions>();
}

[System.Serializable]
public class DialogSystem_DialogOptions
{
    public string OptionInfo = "";
    public List<DialogSystem_DialogOption> Options = new List<DialogSystem_DialogOption>();

    [HideInInspector] public bool OptionToggle = false;
}

[System.Serializable]
public class DialogSystem_DialogOption
{
    public enum Options {GOTO, NEXT};
    public Options Option;

    public Vector2Int NextID = new Vector2Int();
}