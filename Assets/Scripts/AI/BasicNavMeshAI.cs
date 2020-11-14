using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicNavMeshAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform _Target = null;
    [SerializeField] private float _Speed = 2;

    private NavMeshAgent _Agent;

    private void Awake()
    {
        if (_Target == null)
        {
            try
            {
                _Target = GameObject.Find("Player").transform;
            }
            catch
            {
                Debug.Log("No Target");
            }
        }

        _Agent = GetComponent<NavMeshAgent>();
        _Agent.speed = _Speed;
    }

    private void Update()
    {
        if (_Target != null)
        {
            _Agent.SetDestination(_Target.position);
        }
    }
}
