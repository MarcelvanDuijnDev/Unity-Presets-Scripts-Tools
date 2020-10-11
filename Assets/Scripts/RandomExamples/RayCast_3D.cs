using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCast_3D : MonoBehaviour
{
    [SerializeField] private LayerMask _LayerMask;

    private void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, _LayerMask))
        {
            //Hit
        }
        else
        {
            //No hit
        }
    }
}
