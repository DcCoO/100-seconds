using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject _field;

    [HideInInspector] public bool IsPlaying;

    private void OnEnable()
    {
        EventController.OnGameStart += OnGameStart;
        EventController.OnGameLost += OnGameEnd;
        EventController.OnGameWon += OnGameEnd;
    }
    
    private void OnDisable()
    {
        EventController.OnGameStart -= OnGameStart;
        EventController.OnGameLost -= OnGameEnd;
        EventController.OnGameWon -= OnGameEnd;
    }
    
    private void OnGameStart()
    {
        _field.SetActive(true);
        IsPlaying = true;
        Player.Instance.Setup();
    }
    
    private void OnGameEnd()
    {
        _field.SetActive(false);
        IsPlaying = false;
    }

    private void Update()
    {
        if (IsPlaying && Input.GetMouseButtonUp(0))
        {
            EventController.Instance.GameLost();
        }
    }
}
