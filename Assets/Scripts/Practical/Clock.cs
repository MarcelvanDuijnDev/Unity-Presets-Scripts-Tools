using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [Header("0,0,0 = SystemTime / else Custom Time")]
    public Vector3 CurrentTime_Number;

    [Header("Info Only")]
    public float CurrentTime;
    public string CurrentTime_String24H;
    public string CurrentTime_String12H;


    public enum DirectionOptions { Right, Left, Forward, Back }
    [Header("Direction")]
    public DirectionOptions ArrowDirection;

    private bool _CustomTime;
    private Vector3 _Direction;
    private Vector3 _CurrentTime_NumberSmooth;

    [Header("Clock")]
    [SerializeField] private List<Clock_Arrow> Arrows = new List<Clock_Arrow>();

    private void Start()
    {
        if (CurrentTime == 0 && CurrentTime_Number == Vector3.zero)
            _CustomTime = false;
        else
        {
            _CustomTime = true;
            if (CurrentTime_Number != Vector3.zero)
                CurrentTime = CurrentTime_Number.z + (CurrentTime_Number.y * 60) + (CurrentTime_Number.x * 3600);
        }

        switch (ArrowDirection)
        {
            case DirectionOptions.Right:
                _Direction = Vector3.right;
                break;
            case DirectionOptions.Left:
                _Direction = Vector3.left;
                break;
            case DirectionOptions.Forward:
                _Direction = Vector3.forward;
                break;
            case DirectionOptions.Back:
                _Direction = Vector3.back;
                break;
        }
    }

    void Update()
    {
        if (!_CustomTime)
            CurrentTime = DateTime.Now.Second + (DateTime.Now.Minute * 60) + (DateTime.Now.Hour * 3600) + (DateTime.Now.Millisecond * 0.001f);
        else
            CurrentTime += 1 * Time.deltaTime;
        CurrentTime_Number = new Vector3(Mathf.Floor(CurrentTime / 3600), Mathf.Floor((CurrentTime / 60) % 60), Mathf.Floor(CurrentTime % 60));
        _CurrentTime_NumberSmooth = new Vector3(CurrentTime / 3600, (CurrentTime / 60) % 60, CurrentTime % 60);
        CurrentTime_String24H = string.Format("{0:00}:{1:00}:{2:00}", Mathf.Floor(CurrentTime / 3600), Mathf.Floor((CurrentTime / 60) % 60), CurrentTime % 60);

        CurrentTime_String12H = "";
        if (CurrentTime > 43200)
            CurrentTime_String12H = string.Format("{0:00}:{1:00} PM", Mathf.Floor(CurrentTime / 3600) - 12, Mathf.Floor((CurrentTime / 60) % 60));
        else
            CurrentTime_String12H = string.Format("{0:00}:{1:00} AM", Mathf.Floor(CurrentTime / 3600), Mathf.Floor((CurrentTime / 60) % 60));

        for (int i = 0; i < Arrows.Count; i++)
        {
            switch (Arrows[i].Type)
            {
                case Clock_Arrow.ArrowType.Hour:
                    if (Arrows[i].Smooth)
                        Arrows[i].ArrowObj.rotation = Quaternion.AngleAxis((360 / 12) * _CurrentTime_NumberSmooth.x, _Direction);
                    else
                        Arrows[i].ArrowObj.rotation = Quaternion.AngleAxis((360 / 12) * CurrentTime_Number.x, _Direction);
                    break;
                case Clock_Arrow.ArrowType.Minute:
                    if (Arrows[i].Smooth)
                        Arrows[i].ArrowObj.rotation = Quaternion.AngleAxis((360 / 60) * _CurrentTime_NumberSmooth.y, _Direction);
                    else
                        Arrows[i].ArrowObj.rotation = Quaternion.AngleAxis((360 / 60) * CurrentTime_Number.y, _Direction);
                    break;
                case Clock_Arrow.ArrowType.Second:
                    if (Arrows[i].Smooth)
                        Arrows[i].ArrowObj.rotation = Quaternion.AngleAxis((360 / 60) * _CurrentTime_NumberSmooth.z, _Direction);
                    else
                        Arrows[i].ArrowObj.rotation = Quaternion.AngleAxis((360 / 60) * CurrentTime_Number.z, _Direction);
                    break;
            }
        }
    }
}

[System.Serializable]
public class Clock_Arrow
{
    public enum ArrowType { Hour, Minute, Second }
    public ArrowType Type;
    public Transform ArrowObj;
    public bool Smooth;
}