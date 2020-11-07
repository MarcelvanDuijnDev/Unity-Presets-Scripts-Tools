using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Platformer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _NormalSpeed = 5, _SprintSpeed = 8;
    [SerializeField] private float _JumpSpeed = 5;
    [SerializeField] private float _Gravity = 20;
    
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
            _MoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
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
