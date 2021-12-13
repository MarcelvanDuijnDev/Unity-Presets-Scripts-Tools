using UnityEngine;
using UnityEngine.EventSystems;

public class UIEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private enum UIEffectOptions { Grow, Shrink }
    [Header("Effects")]
    [SerializeField] private UIEffectOptions _UIEffect = UIEffectOptions.Grow;

    [Header("Scaling Options")]
    [SerializeField] private bool _RelativeToOriginalSize = true;
    [SerializeField] private float _IncreaseSpeed = 1;

    [Header("Minimal Size:")]
    [SerializeField] private float _MinimalSize = 0.9f;

    [Header("Maximal Size:")]
    [SerializeField] private float _MaximalSize = 1.1f;

    private Vector3 _OriginalSize;
    private bool _MouseOver;

    void Start()
    {
        _OriginalSize = transform.localScale;

        if (_RelativeToOriginalSize)
        {
            _MinimalSize = _OriginalSize.y * _MinimalSize;
            _MaximalSize = _OriginalSize.y * _MaximalSize;
            _IncreaseSpeed = _IncreaseSpeed * ((_OriginalSize.x + _OriginalSize.y + _OriginalSize.z) / 3);
        }
    }

    void Update()
    {
        switch (_UIEffect)
        {
            case UIEffectOptions.Grow:
                if (_MouseOver)
                {
                    if (transform.localScale.y < _MaximalSize)
                        transform.localScale += new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed) * Time.deltaTime;
                }
                else
                    if (transform.localScale.y > _OriginalSize.y)
                    transform.localScale -= new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed) * Time.deltaTime;
                else
                    transform.localScale = new Vector3(_OriginalSize.x, _OriginalSize.y, _OriginalSize.z);
                break;
            case UIEffectOptions.Shrink:
                if (_MouseOver)
                {
                    if (transform.localScale.y > _MinimalSize)
                        transform.localScale -= new Vector3(_IncreaseSpeed, _IncreaseSpeed, _IncreaseSpeed) * Time.deltaTime;
                }
                else
                   if (transform.localScale.y < _OriginalSize.y)
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