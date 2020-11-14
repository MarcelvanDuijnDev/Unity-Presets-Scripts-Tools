using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 _MinMaxRange = Vector2.zero;
    [SerializeField] private float _SecondsBetweenShots = 2;
    [SerializeField] private float _Damage = 25;
    [SerializeField] private GameObject _ShootPart = null;
    [SerializeField] private string _Tag = "Enemy";
    
    private float _Timer;
    private GameObject _Target;

    void Update()
    {
        if (_Target != null)
        {
            _ShootPart.transform.LookAt(_Target.transform.position);
            _Timer += 1 * Time.deltaTime;
            if (_Timer >= _SecondsBetweenShots)
            {
                _Target.GetComponent<Health>().DoDamage(_Damage);
                _Timer = 0;
            }
        }
        else
        {
            _ShootPart.transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        _Target = FindEnemy();
    }

    public GameObject FindEnemy()
    {
        GameObject[] m_Targets = GameObject.FindGameObjectsWithTag(_Tag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        _MinMaxRange.x = _MinMaxRange.x * _MinMaxRange.x;
        _MinMaxRange.y = _MinMaxRange.y * _MinMaxRange.y;
        foreach (GameObject target in m_Targets)
        {
            Vector3 diff = target.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance && curDistance >= _MinMaxRange.x && curDistance <= _MinMaxRange.y)
            {
                closest = target;
                distance = curDistance;
            }
        }
        return closest;
    }
}