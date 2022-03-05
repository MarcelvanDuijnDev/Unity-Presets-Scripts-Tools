using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosToPos : MonoBehaviour
{
    [Header("Target Position")]
    [SerializeField] private Transform _GotoPosition = null;

    [Header("Settings")]
    [SerializeField] private float _Speed = 1;
    [SerializeField] private bool _Lerp = false;
    [SerializeField] private bool _OnStart = false;

    private bool _Activated;

    private void Start()
    {
        if (_OnStart)
            StartMoving();
    }

    void Update()
    {
        if (_Activated)
            if (_Lerp)
                transform.position = Vector3.Lerp(transform.position, _GotoPosition.position, _Speed * Time.deltaTime);
            else
                transform.position = Vector3.MoveTowards(transform.position, _GotoPosition.position, _Speed * Time.deltaTime);
    }

    public void StartMoving()
    {
        _Activated = true;
    }
}
