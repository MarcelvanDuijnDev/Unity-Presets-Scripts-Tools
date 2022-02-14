using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _Speed = 1;
    [SerializeField] private float _Distance = 20;

    [Header("Offset")]
    [SerializeField] private Vector3 _RotationOffset = Vector3.zero;
    [SerializeField] private bool _SetCurrentRotationAsOffset = true;

    enum AxisOptions { X, Y, Z }
    [SerializeField] private AxisOptions _Axis = AxisOptions.X;

    private void Start()
    {
        if (_SetCurrentRotationAsOffset)
            _RotationOffset = transform.eulerAngles;
    }

    void Update()
    {
        float angle = _Distance * Mathf.Sin(Time.time * _Speed);
        switch (_Axis)
        {
            case AxisOptions.X:
                transform.localRotation = Quaternion.Euler(_RotationOffset.x + angle, _RotationOffset.y, _RotationOffset.z);
                break;
            case AxisOptions.Y:
                transform.localRotation = Quaternion.Euler(_RotationOffset.x, _RotationOffset.y + angle, _RotationOffset.z);
                break;
            case AxisOptions.Z:
                transform.localRotation = Quaternion.Euler(_RotationOffset.x, _RotationOffset.y, _RotationOffset.z + angle);
                break;
        }
    }
}