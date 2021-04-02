using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ExampleGame_TopDownShooter_Enemy : MonoBehaviour
{
    [SerializeField] private GameObject _DieEffect;

    private NavMeshAgent _Nav;
    private GameObject _PlayerObj;
    private Health _Health;

    void Start()
    {
        _Health = GetComponent<Health>();
        _PlayerObj = GameObject.Find("Player");
        _Nav = this.gameObject.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        _Nav.destination = _PlayerObj.transform.position;
    }

    public void DoDamage(float damageAmount)
    {
        if (_Health.GetCurrentHealth() <= 0)
        {
            GameObject.Find("GameHandler").GetComponent<ExampleGame_TopDownShooter>().AddScore(100);
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Instantiate(_DieEffect, transform.position, Quaternion.identity);
            GameObject.Find("GameHandler").GetComponent<ExampleGame_TopDownShooter>().DoDamage();
            gameObject.SetActive(false);
        }
    }
}
