using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private Vector3 _RotationSpeed = Vector3.zero;

    void Update()
    {
        transform.Rotate(new Vector3(_RotationSpeed.x, _RotationSpeed.y, _RotationSpeed.z) * Time.deltaTime);
    }
}
