using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionHandler : MonoBehaviour
{
    [Header("Range")]
    [SerializeField] private float _Range = 3;

    [Header("Refs")]
    [SerializeField] private Image _Cursor = null;
    [SerializeField] private LayerMask _LayerMask = 0;
    [SerializeField] private Transform _Head = null;

    [Header("Pickup")]
    [SerializeField] private GameObject _PickupPoint = null;
    [SerializeField] private Vector2 _PickupMinMaxRange = Vector2.zero;

    [Header("Item")]
    [SerializeField] private Transform _ItemPreviewPoint = null;
    [SerializeField] private TextMeshProUGUI _ItemInfoText = null;

    [Header("Visual Settings")]
    [SerializeField] private Color _Cursor_DefaultColor = new Vector4(0.3f,0.3f,0.3f,0.4f);
    [SerializeField] private Color _Cursor_InteractColor = new Vector4(1f, 1f, 1f, 1f);
    [SerializeField] private float _Cursor_Increase = 1.5f;
    [SerializeField] private float _Cursor_Increase_ChangeSpeed = 20;

    private string _ItemInfo;

    private Vector3 _PickupPointPosition;
    private Vector3 _CalcVelocity;
    private Vector3 _PrevPosition;

    private GameObject _ActiveObject;
    private GameObject _CheckObject;
    private Interactable _Interactable;

    private bool _Interacting;
    private bool _Previewing;


    private float _DefaultSize;

    private Movement_CC_FirstPerson _CCS; //Script that handles rotation

    void Start()
    {
        _CCS = GetComponent<Movement_CC_FirstPerson>();
        _PickupPointPosition.z = _PickupMinMaxRange.x;

        _DefaultSize = _Cursor.transform.localScale.x;
    }

    void Update()
    {
        if (!_Interacting)
        {
            RaycastHit hit;

            if (Physics.Raycast(_Head.position, _Head.TransformDirection(Vector3.forward), out hit, _Range, _LayerMask))
            {
                Debug.DrawRay(_Head.position, _Head.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

                _ActiveObject = hit.transform.gameObject;

                _Cursor.color = _Cursor_InteractColor;

                _Cursor.transform.localScale = Vector3.Lerp(_Cursor.transform.localScale, new Vector3(_DefaultSize * _Cursor_Increase,
                    _DefaultSize * _Cursor_Increase,
                    _DefaultSize * _Cursor_Increase), _Cursor_Increase_ChangeSpeed * Time.deltaTime);
            }
            else
            {
                Debug.DrawRay(_Head.position, _Head.TransformDirection(Vector3.forward) * _Range, Color.white);
                _Cursor.color = _Cursor_DefaultColor;

                _ActiveObject = null;
                _CheckObject = null;

                _Cursor.transform.localScale = Vector3.Lerp(_Cursor.transform.localScale, new Vector3(_DefaultSize,
                    _DefaultSize,
                    _DefaultSize), _Cursor_Increase_ChangeSpeed * Time.deltaTime);
            }

            if (_ActiveObject != _CheckObject)
            {
                _Interactable = _ActiveObject.GetComponent<Interactable>();
                _CheckObject = _ActiveObject;
            }
        }

        if (_ActiveObject != null && _Interactable != null)
        {
            if (_Interactable._Type != Interactable.InteractableType.Item)
            {
                //OnDown
                if (Input.GetMouseButtonDown(0))
                    OnDown();

                if (_Interacting)
                {
                    //OnUp
                    if (Input.GetMouseButtonUp(0))
                        OnUp();

                    //OnActive
                    OnActive();
                }
            }
            else
            {
                if (!_Previewing)
                {
                    //Start Preview
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        _ItemInfo = _Interactable.GetItemInfo();
                        _CCS.LockRotation(true);
                        _Previewing = true;
                    }
                }
                else
                {
                    _ActiveObject.transform.position = _ItemPreviewPoint.position;
                    _Interactable.gameObject.transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);

                    //Reset Preview
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        _ItemInfo = "";
                        _CCS.LockRotation(false);
                        _Interactable.ReturnToDefaultPos();
                        _Previewing = false;
                    }
                }
            }
        }

        _ItemInfoText.text = _ItemInfo;
    }

    void FixedUpdate()
    {
        if (_Interacting)
        {
            OnActiveFixed();
            OnActiveFixed();
        }
    }

    private void OnUp()
    {
        _Interacting = false;
        switch (_Interactable._Type)
        {
            case Interactable.InteractableType.Lever:
                _CCS.LockRotation(false);
                break;
            case Interactable.InteractableType.Door:
                _CCS.LockRotation(false);
                break;
            case Interactable.InteractableType.Move:
                _Interactable.SetVelocity(_CalcVelocity);
                break;
        }
    }
    private void OnDown()
    {
        _Interacting = true;

        //OnClick
        switch (_Interactable._Type)
        {
            case Interactable.InteractableType.SetLight:
                _Interactable.SetLight(true);
                break;
            case Interactable.InteractableType.SetLightNegative:
                _Interactable.SetLightNegative();
                break;
            case Interactable.InteractableType.Move:
                _PickupPoint.transform.rotation = _ActiveObject.transform.rotation;
                _PickupPointPosition.z = Vector3.Distance(_Head.position, _ActiveObject.transform.position);
                break;
            case Interactable.InteractableType.Lever:
                _CCS.LockRotation(true);
                _PickupPointPosition.z = Vector3.Distance(_Head.position, _ActiveObject.transform.position);
                break;
            case Interactable.InteractableType.Door:
                _CCS.LockRotation(true);
                break;
            case Interactable.InteractableType.Button:
                _Interactable.PressButtonNegative();
                break;
            case Interactable.InteractableType.UIButton:
                _Interactable.PressUIButton();
                break;
            case Interactable.InteractableType.Trigger:
                _Interactable.TriggerEvent();
                break;
        }
    }
    private void OnActive()
    {
        switch (_Interactable._Type)
        {
            case Interactable.InteractableType.Move:
                if (_PickupPointPosition.z < _PickupMinMaxRange.y && Input.mouseScrollDelta.y > 0)
                    _PickupPointPosition.z += Input.mouseScrollDelta.y * 0.5f;
                if (_PickupPointPosition.z > _PickupMinMaxRange.x && Input.mouseScrollDelta.y < 0)
                    _PickupPointPosition.z += Input.mouseScrollDelta.y * 0.5f;

                if (Input.GetMouseButtonDown(1))
                {
                    _Interactable.TrowObject(_Head.transform);
                    OnUp();
                }
                break;
            case Interactable.InteractableType.Door:
                _Interactable.OpenDoor();
                break;
            case Interactable.InteractableType.Lever:
                _Interactable.MoveLever();
                break;
        }

        if (Vector3.Distance(_Head.transform.position, _ActiveObject.transform.position) > _Range)
        {
            _Interacting = false;
            OnUp();
        }
    }

    private void OnActiveFixed()
    {
        switch (_Interactable._Type)
        {
            case Interactable.InteractableType.Move:
                _Interactable.GotoPickupPoint(_PickupPoint.transform);

                _PickupPoint.transform.localPosition = _PickupPointPosition;

                _CalcVelocity = (_ActiveObject.transform.position - _PrevPosition) / Time.deltaTime;
                _PrevPosition = _ActiveObject.transform.position;
                break;
        }
    }
}