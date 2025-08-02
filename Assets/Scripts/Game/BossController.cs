using System;
using System.Collections;
using UnityEngine;
using Wildlife.Cheetah.ScriptableEvents;

public class BossController : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rigidbody;
    [SerializeField] ParticleSystem _psRandom;
    [SerializeField] ParticleSystem _psRotatingBurst;
    
    [Header("Health")] //
    [SerializeField] HealthController _healthController;
    [SerializeField] int maxHitPoints;
    [SerializeField] ScriptableEvent OnBossKilledEvent;
    
    [Header("Boundaries")] //
    [SerializeField] RectTransform _upperLeft;
    [SerializeField] RectTransform _bottomRight;

    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _frenzyAudioClip;
    
    Vector3 _bottomPos;
    Vector3 _upperPos;

    bool _isRandomWalkAllowed;
    bool _isPlayingPatternLevel1;
    bool _isPlayingPatternLevel2;
    bool _isMovementAllowed;

    float MAX_POS_DIFF = 0.01f;

    readonly Vector3 _startingPoint = new Vector3(0f, 3f, 1f);

    WaitForSeconds _pauseWaitTime = new WaitForSeconds(3f);
    WaitForSeconds _randomPatternPlayTime = new WaitForSeconds(6f);
    WaitForSeconds _rotatingPatternPlayTime = new WaitForSeconds(12f);
    
    Action OnCenterReached;

    public HealthController Health;

    public void Setup(Vector3 upperPosLimit, Vector3 bottomPosLimit)
    {
        _upperPos = upperPosLimit;
        _bottomPos = bottomPosLimit;
    }
    
    void OnEnable()
    {
        _healthController.SetupHealth(maxHitPoints, () => OnBossKilledEvent.Raise());
        
        _isMovementAllowed = true;
    }
    
    void Start()
    {
        if (_bottomRight != null && _upperLeft != null)
        {
            _upperPos = _upperLeft.TransformPoint(_upperLeft.rect.center);
            _bottomPos = _bottomRight.TransformPoint(_bottomRight.rect.center);
        }

        InvokeRepeating(nameof(UpdateAttackPattern), 1f, 1f);
    }

    public void ClearParticles()
    {
        if (_psRandom != null && _psRandom.isPlaying)
        {
            _psRandom.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _psRandom.Play();
        }
        
        if (_psRotatingBurst != null && _psRotatingBurst.isPlaying)
        {
            _psRotatingBurst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _psRotatingBurst.Play();
        }
    }
    
    public void StopAndClearParticles()
    {
        if (_psRandom != null)
        {
            _psRandom.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        
        if (_psRotatingBurst != null)
        {
            _psRotatingBurst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
    
    void RandomWalk()
    {
        var point = MathExtensions.GetRandomPointBetweenPoints(_upperPos, _bottomPos);
        
        // Calculate movement direction towards the new point
        _rigidbody.AddForce(point - transform.position, ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {
        if (!_isMovementAllowed) return;
        
        if (_psRotatingBurst.isPlaying)
        {
            var currentZ = _psRotatingBurst.transform.localEulerAngles.z;
            _psRotatingBurst.transform.rotation = Quaternion.Euler(0f, 0f, currentZ + 5f);
        }
        
        if (IsRandomWalkAllowed())
        {
            RandomWalk();
            return;
        }
        
        GoToCenter();
    }
    
    void GoToCenter()
    {
        var direction = _startingPoint - transform.position;
        
        if (direction.magnitude <= MAX_POS_DIFF)
        {
            transform.position = _startingPoint;
            
            OnCenterReached?.Invoke();
            OnCenterReached = null;
            
            return;
        }
        
        transform.Translate(direction * (Time.fixedDeltaTime * 2f));
    }
    
    bool IsRandomWalkAllowed()
    {
        return _isRandomWalkAllowed;
    }

    void OnDisable()
    {
        OnCenterReached = null;
        StopAllCoroutines();
        CancelInvoke();
        ResetBoss(true);
    }
    
    void UpdateAttackPattern() 
    {
        if (_isPlayingPatternLevel2) { return; }
        
        if (_healthController.CurrentHitPoints > _healthController.MaxHitPoints / 2)
        {
            if (_isPlayingPatternLevel1) { return; }
            
            _isPlayingPatternLevel1 = true;
            
            StartCoroutine(nameof(PlayLevel1Patterns));
            return;
        }
        
        _isPlayingPatternLevel2 = true;
        _isMovementAllowed = false;
        
        ResetBoss(true);
        StartCoroutine(nameof(PlayLevel2Patterns));
    }
    
    void ResetBoss(bool shouldStopAllCoroutines = false)
    { 
        _rigidbody.velocity = Vector2.zero;
        
        if (shouldStopAllCoroutines) StopAllCoroutines();
        
        _psRandom.Stop();
        _psRotatingBurst.Stop();
    }
    
    IEnumerator PlayLevel1Patterns()
    {
        while (true)
        {
            ResetBoss();
            SetupRandomPattern();
            yield return _randomPatternPlayTime;
            
            ResetBoss();
            SetupRotatingBurstPattern();
            yield return _rotatingPatternPlayTime;
        }
    }
    
    IEnumerator PlayLevel2Patterns()
    {
        _audioSource.PlayOneShot(_frenzyAudioClip);
        yield return _pauseWaitTime;
        
        _isMovementAllowed = true;
        
        while (true)
        {
            var psRandomMain = _psRandom.main;
            var psRandomEmission = _psRandom.emission;
            
            psRandomMain.startSpeedMultiplier = 6f;
            psRandomEmission.rateOverTimeMultiplier = 11f;

            SetupRandomPattern();
            yield return _randomPatternPlayTime;
            
            ResetBoss();
            
            var psRotatingBurstMain = _psRotatingBurst.main;
            psRotatingBurstMain.startSpeedMultiplier = 7f;
            
            SetupRotatingBurstPattern();
            yield return _rotatingPatternPlayTime;
            
            _audioSource.PlayOneShot(_frenzyAudioClip);
            ResetBoss();
        }
    }
    
    void SetupRandomPattern()
    {
        _isRandomWalkAllowed = true;
        _psRandom.Play();
    }
    
    void SetupRotatingBurstPattern()
    {
        _isRandomWalkAllowed = false;
        
        OnCenterReached = () =>
        {
            if (!_psRotatingBurst.isPlaying)
            {
                _psRotatingBurst.Play();
            }
        };
    }
}
