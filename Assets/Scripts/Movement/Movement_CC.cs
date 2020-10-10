using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Movemenct_CC : MonoBehaviour
{

    //Movement
    [SerializeField] private float _NormalSpeed = 0, _SprintSpeed = 0;
    [SerializeField] private float _JumpSpeed = 0;
    [SerializeField] private float _Gravity = 0;
    private Vector3 _MoveDirection = Vector3.zero;
    //Look around
    public float _CameraSensitivity = 0;
    [SerializeField] private Transform _Head = null;
    private float _RotationX = 90.0f;
    private float _RotationY = 0.0f;
    private float _Speed;

    private CharacterController _CC;
    private bool _LockRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _CC = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!_LockRotation)
        {
            //Look around
            _RotationX += Input.GetAxis("Mouse X") * _CameraSensitivity;
            _RotationY += Input.GetAxis("Mouse Y") * _CameraSensitivity;
            _RotationY = Mathf.Clamp(_RotationY, -90, 90);

            transform.localRotation = Quaternion.AngleAxis(_RotationX, Vector3.up * Time.deltaTime);
            _Head.transform.localRotation = Quaternion.AngleAxis(_RotationY, Vector3.left * Time.deltaTime);
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