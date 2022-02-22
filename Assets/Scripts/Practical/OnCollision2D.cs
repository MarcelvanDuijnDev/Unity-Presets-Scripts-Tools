using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnCollision2D : MonoBehaviour
{
    private enum Options {OnTriggerEnter, OnTriggerExit, OnTriggerStay, OnCollisionEnter, OnCollisionExit, OnCollisionStay, OnAll};
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

    private void Action(Collider2D other)
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
    private void Action(Collision2D other)
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_Option == Options.OnTriggerEnter || _Option == Options.OnAll)
            Action(other);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (_Option == Options.OnTriggerExit || _Option == Options.OnAll)
            Action(other);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (_Option == Options.OnTriggerStay || _Option == Options.OnAll)
            Action(other);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_Option == Options.OnCollisionEnter || _Option == Options.OnAll)
            Action(other);
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (_Option == Options.OnCollisionExit || _Option == Options.OnAll)
            Action(other);
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        if (_Option == Options.OnCollisionStay || _Option == Options.OnAll)
            Action(other);
    }
}