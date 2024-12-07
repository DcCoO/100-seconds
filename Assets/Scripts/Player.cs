using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    private Transform _transform;
    [SerializeField] private float _speed;

    private float _stepLength;
    
    public void Setup()
    {
        _transform = transform;
        _stepLength = _speed * Time.fixedDeltaTime;
        var mouseWorldPosition = (Vector2)_camera.ScreenToWorldPoint(Input.mousePosition);
        _transform.position = mouseWorldPosition;
    }

    private void FixedUpdate()
    {
        Vector2 currentPosition = _transform.position;
        Vector2 mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        var difference = mouseWorldPosition - currentPosition;
        var direction = difference.normalized;
        
        if (difference.magnitude < _stepLength)
        {
            _rigidbody2D.MovePosition(currentPosition + difference);
        }
        else
        {
            _rigidbody2D.MovePosition(currentPosition + direction * _stepLength);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EventController.Instance.GameLost();
    }
}
