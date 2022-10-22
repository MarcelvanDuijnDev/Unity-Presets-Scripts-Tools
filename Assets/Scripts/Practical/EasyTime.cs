using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyTime : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _Multiplier = 1;

    [Header("Info")]
    [SerializeField] private float _TotalTime;
    [SerializeField] private float _CurrentTime;
    [SerializeField] private Vector3 _TimeData;
    [SerializeField] private int _Day;

    void Update()
    {
        _CurrentTime += 1 * _Multiplier * Time.deltaTime;
        _TotalTime += 1 * _Multiplier * Time.deltaTime;
        _TimeData = new Vector3(_CurrentTime % 60, Mathf.Floor((_CurrentTime / 60) % 60), Mathf.Floor(_CurrentTime / 3600));


        if(_TimeData.z >= 24)
        {
            _Day++;
            _CurrentTime = 0;
        }
    }

    public float CurrentTime_Float
    {
        get { return _CurrentTime; }
        set { _CurrentTime = value; }
    }
    public float TotalTime_Flaot
    {
        get { return _TotalTime; }
        set { _TotalTime = value; }
    }
    public Vector3 CurrentTime_Vector4
    {
        get { return _TimeData; }
        set { _TimeData = value; }
    }
    public int CurrentDay
    {
        get { return _Day; }
        set { _Day = value; }
    }
}
