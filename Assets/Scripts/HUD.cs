using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text _timeText;
    
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _gameWonPanel;
    
    private void OnEnable()
    {
        EventController.OnGameStart += OnStartGame;
        EventController.OnSecondElapsed += OnSecondElapsed;
        EventController.OnGameLost += GameLost;
        EventController.OnGameWon += GameWon;
    }

    private void OnDisable()
    {
        EventController.OnGameStart -= OnStartGame;
        EventController.OnSecondElapsed -= OnSecondElapsed;
        EventController.OnGameLost -= GameLost;
        EventController.OnGameWon -= GameWon;
    }

    private void OnStartGame()
    {
        _timeText.gameObject.SetActive(true);
    }
    
    private void OnSecondElapsed(int seconds)
    {
        _timeText.text = $"{100 - seconds}";
    }
    
    private void GameLost()
    {
        _gameOverPanel.SetActive(true);
        _timeText.gameObject.SetActive(false);
    }
    
    private void GameWon()
    {
        _gameWonPanel.SetActive(true);
        _timeText.gameObject.SetActive(false);
    }
}
