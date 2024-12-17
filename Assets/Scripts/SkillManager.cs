using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private static readonly int _usingSkill = Animator.StringToHash("usingSkill");
    private ESkill _currentSkill;

    [SerializeField] private float _mouseDuration;
    [SerializeField] private float _mouseCooldown;
    [SerializeField] private float _originalColliderRadius;
    [SerializeField] private float _mouseColliderRadius;
    [SerializeField] private float _mouseTrailWidth;
    
    [SerializeField] private float _teleportCooldown;

    [SerializeField] private float _disableDuration;
    [SerializeField] private float _disableCooldown;

    private bool _isUsingSkill;
    private float _skillEndTime;
    
    private void OnEnable()
    {
        Player.OnDoubleTap += OnDoubleTap;
        EventController.OnGameLost += OnGameLost;
        EventController.OnGameWon += OnGameWon;
        _skillEndTime = 0;
    }

    private void OnDisable()
    {
        Player.OnDoubleTap -= OnDoubleTap;
        EventController.OnGameLost -= OnGameLost;
        EventController.OnGameWon -= OnGameWon;
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
        player.Animator.SetBool(_usingSkill, true);
        
        switch (_currentSkill)
        {
            case ESkill.Disable:
                VFXController.Instance.FieldSmoke();
                SpawnController.Instance.Freeze(_disableDuration);
                _skillEndTime = Time.time + _disableCooldown;
                _isUsingSkill = true;
                break;
            case ESkill.Mouse:
                VFXController.Instance.Smoke(player.transform.position);
                player.GetComponent<CircleCollider2D>().radius = _mouseColliderRadius;
                player.GetComponent<TrailRenderer>().widthMultiplier = _mouseTrailWidth;
                _skillEndTime = Time.time + _mouseDuration;
                _isUsingSkill = true;
                break;
            case ESkill.Teleport:
                VFXController.Instance.Smoke(player.transform.position);
                player.transform.position = tapWorldPosition;
                player.ResetMovement();
                break;
        }
    }

    public void EvaluateNextPosition(ref Vector2 nextPosition, Transform bottomLeftLimit, Transform topRightLimit)
    {
        if (_currentSkill == ESkill.Boundless)
        {
            if (nextPosition.x < bottomLeftLimit.position.x)
            {
                nextPosition.x = topRightLimit.position.x - (bottomLeftLimit.position.x - nextPosition.x) % (topRightLimit.position.x - bottomLeftLimit.position.x);
            }
            else if (nextPosition.x > topRightLimit.position.x)
            {
                nextPosition.x = bottomLeftLimit.position.x + (nextPosition.x - topRightLimit.position.x) % (topRightLimit.position.x - bottomLeftLimit.position.x);
            }
            if (nextPosition.y < bottomLeftLimit.position.y)
            {
                nextPosition.y = topRightLimit.position.y - (bottomLeftLimit.position.y - nextPosition.y) % (topRightLimit.position.y - bottomLeftLimit.position.y);
            }
            else if (nextPosition.y > topRightLimit.position.y)
            {
                nextPosition.y = bottomLeftLimit.position.y + (nextPosition.y - topRightLimit.position.y) % (topRightLimit.position.y - bottomLeftLimit.position.y);
            }
        }
        else
        {
            nextPosition.x = Mathf.Clamp(nextPosition.x, bottomLeftLimit.position.x, topRightLimit.position.x);
            nextPosition.y = Mathf.Clamp(nextPosition.y, bottomLeftLimit.position.y, topRightLimit.position.y);
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
                if (!gameEnd) VFXController.Instance.Smoke(player.transform.position);
                break;
            case ESkill.Teleport:
                break;
        }
    }
}
