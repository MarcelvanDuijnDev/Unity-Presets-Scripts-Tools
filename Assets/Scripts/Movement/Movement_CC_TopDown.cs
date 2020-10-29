using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement_CC_TopDown : MonoBehaviour
{
    //Movement
    [Header("Settings Camera")]
    [SerializeField] private Camera _Camera;
    [Header("Settings")]
    [SerializeField] private float _NormalSpeed = 5;
    [SerializeField] private float _SprintSpeed = 8;
    [SerializeField] private float _JumpSpeed = 5;
    [SerializeField] private float _Gravity = 20;
    [SerializeField] private bool _MovementRelativeToRotation = false;

    private float _Speed = 0;
    private Vector3 _MoveDirection = Vector3.zero;
    private CharacterController _CC;

    void Start()
    {
        _CC = GetComponent<CharacterController>();
    }

    void Update()
    {
        //Movement
        if (_CC.isGrounded)
        {
            _MoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (_MovementRelativeToRotation)
                _MoveDirection = transform.TransformDirection(_MoveDirection);
            _MoveDirection *= _Speed;
            if (Input.GetButton("Jump"))
                _MoveDirection.y = _JumpSpeed;
        }

        _MoveDirection.y -= _Gravity * Time.deltaTime;
        _CC.Move(_MoveDirection * Time.deltaTime);

        //Sprint
        if (Input.GetKey(KeyCode.LeftShift))
            _Speed = _SprintSpeed;
        else
            _Speed = _NormalSpeed;

        Ray cameraRay = _Camera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;
        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
        }
    }

    public void SetCamera(Camera cameraobj)
    {
        _Camera = cameraobj;
    }
}
