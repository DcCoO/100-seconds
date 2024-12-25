using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private static readonly int _usingSkill = Animator.StringToHash("usingSkill");
    private ESkill _currentSkill;

    [SerializeField] private float _mouseDuration;
    [SerializeField] private float _originalColliderRadius;
    [SerializeField] private float _mouseColliderRadius;
    [SerializeField] private float _mouseTrailWidth;
    
    [SerializeField] private float _teleportCooldown;

    [SerializeField] private float _disableDuration;

    private bool _isUsingSkill;
    public bool IsUsingSkill => _isUsingSkill;
    private float _skillEndTime;
    
    private void OnEnable()
    {
        Player.OnDoubleTap += OnDoubleTap;
        EventController.OnGameLost += OnGameLost;
        EventController.OnGameWon += OnGameWon;
        EventController.OnDodge += OnDodge;
        _skillEndTime = 0;
    }

    private void OnDisable()
    {
        Player.OnDoubleTap -= OnDoubleTap;
        EventController.OnGameLost -= OnGameLost;
        EventController.OnGameWon -= OnGameWon;
        EventController.OnDodge -= OnDodge;
    }

    private void Update()
    {
        if (!_isUsingSkill) return;
        
        if (Time.time > _skillEndTime)
        {
            CancelSkill();
        }
    }

    public void SetSkill(ESkill skill)
    {
        _currentSkill = skill;
    }
    
    private void OnDoubleTap(Vector2 tapWorldPosition)
    {
        if (_isUsingSkill) return;
        
        var player = Player.Instance;

        if (!player.CanUseSkill) return;
        
        player.Animator.SetBool(_usingSkill, true);
        EventController.Instance.SkillUsed();
        
        switch (_currentSkill)
        {
            case ESkill.Disable:
                VFXController.Instance.FieldSmoke();
                AudioManager.Instance.PlayDisablerSkill();
                SpawnController.Instance.Freeze(_disableDuration);
                _skillEndTime = Time.time + _disableDuration;
                _isUsingSkill = true;
                break;
            case ESkill.Mouse:
                VFXController.Instance.Smoke(player.transform.position);
                AudioManager.Instance.PlayBecomeMouse();
                player.GetComponent<CircleCollider2D>().radius = _mouseColliderRadius;
                player.GetComponent<TrailRenderer>().widthMultiplier = _mouseTrailWidth;
                _skillEndTime = Time.time + _mouseDuration;
                _isUsingSkill = true;
                break;
            case ESkill.Teleport:
                VFXController.Instance.Smoke(player.transform.position);
                AudioManager.Instance.PlayTeleport();
                player.transform.position = tapWorldPosition;
                player.ResetMovement();
                break;
        }
    }

    private void OnDodge(int combo)
    {
        if (_currentSkill == ESkill.Mouse)
        {
            _skillEndTime = Time.time + _mouseDuration;
        }
    }

    public bool EvaluateNextPosition(ref Vector2 nextPosition, Transform bottomLeftLimit, Transform topRightLimit)
    {
        if (_currentSkill == ESkill.Boundless)
        {
            bool crossedWall = false;
            if (nextPosition.x < bottomLeftLimit.position.x)
            {
                nextPosition.x = topRightLimit.position.x - (bottomLeftLimit.position.x - nextPosition.x) % (topRightLimit.position.x - bottomLeftLimit.position.x);
                crossedWall = true;
            }
            else if (nextPosition.x > topRightLimit.position.x)
            {
                nextPosition.x = bottomLeftLimit.position.x + (nextPosition.x - topRightLimit.position.x) % (topRightLimit.position.x - bottomLeftLimit.position.x);
                crossedWall = true;
            }
            if (nextPosition.y < bottomLeftLimit.position.y)
            {
                nextPosition.y = topRightLimit.position.y - (bottomLeftLimit.position.y - nextPosition.y) % (topRightLimit.position.y - bottomLeftLimit.position.y);
                crossedWall = true;
            }
            else if (nextPosition.y > topRightLimit.position.y)
            {
                nextPosition.y = bottomLeftLimit.position.y + (nextPosition.y - topRightLimit.position.y) % (topRightLimit.position.y - bottomLeftLimit.position.y);
                crossedWall = true;
            }
            
            nextPosition.x = Mathf.Clamp(nextPosition.x, bottomLeftLimit.position.x, topRightLimit.position.x);
            nextPosition.y = Mathf.Clamp(nextPosition.y, bottomLeftLimit.position.y, topRightLimit.position.y);
            //if (crossedWall) AudioManager.Instance.PlayBoundlessSkill();
            return crossedWall;
        }
        else
        {
            nextPosition.x = Mathf.Clamp(nextPosition.x, bottomLeftLimit.position.x, topRightLimit.position.x);
            nextPosition.y = Mathf.Clamp(nextPosition.y, bottomLeftLimit.position.y, topRightLimit.position.y);
            return false;
        }
    }
    
    private void OnGameLost(int seconds, float exactTime)
    {
        _currentSkill = ESkill.None;
        CancelSkill(true);
    }
    
    private void OnGameWon()
    {
        _currentSkill = ESkill.None;
        CancelSkill(true);
    }
    
    private void CancelSkill(bool gameEnd = false)
    {
        var player = Player.Instance;
        _isUsingSkill = false;
        
        player.Animator.SetBool(_usingSkill, false);
        
        switch (_currentSkill)
        {
            case ESkill.Disable:
                break;
            case ESkill.Mouse:
                player.GetComponent<CircleCollider2D>().radius = _originalColliderRadius;
                player.ResetTrail();
                if (!gameEnd)
                {
                    AudioManager.Instance.PlayBecomeHuman();
                    VFXController.Instance.Smoke(player.transform.position);
                }
                break;
            case ESkill.Teleport:
                break;
        }
    }
}
