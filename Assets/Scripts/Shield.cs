using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Gradient _gradient;
    [SerializeField] private float _timeScale;
    private float _time;
    
    
    private void OnEnable()
    {
        _time = 0;
        _spriteRenderer.color = _gradient.Evaluate(0);
    }

    private void Update()
    {
        _spriteRenderer.color = _gradient.Evaluate(_time);
        _time += Time.deltaTime * _timeScale;
        if (_time > 1) _time -= 1;
    }
}
