using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnCollision : MonoBehaviour
{
    private enum Options {OnTriggerEnter, OnTriggerExit, OnTriggerStay, OnCollisionEnter, OnCollisionExit, OnCollisionStay};
    [SerializeField] private LayerMask _LayerMask = ~0;
    [SerializeField] private Options _Option = Options.OnTriggerEnter;
    [SerializeField] private string _Tag = "";
    [SerializeField] private UnityEvent _Event = null;

    private bool _HasTag;

    private void Start()
    {
        if (_Tag != "" || _Tag != null)
            _HasTag = true;
    }

    private void Action(Collider other)
    {
        if (_HasTag)
            if (other.CompareTag(_Tag) && other.gameObject.layer == _LayerMask)
                _Event.Invoke();
    }
    private void Action(Collision other)
    {
        if (_HasTag)
            if (other.gameObject.CompareTag(_Tag) && other.gameObject.layer == _LayerMask)
                _Event.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_Option == Options.OnTriggerEnter)
            Action(other);
    }
    private void OnTriggerExit(Collider other)
    {
        if (_Option == Options.OnTriggerExit)
            Action(other);
    }
    private void OnTriggerStay(Collider other)
    {
        if (_Option == Options.OnTriggerStay)
            Action(other);
    }
    void OnCollisionEnter(Collision other)
    {
        if (_Option == Options.OnCollisionEnter)
            Action(other);
    }
    void OnCollisionExit(Collision other)
    {
        if (_Option == Options.OnCollisionExit)
            Action(other);
    }
    void OnCollisionStay(Collision other)
    {
        if (_Option == Options.OnCollisionStay)
            Action(other);
    }
}