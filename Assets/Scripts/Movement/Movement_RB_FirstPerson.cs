using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement_RB_FirstPerson : MonoBehaviour
{
    [Header("Set Refference")]
    [SerializeField] private Transform _Head = null;

    [Header("Settings")]
    [SerializeField] private float _MovementSpeed = 5;
    [SerializeField] private float _JumpSpeed = 5;
    [SerializeField] private float _CameraSensitivity = 1;

    private Vector2 _LookRot = new Vector2(90,0);
    private Rigidbody _RB;
    private bool _Grounded;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _RB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Check Grounded
        _Grounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), 0.4f);

        //Movement
        float x = Input.GetAxisRaw("Horizontal") * _MovementSpeed;
        float y = Input.GetAxisRaw("Vertical") * _MovementSpeed;

        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && _Grounded)
            _RB.velocity = new Vector3(_RB.velocity.x, _JumpSpeed, _RB.velocity.z);

        //Apply Movement
        Vector3 move = transform.right * x + transform.forward * y;
        _RB.velocity = new Vector3(move.x, _RB.velocity.y, move.z);

        //Look around
        _LookRot.x += Input.GetAxis("Mouse X") * _CameraSensitivity;
        _LookRot.y += Input.GetAxis("Mouse Y") * _CameraSensitivity;
        _LookRot.y = Mathf.Clamp(_LookRot.y, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(_LookRot.x, Vector3.up);
        _Head.transform.localRotation = Quaternion.AngleAxis(_LookRot.y, Vector3.left);
    }
}
