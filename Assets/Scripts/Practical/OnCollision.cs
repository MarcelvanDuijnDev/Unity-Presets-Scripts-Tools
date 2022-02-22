using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnCollision : MonoBehaviour
{
    private enum Options { OnTriggerEnter, OnTriggerExit, OnTriggerStay, OnCollisionEnter, OnCollisionExit, OnCollisionStay, OnAll };
    [SerializeField] private LayerMask _LayerMask = ~0;
    [SerializeField] private Options _Option = Options.OnAll;
    [SerializeField] private string _Tag = "";
    [SerializeField] private UnityEvent _Event = null;

    private bool _HasTag;

    private void Start()
    {
        if (_Tag != "" && _Tag != null)
            _HasTag = true;
    }

    private void Action(Collider other)
    {
        if (_HasTag)
        {
            if (other.CompareTag(_Tag) && _LayerMask == (_LayerMask | (1 << other.gameObject.layer)))
                _Event.Invoke();
        }
        else
            if(_LayerMask == (_LayerMask | (1 << other.gameObject.layer)))
                _Event.Invoke();
    }
    private void Action(Collision other)
    {
        if (_HasTag)
        { 
            if (other.gameObject.CompareTag(_Tag) && _LayerMask == (_LayerMask | (1 << other.gameObject.layer)))
                _Event.Invoke();
        }
        else
            if (_LayerMask == (_LayerMask | (1 << other.gameObject.layer)))
            _Event.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_Option == Options.OnTriggerEnter || _Option == Options.OnAll)
            Action(other);
    }
    private void OnTriggerExit(Collider other)
    {
        if (_Option == Options.OnTriggerExit || _Option == Options.OnAll)
            Action(other);
    }
    private void OnTriggerStay(Collider other)
    {
        if (_Option == Options.OnTriggerStay || _Option == Options.OnAll)
            Action(other);
    }
    private void OnCollisionEnter(Collision other)
    {
        if (_Option == Options.OnCollisionEnter || _Option == Options.OnAll)
            Action(other);
    }
    private void OnCollisionExit(Collision other)
    {
        if (_Option == Options.OnCollisionExit || _Option == Options.OnAll)
            Action(other);
    }
    private void OnCollisionStay(Collision other)
    {
        if (_Option == Options.OnCollisionStay || _Option == Options.OnAll)
            Action(other);
    }
}
