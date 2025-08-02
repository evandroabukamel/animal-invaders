using System.Collections.Generic;
using UnityEngine;
using Wildlife.Cheetah.ScriptableEvents;

public class MatchController : MonoBehaviour
{
    [Header("Camera")] //
    [SerializeField] public Camera MainCamera;
    
    [Header("Player")] //
    [SerializeField] PlayerController player;
    
    [Header("Enemies")] //
    [SerializeField] BossController spawnedBoss;
    [SerializeField] List<EnemyController> enemiesPrefabs;
    [SerializeField] List<EnemyController> spawnedEnemies;
    [SerializeField] ScriptableEvent OnEnemyKilledEvent;
    [SerializeField] ScriptableEvent OnBossKilledEvent;
    [SerializeField] ScriptableEvent OnPlayerDiedEvent;

    [Header("Key positions")] //
    [SerializeField] RectTransform enemySpawnTopLeft;
    [SerializeField] RectTransform enemySpawnBottomRight;
    [SerializeField] RectTransform bossAreaTopLeft;
    [SerializeField] RectTransform bossAreaBottomRight;
    [SerializeField] RectTransform playerAreaBottomRight;

    [Header("Rules")] //
    [SerializeField, Range(1, 5)] int difficultyLevel = 1;
    [SerializeField] RuleSet prodRuleSet;
    [SerializeField] RuleSet stagRuleSet;
    [SerializeField] List<PowerUpController> powerUpsPrefabs;

    [Header("UI")] // 
    [SerializeField] GameplayViewController gameplayView;

    RuleSet _ruleSet;
    
    float _matchStartTime;
    int _nextUpdate = 1;
    float _enemySpawnInterval;
    float _accumulatedTime;
    const int SecondUpdate = 1;
    bool _bossAlreadySpawned;
    bool _bossKilled;
    bool _bossDroppedPowerUp;
    int _enemiesKilled;
    int _enemiesKilledSinceLastPowerUpDrop;
    
    Vector2 enemySpawnTopLeftPos;
    Vector2 enemySpawnBottomRightPos;
    Vector2 bossAreaTopLeftPos;
    Vector2 bossAreaBottomRightPos;
    Vector2 playerAreaBottomRightPos;

    static MatchController _instance;
    public static MatchController Instance => _instance;

    public PlayerController Player => player;
    public BossController Boss => spawnedBoss;
    public List<EnemyController> Enemies => spawnedEnemies;
    public int EnemiesKilled => _enemiesKilled;
    
    public Vector2 BossAreaBottomRightPos => bossAreaBottomRightPos;
    public Vector2 BossAreaTopLeftPos => bossAreaTopLeftPos;
    public Vector2 PlayerAreaBottomRightPos => playerAreaBottomRightPos;
    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        
#if UNITY_EDITOR
        _ruleSet = stagRuleSet;
#else
        _ruleSet = prodRuleSet;
#endif
    }

    void Start()
    {
        enemySpawnTopLeftPos = enemySpawnTopLeft.TransformPoint(enemySpawnTopLeft.rect.center);
        enemySpawnBottomRightPos = enemySpawnBottomRight.TransformPoint(enemySpawnBottomRight.rect.center);
        bossAreaTopLeftPos = bossAreaTopLeft.TransformPoint(bossAreaTopLeft.rect.center);
        bossAreaBottomRightPos = bossAreaBottomRight.TransformPoint(bossAreaBottomRight.rect.center);
        playerAreaBottomRightPos = playerAreaBottomRight.TransformPoint(playerAreaBottomRight.rect.center);
        
        _matchStartTime = Time.time;
        _enemySpawnInterval = _ruleSet.SpawnIntervalByLevel[difficultyLevel-1];

        _enemiesKilled = 0;
        OnEnemyKilledEvent.OnRaise += HandleEnemyKilled;
        OnBossKilledEvent.OnRaise += HandleBossKilled;
        OnPlayerDiedEvent.OnRaise += HandlePlayerDied;
    }

    void FixedUpdate()
    {
        if (_bossKilled || player.IsDead()) return;

        UpdatePerSecond();
        
        // Stop spawning enemies when the boss is active
        if (!_bossAlreadySpawned)
        {
            SpawnEnemies();
        }
        
        // Drop power up by enemies killed
        if (_enemiesKilledSinceLastPowerUpDrop >= _ruleSet.EnemiesKilledByLevelToReleasePowerUp[difficultyLevel - 1])
        {
            SpawnPowerUp();
            _enemiesKilledSinceLastPowerUpDrop = 0;
        }
    }

    void UpdatePerSecond()
    {
        if (Time.time < _nextUpdate) return;
        _nextUpdate = Mathf.FloorToInt(Time.time) + SecondUpdate;
        
        UpdateDifficultyLevel();
        SpawnPowerUpByBossHealth();
    }

    void OnDestroy()
    {
        OnEnemyKilledEvent.OnRaise -= HandleEnemyKilled;
        OnBossKilledEvent.OnRaise -= HandleBossKilled;
        OnPlayerDiedEvent.OnRaise -= HandlePlayerDied;
        StopAllCoroutines();
    }

    Vector3 GetSpawnPoint()
    {
        return MathExtensions.GetRandomPointBetweenPoints(enemySpawnTopLeftPos, enemySpawnBottomRightPos) + Vector3.forward;
    }

    void SpawnEnemies()
    {
        _accumulatedTime += Time.fixedDeltaTime;
        var spawnRate = _enemySpawnInterval;
        if (_accumulatedTime < spawnRate) return;
    
        var randomEnemy = enemiesPrefabs[Random.Range(0, enemiesPrefabs.Count)];
        EnemyController enemyObj = null;
        
        // Recycle spawned enemies
        for (var e = 0; e < spawnedEnemies.Count; e++)
        {
            if (spawnedEnemies[e].gameObject.activeSelf) continue;
            if (spawnedEnemies[e].enemyType != randomEnemy.enemyType) continue;

            enemyObj = spawnedEnemies[e];
        }

        // Spawn a new enemy
        if (enemyObj == null)
        {
            enemyObj = Instantiate(randomEnemy);
            spawnedEnemies.Add(enemyObj);
        }

        Transform enemyTransform;
        (enemyTransform = enemyObj.transform).position = GetSpawnPoint();
        enemyObj.Setup(new Vector3(enemyTransform.position.x, bossAreaBottomRightPos.y, 1f));
        enemyObj.gameObject.SetActive(true);
        
        _accumulatedTime -= spawnRate;
    }

    void SpawnPowerUp()
    {
        var index = Random.Range(0, powerUpsPrefabs.Count);
        var powerUpObj = Instantiate(powerUpsPrefabs[index]);
        powerUpObj.transform.position = GetSpawnPoint();
    }

    void SpawnPowerUpByBossHealth()
    {
        if (!Boss.gameObject.activeSelf || _bossDroppedPowerUp || _bossKilled) return;

        if (Boss.Health.CurrentHitPoints <= Boss.Health.MaxHitPoints * 0.5f)
        {
            SpawnPowerUp();
            _bossDroppedPowerUp = true;
        }
    }

    void SpawnBoss()
    {
        _bossAlreadySpawned = true;

        spawnedBoss.Setup(bossAreaTopLeftPos, bossAreaBottomRightPos);

        foreach (var enemy in spawnedEnemies)
        {
            enemy.gameObject.SetActive(false);
        }
        
        gameplayView.BossEntrance(() => spawnedBoss.gameObject.SetActive(true));
    } 
    
    void UpdateDifficultyLevel()
    {
        if (_bossAlreadySpawned) return;
        
        // Spawn boss if reached time
        if (Time.time - _matchStartTime >= _ruleSet.TimeForBossAppearance)
        {
            SpawnBoss();
        }

        // Change difficulty level and enemies spawn interval
        if (difficultyLevel < _ruleSet.SpawnIntervalByLevel.Count)
        {
            if (Time.time - _matchStartTime >= (difficultyLevel+1) * _ruleSet.LevelDuration)
            {
                difficultyLevel++;
                _enemySpawnInterval = _ruleSet.SpawnIntervalByLevel[difficultyLevel-1];  
            }
        }
    }

    void HandleEnemyKilled()
    {
        _enemiesKilled++;
        _enemiesKilledSinceLastPowerUpDrop++;
    }

    void HandleBossKilled() => _bossKilled = true;

    void HandlePlayerDied()
    {
        StopAndClearEnemiesParticles();
        Invoke(nameof(DisableEnemies), 2f);
    }

    void DisableEnemies()
    {
        if (Boss.gameObject.activeSelf)
        {
            Boss.gameObject.SetActive(false);
        }
        
        for (var e = 0; e < Enemies.Count; e++)
        {
            if (!Enemies[e].gameObject.activeSelf) continue;

            Enemies[e].gameObject.SetActive(false);
        }
    }

    public void ClearEnemiesParticles()
    {
        if (Boss.gameObject.activeSelf)
        {
            Boss.ClearParticles();
        }
        
        for (var e = 0; e < Enemies.Count; e++)
        {
            if (!Enemies[e].gameObject.activeSelf) continue;

            Enemies[e].ClearParticles();
        }
    }
    
    public void StopAndClearEnemiesParticles()
    {
        if (Boss.gameObject.activeSelf)
        {
            Boss.StopAndClearParticles();
        }
        
        for (var e = 0; e < Enemies.Count; e++)
        {
            if (!Enemies[e].gameObject.activeSelf) continue;

            Enemies[e].StopAndClearParticles();
        }
    }
}
