using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject _field;

    //[HideInInspector] public bool IsPlaying;

    //private int _elapsedTime;

    private void OnEnable()
    {
        EventController.OnGameStart += OnGameStart;
        EventController.OnGameLost += OnGameLost;
        EventController.OnGameWon += OnGameWon;
        //EventController.OnSecondElapsed += UpdateElapsedTime;
    }
    
    private void OnDisable()
    {
        EventController.OnGameStart -= OnGameStart;
        EventController.OnGameLost -= OnGameLost;
        EventController.OnGameWon -= OnGameWon;
        //EventController.OnSecondElapsed -= UpdateElapsedTime;
    }
    
    private void OnGameStart()
    {
        _field.SetActive(true);
        Player.Instance.Setup();
    }
    
    private void OnGameWon()
    {
        _field.SetActive(false);
    }
    
    private void OnGameLost(int seconds, float exactTime)
    {
        _field.SetActive(false);
    }
    
    //private void UpdateElapsedTime(int seconds) => _elapsedTime = seconds;
}
