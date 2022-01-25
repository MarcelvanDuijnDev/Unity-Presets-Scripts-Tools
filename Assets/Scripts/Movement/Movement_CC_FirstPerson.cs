using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement_CC_FirstPerson : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _NormalSpeed = 5;
    [SerializeField] private float _SprintSpeed = 8;
    [SerializeField] private float _JumpSpeed = 5;
    [SerializeField] private float _Gravity = 20;
    [SerializeField] private float _CameraSensitivity = 1;

    [Header("Head")]
    [SerializeField] private Transform _Head = null;
    
    //Private Variables
    private Vector3 _MoveDirection;
    private Vector2 _LookRotation;
    private CharacterController _CC;
    private bool _LockRotation;
    private float _Speed;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _CC = GetComponent<CharacterController>();
        if (_Head == null)
            _Head = transform.GetChild(0).transform;
    }

    void Update()
    {
        //Look around
        if (!_LockRotation)
        {
            _LookRotation.x += Input.GetAxis("Mouse X") * _CameraSensitivity;
            _LookRotation.y += Input.GetAxis("Mouse Y") * _CameraSensitivity;
            _LookRotation.y = Mathf.Clamp(_LookRotation.y, -90, 90);

            transform.localRotation = Quaternion.AngleAxis(_LookRotation.x, Vector3.up);
            _Head.transform.localRotation = Quaternion.AngleAxis(_LookRotation.y, Vector3.left);
        }

        //Movement
        if (_CC.isGrounded)
        {
            _MoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            _MoveDirection = transform.TransformDirection(_MoveDirection);
            _MoveDirection *= _Speed;
            if (Input.GetButton("Jump"))
                _MoveDirection.y = _JumpSpeed;
        }

        //Sprint
        if (Input.GetKey(KeyCode.LeftShift))
            _Speed = _SprintSpeed;
        else
            _Speed = _NormalSpeed;

        //Apply Movement
        _MoveDirection.y -= _Gravity * Time.deltaTime;
        _CC.Move(_MoveDirection * Time.deltaTime);
    }

    public void LockRotation(bool state)
    {
        _LockRotation = state;
    }
}