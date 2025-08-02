using System;
using UnityEngine;
using Wildlife.Cheetah.ScriptableEvents;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")] //
    [SerializeField] Transform startPosition;
    [SerializeField] Rigidbody2D _rigidbody;
    [SerializeField] Vector2ScriptableEvent onStartMoveVector2;
    [SerializeField] Vector2ScriptableEvent onMoveVector2;
    [SerializeField] Vector2ScriptableEvent onStopMoveVector2;
    [SerializeField, Range(1f, 30f)] float moveSpeed = 3f;

    [Header("Shooting")] //
    [SerializeField] AudioSource shootingSound;
    [SerializeField] AudioClip shootingAudioClip;
    [SerializeField] float shotsPerSecond = 2f;
    [SerializeField] GunsController[] gunsAvailable;

    [Header("Health")] //
    [SerializeField] HealthController _healthController;
    [SerializeField] int maxHitPoints;
    [SerializeField] ScriptableEvent OnPlayerDiedEvent;
    [SerializeField] ScriptableEvent OnBossKilledEvent;
    
    [Header("Power ups")] //
    [SerializeField] AudioSource powerUpSound;
    [SerializeField] AudioClip powerUpAudioClip;

    [Header("Shield")] //
    [SerializeField] ShieldController shieldController;

    Vector2 _move;
    float _accumulatedTime;
    int _gunLevel;
    float _joystickDeadZone = 1f;

    public bool IsShieldActive => shieldController.gameObject.activeSelf;

    void Start()
    {
        ResetGuns();
        
        shieldController.gameObject.SetActive(false);

        if (startPosition != null)
        {
            transform.position = startPosition.position;
        }

        onStartMoveVector2.OnRaise += OnStartMove;
        onMoveVector2.OnRaise += OnMove;
        onStopMoveVector2.OnRaise += OnStopMove;
        OnBossKilledEvent.OnRaise += HandleBossKilled;
        
        _healthController.SetupHealth(maxHitPoints, () => OnPlayerDiedEvent.Raise());
    }

    void OnDestroy()
    {
        onStartMoveVector2.OnRaise -= OnStartMove;
        onMoveVector2.OnRaise -= OnMove;
        onStopMoveVector2.OnRaise -= OnStopMove;
        OnBossKilledEvent.OnRaise -= HandleBossKilled;
        
        StopAllCoroutines();
    }
    
    void OnStartMove(Vector2 move)
    {
        _move = move;
    }
    
    void OnMove(Vector2 move)
    {
        if (Math.Abs(_move.x) > _joystickDeadZone && Math.Abs(_move.y) > _joystickDeadZone && 
            Math.Abs(move.x) <= _joystickDeadZone && Math.Abs(move.y) <= _joystickDeadZone)
        {
            move = Vector2.zero;
        }
        _move = move;
    }

    void OnStopMove(Vector2 move)
    {
        _move = Vector2.zero;
    }

    void Update()
    {
        HandleMovement();
        HandleShoot();
    }

    void HandleMovement()
    {
        _rigidbody.velocity = _move * moveSpeed;
    }

    void ResetGuns()
    {
        _gunLevel = 0;
        gunsAvailable[0].gameObject.SetActive(true);
        for (var g = 1; g < gunsAvailable.Length; g++)
        {
            gunsAvailable[g].gameObject.SetActive(true);
        }
    }
    
    void HandleShoot()
    {
        _accumulatedTime += Time.deltaTime;

        var fireRate = 1f / shotsPerSecond;
        if (_accumulatedTime >= fireRate)
        {
            gunsAvailable[_gunLevel].Shoot();
            PlayShootSound();

            _accumulatedTime -= fireRate;
        }
    }
    
    void PlayShootSound()
    {
        if (shootingAudioClip == null) return;
        if (shootingSound.isPlaying) shootingSound.Stop();
        shootingSound.PlayOneShot(shootingAudioClip);
    }
    
    void PlayPowerUpSound()
    {
        if (powerUpAudioClip == null) return;
        powerUpSound.PlayOneShot(powerUpAudioClip);
    }

    void HandleBossKilled()
    {
        gameObject.SetActive(false);
    }

    void IncreaseGunLevel()
    {
        if (_gunLevel + 1 >= gunsAvailable.Length) return;
        
        if (gunsAvailable[_gunLevel + 1] != null)
        {
            gunsAvailable[_gunLevel].gameObject.SetActive(false);
            _gunLevel++;
            gunsAvailable[_gunLevel].gameObject.SetActive(true);
        }
    }

    void IncreaseShootingRate()
    {
        shotsPerSecond++;
    }

    void ActivateShield()
    {
        shieldController.gameObject.SetActive(true);
        shieldController.Activate();
    }

    public bool IsDead() => _healthController.IsDead();

    public void ReceivePowerUp(PowerUpController.PowerUpType powerUpType)
    {
        PlayPowerUpSound();
        
        switch (powerUpType)
        {
            case PowerUpController.PowerUpType.ShotRate:
                IncreaseShootingRate();
                break;
            
            case PowerUpController.PowerUpType.AddGun:
                IncreaseGunLevel();
                break;
            
            case PowerUpController.PowerUpType.RestoreOneHit:
                _healthController.RestoreHisPoints();
                break;
            
            case PowerUpController.PowerUpType.RestoreAllHits:
                _healthController.RestoreAllHitPoints();
                break;
                
            case PowerUpController.PowerUpType.GainOneHitPoint:
                _healthController.IncrementMaxHitPoints();
                break;
            
            case PowerUpController.PowerUpType.Shield:
                ActivateShield();
                break;

            case PowerUpController.PowerUpType.Nuke:
                MatchController.Instance.ClearEnemiesParticles();
                break;
        }
    }
}
