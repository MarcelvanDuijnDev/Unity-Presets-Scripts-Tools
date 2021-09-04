using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogSystem : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private TextMeshProUGUI _DialogText;
    [SerializeField] private Button _NextButton;

    [Header("Ref Options")]
    [SerializeField] private List<TextMeshProUGUI> _OptionsText = new List<TextMeshProUGUI>();
    [SerializeField] private List<GameObject> _OptionsButtons = new List<GameObject>();

    [Header("Dialog")]
    public DialogSystem_File _Dialog;

    //Private variables
    private Vector2Int _CurrentID = new Vector2Int(0, 0);
    private bool _Finished;

    void Update()
    {
        _DialogText.text = _Dialog.DialogTree[_CurrentID.x].Dialog[_CurrentID.y].Dialog;

        // Option Buttons 
        for (int i = 0; i < _OptionsButtons.Count; i++)
        {
            if (_Dialog.DialogTree[_CurrentID.x].Dialog[_CurrentID.y].Options.Count != 0)
            {
                if (i < _Dialog.DialogTree[_CurrentID.x].Dialog[_CurrentID.y].Options.Count)
                {
                    _OptionsButtons[i].SetActive(true);
                    _OptionsText[i].text = _Dialog.DialogTree[_CurrentID.x].Dialog[_CurrentID.y].Options[i].OptionInfo;
                }
                else
                    _OptionsButtons[i].SetActive(false);
            }
            else
                _OptionsButtons[i].SetActive(false);
        }

        // NextButton
        if (_Dialog.DialogTree[_CurrentID.x].Dialog[_CurrentID.y].Options != null)
        {
            if (_Dialog.DialogTree[_CurrentID.x].Dialog[_CurrentID.y].Options.Count == 0)
                _NextButton.gameObject.SetActive(true);
            else
                _NextButton.gameObject.SetActive(false);
        }
        else
            _NextButton.gameObject.SetActive(false);

        //Done
        if(_Finished)
        {
            _NextButton.gameObject.SetActive(false);
            for (int i = 0; i < _OptionsButtons.Count; i++)
            {
                _OptionsButtons[i].SetActive(false);
            }
            _DialogText.text = "Finished";
        }
    }

    public void ButtonInput(int id)
    {
        for (int i = 0; i < _Dialog.DialogTree[_CurrentID.x].Dialog[_CurrentID.y].Options[id].Options.Count; i++)
        {
            switch (_Dialog.DialogTree[_CurrentID.x].Dialog[_CurrentID.y].Options[id].Options[i].Option)
            {
                case DialogSystem_DialogOption.Options.GOTO:
                    _CurrentID = _Dialog.DialogTree[_CurrentID.x].Dialog[_CurrentID.y].Options[id].Options[i].NextID;
                    break;
                case DialogSystem_DialogOption.Options.NEXT:
                    _CurrentID.y++;
                    break;
                case DialogSystem_DialogOption.Options.FINISHED:
                    _Finished = true;
                    break;
            }
        }
    }

    public void Next()
    {
        _CurrentID.y++;
    }
}

[System.Serializable]
public class DialogSystem_File
{
    public string DialogNameID;
    public List<DialogSystem_DialogTree> DialogTree = new List<DialogSystem_DialogTree>();
}

[System.Serializable]
public class DialogSystem_DialogTree
{
    public string DialogTreeInfo = "";
    public List<DialogSystem_Dialog> Dialog = new List<DialogSystem_Dialog>();
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
    //Options
    public enum Options {GOTO, NEXT, FINISHED};
    public Options Option;

    //EventData
    public Vector2Int NextID = new Vector2Int();
}