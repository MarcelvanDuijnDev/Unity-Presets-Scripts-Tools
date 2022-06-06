using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOnDistance : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _Revert;
    [SerializeField] private Vector2 _MinMaxSize;
    [SerializeField] private Vector2 _MinMaxDistance;

    [Header("Target")]
    [SerializeField] private Transform _Target;

    private Vector3 _DefaultScale;

    void Start()
    {
        _DefaultScale = transform.localScale;
    }

    void Update()
    {
        //Calc
        float a = (Vector3.Distance(transform.position, _Target.position) - _MinMaxDistance.x) * (1 / (_MinMaxDistance.y - _MinMaxDistance.x));
        float b = _MinMaxSize.y - _MinMaxSize.x;

        if (_Revert)
            a = 1 - a;

        float newsize = a * b;

        //Limits
        if (newsize < _MinMaxSize.x)
            newsize = _MinMaxSize.x;
        if (newsize > _MinMaxSize.y)
            newsize = _MinMaxSize.y;

        //Apply
        transform.localScale = _DefaultScale * newsize;
    }
}
