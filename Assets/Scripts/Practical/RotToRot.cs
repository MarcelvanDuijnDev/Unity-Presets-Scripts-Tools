using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotToRot : MonoBehaviour
{
    [SerializeField] private Vector3 _TargetAngle = new Vector3(0f, 0f, 0f);

    [Header("Settings")]
    [SerializeField] private float _Speed = 1;
    [SerializeField] private bool _Lerp = true;
    [SerializeField] private bool _OnStart = false;

    private bool _Activated;

    private Vector3 _CurrentAngle;

    void Start()
    {
        _CurrentAngle = transform.localEulerAngles;

        if(_OnStart)
            StartRotating();
    }

    void Update()
    {
        if (_Activated)
        {
            if (_Lerp)
            {
                _CurrentAngle = new Vector3(
                    Mathf.LerpAngle(_CurrentAngle.x, _TargetAngle.x, _Speed * Time.deltaTime),
                    Mathf.LerpAngle(_CurrentAngle.y, _TargetAngle.y, _Speed * Time.deltaTime),
                    Mathf.LerpAngle(_CurrentAngle.z, _TargetAngle.z, _Speed * Time.deltaTime));
            }
            else
            {
                _CurrentAngle = Vector3.MoveTowards(_CurrentAngle, _TargetAngle, _Speed * Time.deltaTime);
            }
            transform.localEulerAngles = _CurrentAngle;
        }
    }

    public void StartRotating()
    {
        _Activated = true;
    }
}
