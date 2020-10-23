using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _Speed;
    [SerializeField] private float _Damage;

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * _Speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "ExampleTag")
        {
            //DoDamage
            gameObject.SetActive(false);
        }
    }
}