using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ShootingRayCast : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _Damage = 20;
    [SerializeField] private float _ShootDistance = 50;
    [SerializeField] private string _EnemyTag = "Enemy";

    [Header("Semi")]
    [SerializeField] private int _SemiAutomaticBulletAmount = 3;
    [SerializeField] private float _SemiShootSpeed = 0.2f;
    [Header("Automatic")]
    [SerializeField] private float _SecondsBetweenShots = 0.5f;

    private enum ShootModes {SingleShot, SemiAutomatic, Automatic }
    [SerializeField] private ShootModes _ShootMode;

    private bool _CheckSingleShot;
    private float _Timer;
    private bool _LockShooting;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            switch (_ShootMode)
            {
                case ShootModes.SingleShot:
                    if (!_CheckSingleShot)
                        Shoot();
                    _CheckSingleShot = true;
                    break;
                case ShootModes.SemiAutomatic:
                    if (!_CheckSingleShot && !_LockShooting)
                        StartCoroutine(SemiShot());
                    _CheckSingleShot = true;
                    break;
                case ShootModes.Automatic:
                    _Timer += 1 * Time.deltaTime;
                    if(_Timer >= _SecondsBetweenShots)
                    {
                        Shoot();
                        _Timer = 0;
                    }
                    break;
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            _CheckSingleShot = false;
        }
    }

    IEnumerator SemiShot()
    {
        _LockShooting = true;
        for (int i = 0; i < _SemiAutomaticBulletAmount; i++)
        {
            Shoot();
            yield return new WaitForSeconds(_SemiShootSpeed);
        }
        _LockShooting = false;
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _ShootDistance))
            if (hit.transform.tag == _EnemyTag)
            {
                hit.transform.GetComponent<Health>().DoDamage(_Damage);
            }
    }
}