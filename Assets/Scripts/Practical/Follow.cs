using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public enum FollowOptions {MoveTowards, Lerp, SetPosition, SetParent }
    [SerializeField] private FollowOptions _Option;
    [SerializeField] private Transform _Target;
    [SerializeField] private float _FollowSpeed;

    private void Start()
    {
        if(_Option == FollowOptions.SetParent)
        {
            transform.parent = _Target.transform;
            transform.localPosition = Vector3.zero;
        }    
    }

    void Update()
    {
        switch(_Option)
        {
            case FollowOptions.MoveTowards:
                transform.position = Vector3.MoveTowards(transform.position, _Target.position, _FollowSpeed * Time.deltaTime);
                break;
            case FollowOptions.Lerp:
                transform.position = Vector3.Lerp(transform.position, _Target.position, _FollowSpeed * Time.deltaTime);
                break;
            case FollowOptions.SetPosition:
                transform.position = _Target.position;
                break;
        }
    }
}
