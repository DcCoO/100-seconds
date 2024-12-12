using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text _timeText;
    
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TMP_Text _loseText;
    [SerializeField] private string[] _losePhrases;
    
    
    [SerializeField] private GameObject _gameWonPanel;
    
    [SerializeField] private TMP_Text _newHighscoreText;
    
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
        _timeText.text = "100";
    }
    
    private void OnSecondElapsed(int seconds)
    {
        _timeText.text = $"{100 - seconds}";
    }
    
    private void GameLost(int seconds)
    {
        _gameOverPanel.SetActive(true);
        _loseText.text = _losePhrases[Random.Range(0, _losePhrases.Length)];
        _timeText.gameObject.SetActive(false);
        if (seconds > PlayerPrefs.GetInt(Menu.HighscoreKey, 0))
        {
            PlayerPrefs.SetInt(Menu.HighscoreKey, seconds);
            _newHighscoreText.gameObject.SetActive(true);
            
            if (seconds == 1) _newHighscoreText.text = "New Highscore!\n1 second!";
            else _newHighscoreText.text = $"New Highscore!\n{seconds} seconds!";
        }
        else _newHighscoreText.gameObject.SetActive(false);
    }
    
    private void GameWon()
    {
        _gameWonPanel.SetActive(true);
        _timeText.gameObject.SetActive(false);
        PlayerPrefs.SetInt(Menu.HighscoreKey, 100);
    }
}
