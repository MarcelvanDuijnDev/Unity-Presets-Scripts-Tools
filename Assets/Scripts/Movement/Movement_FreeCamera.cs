using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_FreeCamera : MonoBehaviour
{
    [SerializeField] private float _Speed;
    [SerializeField] private float _SprintSpeed;

    private float _CurrentSpeed;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            _CurrentSpeed = _SprintSpeed;
        else
            _CurrentSpeed = _Speed;

        float xas = Input.GetAxis("Horizontal");
        float zas = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(xas,0, zas) * _CurrentSpeed * Time.deltaTime);

        float mousex = Input.GetAxis("Mouse X");
        float mousey = Input.GetAxis("Mouse Y");
        transform.eulerAngles += new Vector3(-mousey, mousex, 0);
    }
}
