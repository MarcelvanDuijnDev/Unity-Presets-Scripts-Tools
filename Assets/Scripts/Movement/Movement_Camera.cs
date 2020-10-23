using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Camera : MonoBehaviour
{
    private enum CameraOptionsPos { None, Follow }
    private enum CameraOptionsRot { None, Follow }

    [Header("Options")]
    [SerializeField] private CameraOptionsPos _CameraOptionPos = CameraOptionsPos.Follow;
    [SerializeField] private CameraOptionsRot _CameraOptionRot = CameraOptionsRot.Follow;
    [Header("Settings Position")]
    [SerializeField] private Vector3 _OffsetPosition = new Vector3(0,12,-4);
    [SerializeField] private bool _UseOffsetYAsDefaultHeight = true;
    [Header("Settings Rotation")]
    [SerializeField] private Vector3 _OffsetRotation;

    [Header("Other")]
    [SerializeField] private Transform _Target;

    void Update()
    {
        // Movement
        switch(_CameraOptionPos)
        {
            case CameraOptionsPos.Follow:
                if (_UseOffsetYAsDefaultHeight)
                    transform.position = new Vector3(_Target.position.x + _OffsetPosition.x, _OffsetPosition.y, _Target.position.z + _OffsetPosition.z);
                else
                    transform.position = new Vector3(_Target.position.x + _OffsetPosition.x, _Target.position.y + _OffsetPosition.y, _Target.position.z + _OffsetPosition.z);
                break;
        }
        // Rotation
        switch(_CameraOptionRot)
        {
            case CameraOptionsRot.Follow:
                Vector3 rpos = _Target.position - transform.position;
                Quaternion lookrotation = Quaternion.LookRotation(rpos, Vector3.up);
                transform.eulerAngles = new Vector3(lookrotation.eulerAngles.x + _OffsetRotation.x, lookrotation.eulerAngles.y + _OffsetRotation.y, lookrotation.eulerAngles.z + _OffsetRotation.z);
                break;
        }
    }
}
