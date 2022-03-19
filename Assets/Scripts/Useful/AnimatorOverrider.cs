using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorOverrider : MonoBehaviour
{
    [SerializeField] private AnimatorOverrideController[] _OverrideControllers;
    private Animator _Animator;

    private void Awake()
    {
        _Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //Example
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Set(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            Set(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            Set(2);
    }

    public void SetAnimations(AnimatorOverrideController overridecontroller)
    {
        _Animator.runtimeAnimatorController = overridecontroller;
    }

    public void Set(int value)
    {
        SetAnimations(_OverrideControllers[value]);
    }
}