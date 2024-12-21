using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private int _id;
    public int ID => _id;
    
    protected Player _player;
    protected Transform _transform;
    private EventController _eventController;

    [SerializeField] protected Rigidbody2D _rigidbody2D;
    [SerializeField] protected float _speed;
    [HideInInspector] public float DeathTime;
    [SerializeField] private float _lifetime;
    
    // Close dodge pulse
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _pulseDuration;
    [SerializeField] private Color _pulseColor;
    private float _pulseBeginTime;
    private bool _isPulsing;
    private bool _hasPulsed;
    
    private int _playerLayer;

    protected virtual void FixedUpdate()
    {
        if (Time.time > DeathTime) Die();
        if (_isPulsing)
        {
            float t = (Time.time - _pulseBeginTime) / _pulseDuration;
            _spriteRenderer.color = Color.Lerp(Color.white, _pulseColor, Mathf.Sin(t * Mathf.PI));
            if (t >= 1)
            {
                _isPulsing = false;
                _spriteRenderer.color = Color.white;
            }
        }
    }

    public virtual void OnSpawn(Vector3 initialPosition)
    {
        _player = Player.Instance;
        _transform = transform;
        _eventController = EventController.Instance;
        _transform.position = initialPosition;
        DeathTime = Time.time + _lifetime;
        _playerLayer = LayerMask.NameToLayer("Player");
        
        // Clean pulse
        _spriteRenderer.color = Color.white;
        _hasPulsed = false;
        _isPulsing = false;
    }

    public virtual void OnRespawn(Vector3 initialPosition)
    {
        _transform.position = initialPosition;
        DeathTime = Time.time + _lifetime;
        gameObject.SetActive(true);
        
        // Clean pulse
        _spriteRenderer.color = Color.white;
        _hasPulsed = false;
        _isPulsing = false;
    }

    public void Die()
    {
        _eventController.EnemyDestroyed(this);
        gameObject.SetActive(false);
        // TODO: play VFX
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (_hasPulsed) return;
        if (other.gameObject.layer != _playerLayer) return;

        if (_player.IsDead) return;
        _isPulsing = true;
        _hasPulsed = true;
        _pulseBeginTime = Time.time;
        var playerPosition = _player.transform.position;
        var point = (_transform.position - playerPosition).normalized * 0.16f;
        VFXController.Instance.Dodge(playerPosition + point);
        DodgeController.Instance.Dodge();
    }
}
