using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float _MaxHealth = 100;

    [SerializeField] private UnityEvent _OnDeath;

    private float _CurrentHealth;

    private void OnEnable()
    {
        _CurrentHealth = _MaxHealth;
    }

    public void DoDamage(float damageamount)
    {
        _CurrentHealth -= damageamount;
        if(_CurrentHealth <= 0)
        {
            _CurrentHealth = 0;
            _OnDeath.Invoke();
            gameObject.SetActive(false);
        }
    }

    public float GetCurrentHealth()
    {
        return _CurrentHealth;
    }
    public float GetMaxHealth()
    {
        return GetMaxHealth();
    }
}
