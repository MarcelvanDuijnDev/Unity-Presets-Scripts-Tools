using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    [SerializeField] private Transform _Target;

    void Update()
    {
        transform.LookAt(_Target);
    }
}
