using System;
using System.Collections;
using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    private static readonly int _speed = Animator.StringToHash("speed");
    
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Camera _camera;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private GameObject _shield;
    [SerializeField] private Animator _animator;
    private Transform _transform;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _maxSpeed;
    
    private Vector2 _touchOrigin;
    private Vector2 _positionOrigin;
    
    private int _elapsedTime;
    private bool _hasShield;
    private bool _isInvulnerable;
    
    public const string ShieldKey = "Shield";

    //private float _stepLength;

    private void OnEnable()
    {
        EventController.OnSecondElapsed += UpdateElapsedTime;
        EventController.OnGameLost += OnGameLost;
        EventController.OnGameWon += OnGameWon;
    }
    
    private void OnDisable()
    {
        EventController.OnSecondElapsed -= UpdateElapsedTime;
        EventController.OnGameLost -= OnGameLost;
        EventController.OnGameWon -= OnGameWon;
    }

    public void Setup()
    {
        _transform = transform;
        //_stepLength = _speed * Time.fixedDeltaTime;
#if UNITY_EDITOR
        _touchOrigin = _camera.ScreenToWorldPoint(Input.mousePosition);
#else
        _touchOrigin = _camera.ScreenToWorldPoint(Input.GetTouch(0).position);
#endif
        _transform.position = Vector2.zero;
        _positionOrigin = Vector2.zero;
        _hasShield = PlayerPrefs.GetInt(ShieldKey, 0) == 1;
        _shield.SetActive(_hasShield);
    }
    
    private void UpdateElapsedTime(int seconds) => _elapsedTime = seconds;

    private void OnGameLost(int seconds)
    {
        PlayerPrefs.SetInt(ShieldKey, 0);
    }
    
    private void OnGameWon()
    {
        StopAllCoroutines();
        PlayerPrefs.SetInt(ShieldKey, 0);
    }

    private int maxSpeed = 0;
    private void Update()
    {
        Vector2 currentPosition = _transform.position;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            _touchOrigin = _camera.ScreenToWorldPoint(Input.mousePosition);
            _positionOrigin = _transform.position;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 offset = (Vector2)_camera.ScreenToWorldPoint(Input.mousePosition) - _touchOrigin;
            _transform.position = _positionOrigin + offset;
        }
#else
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            _touchOrigin = _camera.ScreenToWorldPoint(touch.position);
            _positionOrigin = _transform.position;
        }
        else 
        {
            Vector2 offset = (Vector2)_camera.ScreenToWorldPoint(touch.position) - _touchOrigin;
            _transform.position = _positionOrigin + offset;
        }
#endif

        var direction = (Vector2)_transform.position - currentPosition;
        var magnitude = direction.magnitude;
        var movementSpeed = magnitude / Time.deltaTime;
        _animator.SetFloat(_speed, movementSpeed);
        _animator.speed = Mathf.Clamp(movementSpeed / _maxSpeed, 0, 1);
        if (magnitude > 0.001f) _transform.up = Vector3.Slerp(_transform.up, direction.normalized, _rotationSpeed * Time.deltaTime);
    }
    
    void RotateTowardsTarget(Vector2 targetDirection)
    {
        // Obtém o ângulo atual do player
        float currentAngle = Mathf.Atan2(transform.up.y, transform.up.x) * Mathf.Rad2Deg;

        // Calcula o ângulo desejado com base na direção alvo
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        // Interpola suavemente o ângulo atual para o ângulo alvo
        float smoothedAngle = Mathf.LerpAngle(currentAngle, targetAngle, _rotationSpeed * Time.deltaTime);

        // Aplica a rotação suavizada ao player
        transform.rotation = Quaternion.Euler(0f, 0f, smoothedAngle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.A)) return;
#endif


        TakeHit();
    }

    private void TakeHit()
    {
        if (_isInvulnerable) return;
        
        if (_hasShield)
        {
            _hasShield = false;
            _shield.SetActive(false);
            StartCoroutine(InvulnerableRoutine());
        }
        else
        {
            EventController.Instance.GameLost(_elapsedTime);
        }
    }

    private IEnumerator InvulnerableRoutine()
    {
        _isInvulnerable = true;
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            _spriteRenderer.enabled = !_spriteRenderer.enabled;
            yield return null;
        }
        _spriteRenderer.enabled = true;
        _isInvulnerable = false;
    }
}
