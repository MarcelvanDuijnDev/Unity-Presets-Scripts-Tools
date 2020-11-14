using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement_2D_Platformer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _NormalSpeed = 5;
    [SerializeField] private float _SprintSpeed = 8;
    [SerializeField] private float _JumpSpeed = 300;
    [SerializeField] private float _GroundCheck = 0.6f;
    [Header("Set ground layer")]
    [SerializeField] private LayerMask _GroundMask = ~1;

    private float _Speed = 0;
    private Rigidbody2D _RB;

    void Start()
    {
        //Get Rigidbody / Lock z rotation
        _RB = GetComponent<Rigidbody2D>();
        _RB.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        //Sprint
        if (Input.GetKey(KeyCode.LeftShift))
            _Speed = _SprintSpeed;
        else
            _Speed = _NormalSpeed;

        //Jumping
        if (Input.GetButtonDown("Jump") && IsGrounded())
            _RB.AddForce(new Vector2(0, _JumpSpeed));

        //Apply Movement
        _RB.velocity = new Vector2(Input.GetAxis("Horizontal") * _Speed, _RB.velocity.y);
    }

    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _GroundCheck, _GroundMask);
        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }
}
