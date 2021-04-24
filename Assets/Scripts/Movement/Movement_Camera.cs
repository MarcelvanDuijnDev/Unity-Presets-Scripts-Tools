﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Camera : MonoBehaviour
{
    private enum CameraOptionsPos { None, Follow }
    private enum CameraOptionsRot { None, Follow }

    [Header("Options")]
    [SerializeField] private CameraOptionsPos _CameraOptionPos = CameraOptionsPos.Follow;
    [SerializeField] private CameraOptionsRot _CameraOptionRot = CameraOptionsRot.Follow;

    [Header("Settings - Position")]
    [SerializeField] private Vector3 _OffsetPosition = new Vector3(0,12,-4);

    [Header("Settings - Rotation")]
    [SerializeField] private Vector3 _OffsetRotation = Vector3.zero;

    [Header("Settings")]
    [SerializeField] private float _Speed = 1000;

    [Header("Other")]
    [SerializeField] private Transform _Target = null;
    [SerializeField] private bool _LockAxis_X = false;
    [SerializeField] private bool _LockAxis_Y = false;
    [SerializeField] private bool _LockAxis_Z = false;

    private Vector3 _TargetPosition;
    private float _ScreenShakeDuration;
    private float _ScreenShakeIntensity;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            Effect_ScreenShake(3, 0.5f);
        }

        //Update Target Location
        float x_axis = _Target.transform.position.x + _OffsetPosition.x;
        float y_axis = _Target.transform.position.y + _OffsetPosition.y;
        float z_axis = _Target.transform.position.z + _OffsetPosition.z;

        if (_LockAxis_X)
            x_axis = _OffsetPosition.x;
        if (_LockAxis_Y)
            y_axis = _OffsetPosition.y;
        if (_LockAxis_Z)
            z_axis = _OffsetPosition.z;

        _TargetPosition = new Vector3(x_axis, y_axis, z_axis);

        // Movement
        switch (_CameraOptionPos)
        {
            case CameraOptionsPos.Follow:
                transform.position = Vector3.MoveTowards(transform.position, _TargetPosition, _Speed * Time.deltaTime);
                break;
        }

        //ScreenShake
        if(_ScreenShakeDuration > 0)
        {
            transform.localPosition = new Vector3(_TargetPosition.x + Random.insideUnitSphere.x * _ScreenShakeIntensity, _TargetPosition.y + Random.insideUnitSphere.y * _ScreenShakeIntensity, _TargetPosition.z);
            _ScreenShakeDuration -= 1 * Time.deltaTime;
        }
        else
        {
            // Rotation
            switch (_CameraOptionRot)
            {
                case CameraOptionsRot.Follow:
                    Vector3 rpos = _Target.position - transform.position;
                    Quaternion lookrotation = Quaternion.LookRotation(rpos, Vector3.up);
                    transform.eulerAngles = new Vector3(lookrotation.eulerAngles.x + _OffsetRotation.x, lookrotation.eulerAngles.y + _OffsetRotation.y, lookrotation.eulerAngles.z + _OffsetRotation.z);
                    break;
            }
        }
    }

    //Effects
    public void Effect_ScreenShake(float duration, float intesity)
    {
        _ScreenShakeDuration = duration;
        _ScreenShakeIntensity = intesity;
    }

    //Set
    public void Set_CameraTarget(GameObject targetobj)
    {
        _Target = targetobj.transform;
    }
    public void Set_OffSet(Vector3 offset)
    {
        _OffsetPosition = offset;
    }
}
