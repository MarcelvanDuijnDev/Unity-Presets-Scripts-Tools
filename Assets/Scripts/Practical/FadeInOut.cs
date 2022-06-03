using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    private enum Fade { In, Out }
    [SerializeField] private Fade _FadeOption = Fade.In;
    [SerializeField] private float _Duration = 0;

    [SerializeField] private Image _Image = null;

    private float _ChangeSpeed;
    private Color _Color;

    void Start()
    {
        if (_Image == null)
            _Image = GetComponent<Image>();

        if (_FadeOption == Fade.In)
            _Color = new Color(_Image.color.r, _Image.color.g, _Image.color.b, 0);
        else
            _Color = new Color(_Image.color.r, _Image.color.g, _Image.color.b, 1);

        _ChangeSpeed = 1 / _Duration;
    }

    void Update()
    {
        if (_FadeOption == Fade.In && _Color.a < 1)
        {
            _Color.a += _ChangeSpeed * Time.deltaTime;
        }
        if (_FadeOption == Fade.Out && _Color.a > 0)
        {
            _Color.a -= _ChangeSpeed * Time.deltaTime;
        }

        _Image.color = _Color;
    }

    public void SetFade(bool isfadein)
    {
        if (isfadein)
        {
            _FadeOption = Fade.In;
            _Color = new Color(_Image.color.r, _Image.color.g, _Image.color.b, 0);
        }
        else
        {
            _FadeOption = Fade.Out;
            _Color = new Color(_Image.color.r, _Image.color.g, _Image.color.b, 1);
        }
    }
}