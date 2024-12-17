using System;
using System.Collections;
using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Camera _camera;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private GameObject _shield;
    [SerializeField] private Animator _animator;
    public Animator Animator => _animator;
    private Transform _transform;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private TrailRenderer _trail;
    [SerializeField] private float _originalTrailWidth;
    [SerializeField] private SkillManager _skillManager;

    private bool _moreThanOneTouch;
    
    private NinjaSettings _skin;
    private Vector2 _touchOrigin;
    private Vector2 _positionOrigin;
    
    private bool _hasShield;
    private bool _isInvulnerable;
    
    public const string ShieldKey = "Shield";

    private Transform _bottomLeftLimit;
    private Transform _topRightLimit;
    
    // Double tap
    public static event Action<Vector2> OnDoubleTap;
    private float _lastTapTime;
    private const float _doubleTapMaxDelay = 0.25f;

    private void OnEnable()
    {
        EventController.OnGameLost += OnGameLost;
        EventController.OnGameWon += OnGameWon;

        _skin = Menu.Instance.GetSelectedSkin();
        _trail.colorGradient = _skin.TrailColor;
        _animator.SetTrigger(_skin.ID);
        _skillManager.SetSkill(_skin.Skill);
    }
    
    private void OnDisable()
    {
        EventController.OnGameLost -= OnGameLost;
        EventController.OnGameWon -= OnGameWon;
    }
    
    public void SetLimits(Transform bottomLeft, Transform topRight)
    {
        _bottomLeftLimit = bottomLeft;
        _topRightLimit = topRight;
    }

    public int GetShield()
    {
        return PlayerPrefs.GetInt(ShieldKey, 0);
    }

    public void Setup()
    {
        _transform = transform;
        _transform.position = Vector2.zero;
        _positionOrigin = Vector2.zero;
        _hasShield = PlayerPrefs.GetInt(ShieldKey, 0) == 1;
        _shield.SetActive(_hasShield);
        _transform.rotation = Quaternion.identity;
        _animator.speed = 0;
        _trail.Clear();
        ResetTrail();
    }

    private void OnGameLost(int seconds, float exactTime)
    {
        PlayerPrefs.SetInt(ShieldKey, 0);
    }
    
    private void OnGameWon()
    {
        StopAllCoroutines();
        PlayerPrefs.SetInt(ShieldKey, 0);
    }
    
    private void Update()
    {
        Vector2 startPosition = _transform.position;
        Vector2 nextPosition = startPosition;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            _touchOrigin = _camera.ScreenToWorldPoint(Input.mousePosition);
            _positionOrigin = _transform.position;
            nextPosition = _positionOrigin;
            if (Time.time - _lastTapTime < _doubleTapMaxDelay)
            {
                OnDoubleTap?.Invoke(_touchOrigin);
                return;
            }
            else
            {
                _lastTapTime = Time.time;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 offset = (Vector2)_camera.ScreenToWorldPoint(Input.mousePosition) - _touchOrigin;
            nextPosition = _positionOrigin + offset;
        }
        else
        {
            _animator.speed = 0;
        }
        
#else

        if (Input.touchCount != 1)
        {
            _animator.speed = 0;
            _moreThanOneTouch = Input.touchCount > 1;
            return;
        }

        Touch touch = Input.GetTouch(0);
        bool began = touch.phase == TouchPhase.Began;
        if (began || _moreThanOneTouch)
        {
            _touchOrigin = _camera.ScreenToWorldPoint(touch.position);
            _positionOrigin = _transform.position;
            nextPosition = _positionOrigin;
            _moreThanOneTouch = false;
        
            if (began)
            {
                if (Time.time - _lastTapTime < _doubleTapMaxDelay)
                {
                    OnDoubleTap?.Invoke(_touchOrigin);
                    return;
                }
                else
                {
                    _lastTapTime = Time.time;
                }
            }
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            Vector2 offset = (Vector2)_camera.ScreenToWorldPoint(touch.position) - _touchOrigin;
            nextPosition = _positionOrigin + offset;
        }
        else
        {
            _animator.speed = 0;
        }
        
#endif
        _skillManager.EvaluateNextPosition(ref nextPosition, _bottomLeftLimit, _topRightLimit);
        //nextPosition.x = Mathf.Clamp(nextPosition.x, _bottomLeftLimit.position.x, _topRightLimit.position.x);
        //nextPosition.y = Mathf.Clamp(nextPosition.y, _bottomLeftLimit.position.y, _topRightLimit.position.y);
        _transform.position = nextPosition;
        
        var direction = nextPosition - startPosition;
        var magnitude = direction.magnitude;
        var movementSpeed = magnitude / Time.deltaTime;
        _animator.speed = Mathf.Clamp(movementSpeed / _maxSpeed, 0, 1);
        if (magnitude > 0.001f)
        {
            float targetAngle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
            float currentAngle = _transform.eulerAngles.z;
            float smoothAngle = Mathf.LerpAngle(currentAngle, targetAngle, _rotationSpeed * Time.deltaTime);
            _transform.rotation = Quaternion.Euler(0, 0, smoothAngle);
        }
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
            EventController.Instance.GameLost(SpawnController.Instance.ElapsedTime, SpawnController.Instance.ExactElapsedTime);
        }
    }

    public void ResetMovement()
    {
        _touchOrigin = _camera.ScreenToWorldPoint(Input.mousePosition);
        _positionOrigin = _transform.position;
        _trail.Clear();
    }

    public void ResetTrail()
    {
        _trail.widthMultiplier = _originalTrailWidth;
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
