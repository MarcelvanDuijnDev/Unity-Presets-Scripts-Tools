using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorState : MonoBehaviour
{

    public enum CursorStates {Free_Show,Free_NotShow, Locked_Show, Locked_NotShow }
    [SerializeField] private CursorStates _CursorState;

    void Start()
    {
        switch(_CursorState)
        {
            case CursorStates.Free_Show:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case CursorStates.Free_NotShow:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
                break;
            case CursorStates.Locked_Show:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = true;
                break;
            case CursorStates.Locked_NotShow:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
        }
        
    }
}
