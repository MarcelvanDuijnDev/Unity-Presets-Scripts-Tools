using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarArcade : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _ForwardAccel = 8;
    [SerializeField] private float _ReverseAccel = 4;
    [SerializeField] private float _TurnStrength = 180;
    [SerializeField] private float _GravityForce = 15;

    [Header("GroundCheck")]
    [SerializeField] private LayerMask _GroundMask = ~0;
    [SerializeField] private float _GroundCheckLength = 0.5f;

    [Header("RigidBody")]
    [SerializeField] private Rigidbody _RB = null;

    private float _SpeedInput;
    private float _TurnInput;
    private bool _Grounded;

    void Start() => _RB.transform.parent = null;

    void Update()
    {
        _SpeedInput = 0;
        if(Input.GetAxis("Vertical") > 0)
            _SpeedInput = Input.GetAxis("Vertical") * _ForwardAccel * 1000;
        else if(Input.GetAxis("Vertical") < 0)
            _SpeedInput = Input.GetAxis("Vertical") * _ReverseAccel * 1000;

        _TurnInput = Input.GetAxis("Horizontal");

        if(_Grounded)
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, _TurnInput * _TurnStrength * Time.deltaTime, 0));

        transform.position = _RB.transform.position;
    }

    private void FixedUpdate()
    {
        _Grounded = GroundCheck();

        if (_Grounded)
        {
            if (Mathf.Abs(_SpeedInput) > 0)
                _RB.AddForce(transform.forward * _SpeedInput);
        }
        else
            _RB.AddForce(Vector3.up * -_GravityForce * 100);
    }

    private bool GroundCheck()
    {
        _Grounded = false;
        RaycastHit hit;

        if(Physics.Raycast(transform.position, -transform.up, out hit, _GroundCheckLength, _GroundMask))
        {
            _Grounded = true;
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }

        return _Grounded;
    }
}
