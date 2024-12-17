using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnController : SingletonMonoBehaviour<SpawnController>
{
    private class SpawnTimestamp
    {
        public int Index;
        public float Time;

        public SpawnTimestamp(int index, float time)
        {
            Index = index;
            Time = time;
        }
    }
    
    public bool IsSpawning;
    
    public SpawnSettings[] SpawnSettings;
    private SpawnTimestamp[] _spawnTimestamps;
    
    private float _spawnBeginTime;
    private int _elapsedTime;
    private float _exactElapsedTime;
    
    public int ElapsedTime => _elapsedTime;
    public float ExactElapsedTime => _exactElapsedTime;

    private float _lastModulo;
    private int _currentIndex;
    private int _spawnCount;
    private float _freezeEndTime;

    [SerializeField] private Transform _field;
    [SerializeField] private Transform _bottomLeft;
    [SerializeField] private Transform _topRight;
    [SerializeField, Range(1f, 2f)] private float _offsetMultiplier = 1.4f;

    private readonly Dictionary<int, Queue<Enemy>> _enemiesInGame = new();
    private readonly Dictionary<int, Queue<Enemy>> _enemiesPool = new();
    
    private EventController _eventController;

    private void OnEnable()
    {
        _eventController = EventController.Instance;
        EventController.OnGameStart += StartSpawning;
        EventController.OnEnemyDestroyed += RemoveEnemyFromGame;
        EventController.OnGameLost += Stop;
        EventController.OnGameWon += Stop;
    }

    private void OnDisable()
    {
        EventController.OnGameStart -= StartSpawning;
        EventController.OnEnemyDestroyed -= RemoveEnemyFromGame;
        EventController.OnGameLost -= Stop;
        EventController.OnGameWon -= Stop;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.green;   
        foreach (var enemies in _enemiesInGame.Values)
        {
            foreach (var enemy in enemies)
            {
                Gizmos.DrawWireCube(enemy.transform.position, 0.32f * Vector3.one);
            }
        }
    }
#endif
    
    private void Update()
    {
        if (!IsSpawning) return;
        
        _exactElapsedTime = Time.time - _spawnBeginTime;

        if (TryFinishGame(_exactElapsedTime)) return;
        
        var currentModulo = _exactElapsedTime % 1f;
        
        if (currentModulo < _lastModulo) // New second
        {
            _elapsedTime = Mathf.FloorToInt(_exactElapsedTime);
            ScheduleSpawns(_elapsedTime);
            _eventController.ElapseSecond(_elapsedTime);
        }
        
        if (Time.time < _freezeEndTime) return;
        
        while (_currentIndex < _spawnCount && currentModulo >= _spawnTimestamps[_currentIndex].Time)
        {
            Spawn(SpawnSettings[_spawnTimestamps[_currentIndex++].Index]);
        }
        
        _lastModulo = currentModulo;
    }

    public void Freeze(float duration)
    {
        _freezeEndTime = Time.time + duration;
        List<Enemy> enemiesInGame = _enemiesInGame.SelectMany(kv => kv.Value).ToList();
        foreach (var enemy in enemiesInGame) enemy.Die();
    }
    
    private bool TryFinishGame(float currentTime)
    {
        if (currentTime < 100f) return false;
        _eventController.GameWon();
        IsSpawning = false;
        return true;
    }

    private void InitPools()
    {
        foreach (var spawnSetting in SpawnSettings)
        {
            _enemiesPool[spawnSetting.EnemyPrefab.ID] = new Queue<Enemy>();
            _enemiesInGame[spawnSetting.EnemyPrefab.ID] = new Queue<Enemy>();
        }
    }

    private void StartSpawning()
    {
        var mainCamera = Camera.main!;
        
        // Initialize pools
        InitPools();
        
        // Set the corners of the screen
        _bottomLeft.position = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
        _topRight.position = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        
        Player.Instance.SetLimits(_bottomLeft, _topRight);
        
        _spawnBeginTime = Time.time;
        _lastModulo = -1;
        _freezeEndTime = _spawnBeginTime - 1f;
        _spawnCount = SpawnSettings.Length;
        _spawnTimestamps = new SpawnTimestamp[_spawnCount];
        ScheduleSpawns(0);
        IsSpawning = true;
    }

    private void ScheduleSpawns(int second)
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            var spawnChance = SpawnSettings[i].GetSpawnChance(second);
            _spawnTimestamps[i] = new SpawnTimestamp(i, Random.value < spawnChance ? Random.value : float.MaxValue);
        }
        
        //Sort spawn time by Time
        Array.Sort(_spawnTimestamps, (a, b) => a.Time.CompareTo(b.Time));
        _currentIndex = 0;
        
    }
    
    private void Spawn(SpawnSettings spawnSettings)
    {
        int enemyID = spawnSettings.EnemyPrefab.ID;
        var queue = _enemiesInGame[enemyID];
        
        // Check if another instance of the same enemy can be spawned
        if (queue.Count >= spawnSettings.MaxInstancesOnScreen) return;

        var fromPool = TryGetEnemy(spawnSettings, out var enemy);
        queue.Enqueue(enemy);
        var position = GetRandomPointOnBorder();
        if (fromPool) enemy.OnRespawn(position);
        else enemy.OnSpawn(position);
        
    }
    
    /// <summary>Returns true if the enemy was taken from the pool, otherwise Instantiate it and return false.</summary>
    private bool TryGetEnemy(SpawnSettings spawnSettings, out Enemy enemy)
    {
        var queue = _enemiesPool[spawnSettings.EnemyPrefab.ID];
        if (queue.Count > 0)
        {
            enemy = queue.Dequeue();
            return true;
        }
        enemy = Instantiate(spawnSettings.EnemyPrefab, _field);
        return false;
    }

    private void RemoveEnemyFromGame(Enemy enemy)
    {
        _enemiesInGame[enemy.ID].Dequeue();
        _enemiesPool[enemy.ID].Enqueue(enemy);
    }

    /*private void DictToString()
    {
        string s = "[";
        for (int i = 1; i < 10; ++i) s += $" ({i} : {_enemiesInGame[i].Count})  ";
        print($"{s} )");
    }*/
    
    private Vector2 GetRandomPointOnBorder()
    {
        return Random.Range(0, 4) switch
        {
            0 => new Vector2(Random.Range(_bottomLeft.position.x, _topRight.position.x), _bottomLeft.position.y),
            1 => new Vector2(Random.Range(_bottomLeft.position.x, _topRight.position.x), _topRight.position.y),
            2 => new Vector2(_bottomLeft.position.x, Random.Range(_bottomLeft.position.y, _topRight.position.y)),
            3 => new Vector2(_topRight.position.x, Random.Range(_bottomLeft.position.y, _topRight.position.y)),
            _ => Vector2.zero
        };
    }

    private void Stop() => Stop(0, 0);
    
    private void Stop(int seconds, float exactTime)
    {
        IsSpawning = false;
        var enemies = _field.GetComponentsInChildren<Enemy>();
        foreach (var enemy in enemies) Destroy(enemy.gameObject);
        _enemiesPool.Clear();
        _enemiesInGame.Clear();
    }
}
