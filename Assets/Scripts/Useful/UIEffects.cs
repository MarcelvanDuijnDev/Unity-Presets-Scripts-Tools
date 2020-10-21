using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private enum UIEffectOptions { Grow }
    [SerializeField] private UIEffectOptions _UIEffect = UIEffectOptions.Grow;
    [SerializeField] private Vector2 _MinMaxSize = new Vector2(1,1.2f);
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
                    if (transform.localScale.x < _MinMaxSize.y)
                        transform.localScale += new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed) * Time.deltaTime;
                }
                else
                    if (transform.localScale.x > _OriginalSize.x)
                    transform.localScale -= new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed) * Time.deltaTime;
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
