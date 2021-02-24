using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour {

    [Header("Settings")]
    [SerializeField] private float _RotateSpeed = 4.0f;
    [SerializeField] private float _NeighbourDistance = 3.0f;

    private bool _Turning;
    private float _Speed = 0.5f;

    void Start () 
    {
        _Speed = Random.Range(0.5f, 1);
	}
	
	void Update () 
    {
        if (Vector3.Distance(transform.position, Vector3.zero) >= FlockingHandler._FlockArea)
            _Turning = true;
        else
            _Turning = false;

        if (_Turning)
        {
            Vector3 direction = Vector3.zero - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), _RotateSpeed * Time.deltaTime);
            _Speed = Random.Range(0.5f, 1);
        }
        else
        {
            if (Random.Range(0, 5) < 1)
                ApplyRules();
        }
        transform.Translate(0,0,Time.deltaTime * _Speed);
	}

    void ApplyRules()
    {
        GameObject[] gos;
        gos = FlockingHandler._AllUnits;

        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.1f;

        Vector3 goalPos = FlockingHandler._GoalPos;

        float dist;

        int groupSize = 0;
        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                dist = Vector3.Distance(go.transform.position, this.transform.position);
                if (dist <= _NeighbourDistance)
                {
                    vcentre += go.transform.position;
                    groupSize++;

                    if (dist < 1.0f)
                        vavoid = vavoid + (this.transform.position - go.transform.position);

                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock._Speed;
                }
            }
        }

        if (groupSize > 0)
        {
            vcentre = vcentre / groupSize + (goalPos - this.transform.position);
            _Speed = gSpeed / groupSize;

            Vector3 direction = (vcentre + vavoid) - transform.position;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), _RotateSpeed * Time.deltaTime);
        }
    }
}