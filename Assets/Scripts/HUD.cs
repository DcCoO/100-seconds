using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text _timeText;
    
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TMP_Text _loseText;
    [SerializeField] private string[] _losePhrases;
    
    [SerializeField] private TMP_Text _rankingText;
    [SerializeField] private AnimationCurve _playersCurve;
    private static readonly DateTime _beginDate = new DateTime(2024, 12, 14);
    private const int _startUsers = 41593;
    private const float _newUsersPerSecond = 0.006f;
    
    
    
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
    
    private void GameLost(int seconds, float exactTime)
    {
        _gameOverPanel.SetActive(true);
        _loseText.text = $"\"{_losePhrases[Random.Range(0, _losePhrases.Length)]}\"";

        float t = Mathf.Clamp(exactTime / 100f, 0f, 1f);
        float passedSecondsFromStartDate = (float)(DateTime.Now - _beginDate).TotalSeconds;
        int users = _startUsers + Mathf.RoundToInt(passedSecondsFromStartDate * _newUsersPerSecond);
        _rankingText.text = $"there are {Mathf.RoundToInt(users * _playersCurve.Evaluate(t))} ninjas ahead of you";
        
        _timeText.gameObject.SetActive(false);
        if (seconds > PlayerPrefs.GetInt(Menu.HighscoreKey, 0))
        {
            PlayerPrefs.SetInt(Menu.HighscoreKey, seconds);
            
            if (seconds == 1) _newHighscoreText.text = "New Highscore!\n1 second!";
            else _newHighscoreText.text = $"New Highscore!\n{seconds} seconds!";
        }
        else
        {
            _newHighscoreText.text = $"time alive: {seconds} seconds";
        }
    }
    
    private void GameWon()
    {
        _gameWonPanel.SetActive(true);
        _timeText.gameObject.SetActive(false);
        PlayerPrefs.SetInt(Menu.HighscoreKey, 100);
    }
}
