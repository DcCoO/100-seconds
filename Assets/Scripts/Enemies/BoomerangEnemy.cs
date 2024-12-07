using UnityEngine;

public class BoomerangEnemy : Enemy
{
    private enum BoomerangState
    {
        Entering,
        Spinning,
        Leaving
    }

    [SerializeField] private float _spinningDuration;
    private BoomerangState _state;
    private Vector2 _targetPosition;
    private Vector2 _targetDirection;

    private float _spinningCountdown;

    public override void OnSpawn(Vector3 initialPosition)
    {
        base.OnSpawn(initialPosition);
        SetupEntering();
    }

    public override void OnRespawn(Vector3 initialPosition)
    {
        base.OnRespawn(initialPosition);
        SetupEntering();
    }

    private void SetupEntering()
    {
        _state = BoomerangState.Entering;
        _targetPosition = _player.transform.position;
        _targetDirection = (_targetPosition - (Vector2)transform.position).normalized;
        _spinningCountdown = _spinningDuration;
    }

    private void SetupSpinning()
    {
        _state = BoomerangState.Spinning;
    }
    
    private void SetupLeaving()
    {
        _state = BoomerangState.Leaving;
        _targetDirection = (_player.transform.position - transform.position).normalized;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        switch (_state)
        {
            case BoomerangState.Entering:
                var predictedPosition = (Vector2)transform.position + _targetDirection * (_speed * Time.fixedDeltaTime);
                var currentDistance = _targetPosition - (Vector2)transform.position;
                var predictedDistance = _targetPosition - predictedPosition;
                
                if (predictedDistance.sqrMagnitude > currentDistance.sqrMagnitude) SetupSpinning();
                else _rigidbody2D.MovePosition(predictedPosition);
                break;
            
            case BoomerangState.Spinning:
                _spinningCountdown -= Time.fixedDeltaTime;
                if (_spinningCountdown <= 0) SetupLeaving();
                break;
            case BoomerangState.Leaving:
                _rigidbody2D.MovePosition((Vector2)transform.position + _targetDirection * (_speed * Time.fixedDeltaTime));
                break;
        }
    }
}
