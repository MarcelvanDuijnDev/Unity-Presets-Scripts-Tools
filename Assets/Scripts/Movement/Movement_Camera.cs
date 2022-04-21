using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Camera : MonoBehaviour
{
    private enum CameraOptionsPos { None, Follow }
    private enum CameraOptionsRot { None, Follow }

    [Header("Options")]
    [SerializeField] private Transform _Target = null;
    [SerializeField] private CameraOptionsPos _CameraOptionPos = CameraOptionsPos.Follow;
    [SerializeField] private CameraOptionsRot _CameraOptionRot = CameraOptionsRot.Follow;

    [Header("Offset")]
    [SerializeField] private Vector3 _OffsetPosition = new Vector3(0, 12, -4);
    [SerializeField] private Vector3 _OffsetRotation = Vector3.zero;

    [Header("Settings")]
    [SerializeField] private float _Speed = 3;
    [SerializeField] private bool _LerpPosition = true;

    [Header("Contraints")]
    [SerializeField] private bool _LockAxis_X = false;
    [SerializeField] private bool _LockAxis_Y = false;
    [SerializeField] private bool _LockAxis_Z = false;

    private Vector3 _TargetPosition;
    private float _ScreenShakeDuration;
    private float _ScreenShakeIntensity;

    //CutScenes
    private bool _CutScene;
    private GameObject _CutSceneTarget;
    private Vector3 _CutScenePosition;
    private Vector3 _CutSceneRotation;
    private bool _CutSceneFollow;

    public static Movement_Camera CAM;

    private void Awake()
    {
        CAM = this;
    }

    void Update()
    {
        //Update Target Location
        float x_axis = _Target.transform.position.x + _OffsetPosition.x;
        float y_axis = _Target.transform.position.y + _OffsetPosition.y;
        float z_axis = _Target.transform.position.z + _OffsetPosition.z;

        if (!_CutScene)
        {
            //Lock Axis
            if (_LockAxis_X)
                x_axis = _OffsetPosition.x;
            if (_LockAxis_Y)
                y_axis = _OffsetPosition.y;
            if (_LockAxis_Z)
                z_axis = _OffsetPosition.z;

            _TargetPosition = new Vector3(x_axis, y_axis, z_axis);

            //Movement
            switch (_CameraOptionPos)
            {
                case CameraOptionsPos.Follow:
                    if (_LerpPosition)
                        transform.position = Vector3.Lerp(transform.position, _TargetPosition, _Speed * Time.deltaTime);
                    else
                        transform.position = Vector3.MoveTowards(transform.position, _TargetPosition, _Speed * Time.deltaTime);
                    break;
            }

            //ScreenShake
            if (_ScreenShakeDuration > 0)
            {
                transform.localPosition = new Vector3(transform.position.x + Random.insideUnitSphere.x * _ScreenShakeIntensity, transform.position.y + Random.insideUnitSphere.y * _ScreenShakeIntensity, transform.position.z);
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
        else //CutScene
        {
            //Position
            _TargetPosition = _CutScenePosition;

            if (_LerpPosition)
                transform.position = Vector3.Lerp(transform.position, _TargetPosition, _Speed * Time.deltaTime);
            else
                transform.position = Vector3.MoveTowards(transform.position, _TargetPosition, _Speed * Time.deltaTime);

            //Target
            if (_CutSceneFollow)
            {
                Vector3 rpos = _CutSceneTarget.transform.position - transform.position;
                Quaternion lookrotation = Quaternion.LookRotation(rpos, Vector3.up);
                transform.eulerAngles = new Vector3(lookrotation.eulerAngles.x + _CutSceneRotation.x, lookrotation.eulerAngles.y + _CutSceneRotation.y, lookrotation.eulerAngles.z + _CutSceneRotation.z);
            }
            else
                transform.eulerAngles = _CutSceneRotation;
        }
    }

    //Effects
    public void Effect_ScreenShake(float duration, float intesity)
    {
        _ScreenShakeDuration = duration;
        _ScreenShakeIntensity = intesity;
    }

    //GetSet
    public Transform CameraTarget
    {
        get { return _Target; }
        set { _Target = value; }
    }
    public Vector3 Camera_OffSetPosition
    {
        get { return _OffsetPosition; }
        set { _OffsetPosition = value; }
    }
    public Vector3 Camera_OffSetRotation
    {
        get { return _OffsetRotation; }
        set { _OffsetRotation = value; }
    }

    //CutScene
    public void CutScene(bool startcutscene)
    {
        _CutScene = false;
    }
    public void CutScene(bool startcutscene, Vector3 cutscenepos, GameObject targetobj, Vector3 cutscenerot, bool follow)
    {
        _CutScene = startcutscene;
        _CutScenePosition = cutscenepos;
        _CutSceneTarget = targetobj;
        _CutSceneRotation = cutscenerot;
        _CutSceneFollow = follow;
    }
}
