using UnityEngine;

public class ChaserEnemy : Enemy
{
    [Range(0f, 5f)] public float MaxAngleChangePerFrame;
    [SerializeField] private bool _lookAtDirection;
    
    private Vector3 _direction;

    public override void OnSpawn(Vector3 initialPosition)
    {
        base.OnSpawn(initialPosition);
        _direction = (_player.transform.position - initialPosition).normalized;
    }
    
    public override void OnRespawn(Vector3 initialPosition)
    {
        base.OnRespawn(initialPosition);
        _direction = (_player.transform.position - initialPosition).normalized;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        _rigidbody2D.MovePosition(_transform.position + _direction * (_speed * Time.fixedDeltaTime));
        var newDirection = _player.transform.position - transform.position;
        
        var angle = Vector3.SignedAngle(_direction, newDirection, Vector3.forward);
        var angleChange = Mathf.Clamp(angle, -MaxAngleChangePerFrame, MaxAngleChangePerFrame);
        _direction = (Quaternion.Euler(0, 0, angleChange) * _direction).normalized;
        
        if (_lookAtDirection) _transform.up = _direction;
    }
}