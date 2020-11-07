using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement_CC_Platformer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _NormalSpeed = 5, _SprintSpeed = 8;
    [SerializeField] private float _JumpSpeed = 5;
    [SerializeField] private float _Gravity = 20;
    [SerializeField] private bool _ZMovementActive = false;
    
    private Vector3 _MoveDirection = Vector3.zero;
    private float _Speed;
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
            float verticalmovement = 0;
            if (_ZMovementActive)
                verticalmovement = Input.GetAxis("Vertical");

            _MoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, verticalmovement);
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
}
