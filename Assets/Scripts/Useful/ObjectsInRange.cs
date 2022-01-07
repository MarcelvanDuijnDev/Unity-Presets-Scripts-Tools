using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsInRange : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Collider[] _GameObjectsInRange = new Collider[0];
    [SerializeField] private LayerMask _Layer;
    [SerializeField] private float _Range = 5;

    void Start()
    {
        CheckObjectsInRange();
    }

    void Update()
    {
        //CheckObjectsInRange();
    }

    public void CheckObjectsInRange()
    {
        _GameObjectsInRange = Physics.OverlapSphere(transform.position, _Range, _Layer);

        Debug.Log("Objects in range: " + _GameObjectsInRange.Length.ToString());
        for (int i = 0; i < _GameObjectsInRange.Length; i++)
        {
            Debug.Log(_GameObjectsInRange[i].name);
        }
    }
}
