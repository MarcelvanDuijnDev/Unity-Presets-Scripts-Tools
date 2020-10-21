using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private enum UIEffectOptions { Grow, Shrink }
    [SerializeField] private UIEffectOptions _UIEffect = UIEffectOptions.Grow;
    [SerializeField] private Vector3 _MinDefaultMaxSize = new Vector3(0.9f,1f,1.1f);
    [SerializeField] private float _IncreaseSpeed = 1;

    private Vector3 _OriginalSize;
    private bool _MouseOver;

    void Start()
    {
        _OriginalSize = transform.localScale;
    }

    void Update()
    {
        switch (_UIEffect)
        {
            case UIEffectOptions.Grow:
                if (_MouseOver)
                {
                    if (transform.localScale.y < _MinDefaultMaxSize.z)
                        transform.localScale += new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed) * Time.deltaTime;
                }
                else
                    if (transform.localScale.y > _OriginalSize.y)
                    transform.localScale -= new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed) * Time.deltaTime;
                else
                    transform.localScale = new Vector3(_OriginalSize.y, _OriginalSize.z, _OriginalSize.z);
                break;
            case UIEffectOptions.Shrink:
                if (_MouseOver)
                {
                    if (transform.localScale.y > _MinDefaultMaxSize.x)
                        transform.localScale -= new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed) * Time.deltaTime;
                }
                else
                   if (transform.localScale.y < _OriginalSize.x)
                    transform.localScale += new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed) * Time.deltaTime;
                else
                    transform.localScale = new Vector3(_OriginalSize.x, _OriginalSize.y, _OriginalSize.z);
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _MouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _MouseOver = false;
    }
}