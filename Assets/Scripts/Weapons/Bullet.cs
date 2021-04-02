using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _Speed = 5;
    [SerializeField] private float _Damage = 25;

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * _Speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            other.GetComponent<Health>().DoDamage(_Damage);
            gameObject.SetActive(false);
        }
        if(other.tag == "Wall")
        {
            gameObject.SetActive(false);
        }
    }
}