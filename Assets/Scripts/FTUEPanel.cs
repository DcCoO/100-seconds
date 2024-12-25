using System;
using UnityEngine;

public class FTUEPanel : MonoBehaviour
{
    private Action _onStartGame;
    
    public void Setup(Action onStartGame)
    {
        _onStartGame = onStartGame;
        AudioManager.Instance.PlayEldest();
    }
    
    public void OnTap()
    {
        Menu.SetFTUE();
        _onStartGame?.Invoke();
        Destroy(gameObject);
        AudioManager.Instance.StopAllSounds();
    }
}
