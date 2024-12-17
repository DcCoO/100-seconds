using System;
using UnityEngine;

public class EventController : SingletonMonoBehaviour<EventController>
{
    public static event Action OnGameStart;
    public static event Action<int> OnSecondElapsed;
    public static event Action<Enemy> OnEnemyDestroyed;
    public static event Action<int, float> OnGameLost;
    public static event Action OnGameWon;
    public static event Action OnAdWatched;

    public void StartGame()
    {
#if UNITY_EDITOR
        Print(nameof(StartGame), "Game has started!", Color.cyan);
#endif
        OnGameStart?.Invoke();
    }
    
    public void ElapseSecond(int seconds)
    {
        OnSecondElapsed?.Invoke(seconds);
    }
    
    public void EnemyDestroyed(Enemy enemy)
    {
        OnEnemyDestroyed?.Invoke(enemy);
    }
    
    public void GameLost(int time, float exactTime)
    {
#if UNITY_EDITOR
        Print(nameof(GameLost), $"Game has been lost after {time} sconds!", Color.red);
#endif
        OnGameLost?.Invoke(time, exactTime);
    }
    
    public void GameWon()
    {
#if UNITY_EDITOR
        Print(nameof(GameWon), "Game has been won!", Color.green);
#endif
        OnGameWon?.Invoke();
    }
    
    public void AdWatched()
    {
        OnAdWatched?.Invoke();
    }
    
#if UNITY_EDITOR
    private void Print(string eventName, string eventMessage, Color color)
    {
        var hexColor = ColorUtility.ToHtmlStringRGB(color);
        Debug.Log($"<color=#{hexColor}><b>{eventName}</b></color> {eventMessage}");
    }
#endif
}
