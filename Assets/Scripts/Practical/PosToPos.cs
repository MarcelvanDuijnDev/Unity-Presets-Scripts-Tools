using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosToPos : MonoBehaviour
{
    [SerializeField] private Transform _GotoPosition = null;
    [SerializeField] private float _Speed = 0;

    private bool _Activated;

    void Update()
    {
        if (_Activated)
            transform.position = Vector3.MoveTowards(transform.position, _GotoPosition.position, _Speed);
    }

    public void StartMoving()
    {
        _Activated = true;
    }
}
