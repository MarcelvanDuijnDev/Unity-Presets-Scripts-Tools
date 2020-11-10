using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement_2D_Platformer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _NormalSpeed = 5, _SprintSpeed = 8;
    [SerializeField] private float _JumpSpeed = 5;
    [SerializeField] private float _GroundCheck = 0.5f;
    [SerializeField] private LayerMask _GroundMask;

    private Vector2 _MoveDirection = Vector2.zero;
    private float _Speed = 0;
    private Rigidbody2D _RB;
    private bool _Grounded = false;

    void Start()
    {
        _RB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //Movment
        _MoveDirection = new Vector2(Input.GetAxis("Horizontal"), 0);
        _MoveDirection *= _Speed;


        if (Input.GetButton("Jump") && IsGrounded())
            _MoveDirection.y = _JumpSpeed;

        //Sprint
        if (Input.GetKey(KeyCode.LeftShift))
            _Speed = _SprintSpeed;
        else
            _Speed = _NormalSpeed;

        _RB.AddForce(_MoveDirection);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, Vector2.down * _GroundCheck);
    }

    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2, _GroundMask);
        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }
}
