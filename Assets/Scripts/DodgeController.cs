using TMPro;
using UnityEngine;

public class DodgeController : SingletonMonoBehaviour<DodgeController>
{
    [SerializeField] private Transform _player;
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _comboCanvas;
    [SerializeField] private TMP_Text _comboText;
    [SerializeField] private Gradient _comboGradient;
    [SerializeField] private float _comboDuration;
    [SerializeField] private float _comboOffsetY;
    [SerializeField] private Vector3 _comboCanvasOffset;
    private float _comboBeginTime;
    private bool _isMakingCombo;
    private int _comboCount;
    public int ComboCount => _comboCount;

    private void OnEnable()
    {
        _comboCount = 0;
    }
    
    public void Dodge()
    {
        _isMakingCombo = true;
        _comboBeginTime = Time.time;
        _comboText.transform.localPosition = Vector3.zero;
        _comboCanvas.position = _player.position + _comboCanvasOffset;
        _comboText.text = $"x<b>{++_comboCount}</b>";
        AudioManager.Instance.PlayDodge();
        EventController.Instance.Dodge(_comboCount);
    }

    private void Update()
    {
        if (!_isMakingCombo) return;
        
        float t = (Time.time - _comboBeginTime) / _comboDuration;
        _comboCanvas.rotation = _camera.transform.rotation;
        _comboCanvas.position = _player.position + _comboCanvasOffset;
        _comboText.color = _comboGradient.Evaluate(t);
        _comboText.transform.localPosition = Vector3.up * (t * _comboOffsetY);
        if (t >= 1)
        {
            _isMakingCombo = false;
            _comboText.color = Color.clear;
        }
    }
}