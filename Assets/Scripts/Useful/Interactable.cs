using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    public enum InteractableType { Move, Door, SetLight, SetLightNegative, Lever, Button, Item, UIButton }
    public InteractableType _Type;

    private enum AxisOptions { x, y, z }
    [SerializeField] private AxisOptions _AxisOption = AxisOptions.x;

    [SerializeField] private bool _InvertMouse = false;

    [Header("Type - Light")]
    [SerializeField] private GameObject _Light = null;
    [SerializeField] private bool _Light_StartOff = false;
    [Header("Type - Lever/Door")]
    [SerializeField] private Transform _LeverRotationPoint = null;
    [SerializeField] private Vector2 _LeverMinMaxRotation = Vector2.zero;
    [SerializeField] private float _CompleteDeathZone = 0;
    [Header("Type - Button")]
    [SerializeField] private float _ButtonPressDepth = 0;
    private bool _ButtonPressed;
    [Header("Type - Item")]
    [SerializeField] private string _ItemInfo = "";
    [Header("Speed")]
    [SerializeField] private float _Speed = 1;

    [Header("OnHigh")]
    [SerializeField] private UnityEvent _OnHighEvent = null;
    [Header("OnLow")]
    [SerializeField] private UnityEvent _OnLowEvent = null;
    [Header("OnNeutral")]
    [SerializeField] private UnityEvent _OnNeutral = null;
    [Header("Trigger")]
    [SerializeField] private UnityEvent _OnTrigger = null;


    private Vector3 velocity = Vector3.zero;
    private Rigidbody _RB;
    private Vector3 _DefaultLocalPosition;
    private Vector3 _DefaultPosition;
    private bool _MovingBack;

    private void Start()
    {
        _DefaultLocalPosition = transform.localPosition;
        _DefaultPosition = transform.position;
        _RB = GetComponent<Rigidbody>();
        if (_Type == InteractableType.SetLight || _Type == InteractableType.SetLightNegative)
        {
            if (_Light_StartOff)
                _Light.SetActive(false);
            else
                _Light.SetActive(true);
        }
    }

    private void Update()
    {
        if (_Type == InteractableType.Button)
        {
            UpdateButton();
        }

        if (_MovingBack)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, _DefaultLocalPosition, 10 * Time.deltaTime);
            if (transform.localPosition == _DefaultLocalPosition)
                _MovingBack = false;
        }
    }

    public InteractableType Type()
    {
        return _Type;
    }

    public void GotoPickupPoint(Transform point)
    {
        _RB.velocity = Vector3.zero;
        transform.position = Vector3.SmoothDamp(transform.position, point.position, ref velocity, 0.2f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, point.rotation, 5f);
    }
    public void SetVelocity(Vector3 velocity)
    {
        _RB.velocity = velocity;
    }
    public void TrowObject(Transform transformtrow)
    {
        _RB.AddForce(transformtrow.forward * 5000);
    }
    public void OpenDoor()
    {
        float mouseY = Input.GetAxis("Mouse Y");
        float angle = 0;
        switch (_AxisOption)
        {
            case AxisOptions.x:
                angle = _LeverRotationPoint.localEulerAngles.x;
                break;
            case AxisOptions.y:
                angle = _LeverRotationPoint.localEulerAngles.y;
                break;
            case AxisOptions.z:
                angle = _LeverRotationPoint.localEulerAngles.z;
                break;
        }
        angle = (angle > 180) ? angle - 360 : angle;

        HandleRotation(_LeverRotationPoint, new Vector2(0, mouseY), _LeverMinMaxRotation, 1.2f, angle);
    }
    public void MoveLever()
    {
        float mouseY = Input.GetAxis("Mouse Y");
        float angle = 0;
        switch (_AxisOption)
        {
            case AxisOptions.x:
                angle = _LeverRotationPoint.localEulerAngles.x;
                break;
            case AxisOptions.y:
                angle = _LeverRotationPoint.localEulerAngles.y;
                break;
            case AxisOptions.z:
                angle = _LeverRotationPoint.localEulerAngles.z;
                break;
        }
        angle = (angle > 180) ? angle - 360 : angle;

        HandleRotation(_LeverRotationPoint, new Vector2(0, mouseY), _LeverMinMaxRotation, 1.2f, angle);

        //Check
        if (angle < _LeverMinMaxRotation.x + _CompleteDeathZone)
        {
            _OnLowEvent.Invoke();
        }
        if (angle > _LeverMinMaxRotation.y - _CompleteDeathZone)
        {
            _OnHighEvent.Invoke();
        }
        if (angle > _LeverMinMaxRotation.x + _CompleteDeathZone && angle < _LeverMinMaxRotation.y - _CompleteDeathZone)
        {
            _OnNeutral.Invoke();
        }
    }
    public void PressButton(bool option)
    {
        _ButtonPressed = true;
    }
    public void PressButtonNegative()
    {
        _ButtonPressed = !_ButtonPressed;
    }
    public void SetLight(bool option)
    {
        _Light.SetActive(option);
    }
    public void SetLightNegative()
    {
        if (_Light.activeSelf)
            _Light.SetActive(false);
        else
            _Light.SetActive(true);
    }
    public void ReturnToDefaultPos()
    {
        _MovingBack = true;
    }
    public string GetItemInfo()
    {
        return _ItemInfo;
    }
    public void PressUIButton()
    {
        _OnTrigger.Invoke();
    }
    private void HandleRotation(Transform effectedtransform, Vector2 mousemovement, Vector2 minmaxangle, float speed, float angle)
    {
        if (_InvertMouse)
        {
            mousemovement.x = mousemovement.x * -2;
            mousemovement.y = mousemovement.y * -2;
        }

        switch (_AxisOption)
        {
            case AxisOptions.x:
                effectedtransform.localEulerAngles += new Vector3((mousemovement.x + mousemovement.y) * speed, 0, 0);

                if (angle < minmaxangle.x)
                    effectedtransform.localEulerAngles = new Vector3(minmaxangle.x + 0.5f, 0, 0);
                if (angle > minmaxangle.y)
                    effectedtransform.localEulerAngles = new Vector3(minmaxangle.y - 0.5f, 0, 0);
                break;
            case AxisOptions.y:
                effectedtransform.localEulerAngles += new Vector3(0, (mousemovement.x + mousemovement.y) * speed, 0);

                if (angle < minmaxangle.x)
                    effectedtransform.localEulerAngles = new Vector3(0, minmaxangle.x + 0.5f, 0);
                if (angle > minmaxangle.y)
                    effectedtransform.localEulerAngles = new Vector3(0, minmaxangle.y - 0.5f, 0);
                break;
            case AxisOptions.z:
                effectedtransform.localEulerAngles += new Vector3(0, 0, (mousemovement.x + mousemovement.y) * speed);

                if (angle < minmaxangle.x)
                    effectedtransform.localEulerAngles = new Vector3(0, 0, minmaxangle.x + 0.5f);
                if (angle > minmaxangle.y)
                    effectedtransform.localEulerAngles = new Vector3(0, 0, minmaxangle.y - 0.5f);
                break;
        }
    }

    private void UpdateButton()
    {
        switch (_AxisOption)
        {
            case AxisOptions.x:
                if (_ButtonPressed)
                {
                    if (transform.localPosition.x > _DefaultLocalPosition.x - _ButtonPressDepth)
                        transform.localPosition -= new Vector3(_Speed, 0, 0) * Time.deltaTime;
                    else
                    {
                        transform.localPosition = new Vector3(_DefaultLocalPosition.x - _ButtonPressDepth - 0.001f, transform.localPosition.y, transform.localPosition.z);
                        _OnLowEvent.Invoke();
                    }
                }
                else
                {
                    if (transform.localPosition.x < _DefaultLocalPosition.x + _ButtonPressDepth)
                        transform.localPosition += new Vector3(_Speed, 0, 0) * Time.deltaTime;
                    else
                    {
                        transform.localPosition = new Vector3(_DefaultLocalPosition.x + _ButtonPressDepth, transform.localPosition.y, transform.localPosition.z);
                        _OnHighEvent.Invoke();
                    }

                }
                break;
            case AxisOptions.y:
                if (_ButtonPressed)
                {
                    if (transform.localPosition.y > _DefaultLocalPosition.y - _ButtonPressDepth)
                        transform.localPosition -= new Vector3(0, _Speed, 0) * Time.deltaTime;
                    else
                    {
                        transform.localPosition = new Vector3(_DefaultLocalPosition.x, _DefaultLocalPosition.y - _ButtonPressDepth - 0.001f, _DefaultLocalPosition.z);
                        _OnLowEvent.Invoke();
                    }
                }
                else
                {
                    if (transform.localPosition.y < _DefaultLocalPosition.y)
                        transform.localPosition += new Vector3(0, _Speed, 0) * Time.deltaTime;
                    else
                    {
                        transform.localPosition = _DefaultLocalPosition;
                        _OnHighEvent.Invoke();
                    }
                }
                break;
            case AxisOptions.z:
                if (_ButtonPressed)
                {
                    if (transform.localPosition.z > _DefaultLocalPosition.z - _ButtonPressDepth)
                        transform.localPosition -= new Vector3(0, 0, _Speed) * Time.deltaTime;
                    else
                    {
                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, _DefaultLocalPosition.z - _ButtonPressDepth - 0.001f);
                        _OnLowEvent.Invoke();
                    }
                }
                else
                {
                    if (transform.localPosition.z < _DefaultLocalPosition.z + _ButtonPressDepth)
                        transform.localPosition += new Vector3(0, 0, _Speed) * Time.deltaTime;
                    else
                    {
                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, _DefaultLocalPosition.z + _ButtonPressDepth);
                        _OnHighEvent.Invoke();
                    }
                }
                break;
        }
    }
}
