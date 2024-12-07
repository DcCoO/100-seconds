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
    
    protected virtual void OnDisable()
    {
        _eventController.EnemyDestroyed(this);
    }

    protected virtual void FixedUpdate()
    {
        if (Time.time > DeathTime) Die();
    }

    public virtual void OnSpawn(Vector3 initialPosition)
    {
        _player = Player.Instance;
        _transform = transform;
        _eventController = EventController.Instance;
        _transform.position = initialPosition;
        DeathTime = Time.time + _lifetime;
    }

    public virtual void OnRespawn(Vector3 initialPosition)
    {
        _transform.position = initialPosition;
        DeathTime = Time.time + _lifetime;
        gameObject.SetActive(true);
    }

    private void Die()
    {
        _eventController.EnemyDestroyed(this);
        gameObject.SetActive(false);
        // TODO: play VFX
    }
}
