using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement_2D_TopDown : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _NormalSpeed = 5;
    [SerializeField] private float _SprintSpeed = 8;

    private float _Speed = 0;
    private Rigidbody2D _RB;

    void Start()
    {
        //Get Rigidbody / Lock z rotation
        _RB = GetComponent<Rigidbody2D>();
        _RB.constraints = RigidbodyConstraints2D.FreezeRotation;
        _RB.gravityScale = 0;
    }

    void Update()
    {
        //Sprint
        if (Input.GetKey(KeyCode.LeftShift))
            _Speed = _SprintSpeed;
        else
            _Speed = _NormalSpeed;

        //Apply Movement
        _RB.velocity = new Vector2(Input.GetAxis("Horizontal") * _Speed, Input.GetAxis("Vertical") * _Speed);
    }
}
