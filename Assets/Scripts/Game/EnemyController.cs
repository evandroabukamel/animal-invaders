using System.Collections;
using UnityEngine;
using Wildlife.Cheetah.ScriptableEvents;

public class EnemyController : MonoBehaviour
{
    public enum EnemyType { Static, Ramming, Moving }
    
    [Header("Health")] //
    [SerializeField] public HealthController _healthController;
    [SerializeField] int maxHitPoints;
    [SerializeField] ScriptableEvent OnEnemyKilledEvent;
    
    [Header("Components")] //
    [SerializeField] Rigidbody2D _rigidbody;
    [SerializeField] ParticleSystem _particleSystem;
    
    [Header("Attributes")] //
    public EnemyType enemyType;
    [SerializeField, Range(1f, 30f)] float moveSpeed = 1f;
    [SerializeField, Range(1f, 10f)]float walkingInterval = 3f;
    
    public bool HasReachedStartPoint => hasReachedStartPoint;
    
    Vector3 _initialDestination;
    bool hasReachedStartPoint;
    
    public bool HasCharged => _hasCharged;
    
    bool _isCharging;
    bool _hasCharged;
    float _chargeSpeed;
    Vector3 _chargePos;
    Vector2 _chargeDirection;
    
    Vector3 _bottomPos;
    Vector3 _upperPos;

    float _walkTime;
    float _safeOffset = 0.5f;

    readonly WaitForSeconds _chargeWaitTime = new WaitForSeconds(0.5f);

    public void Reset()
    {
        _chargeSpeed = 0f;
        _isCharging = false;
        _hasCharged = false;
        hasReachedStartPoint = false;
    }

    public void Setup(Vector3 firstMoveDestination)
    {
        if (enemyType == EnemyType.Moving)
        {
            var minX = MatchController.Instance.BossAreaTopLeftPos.x;
            var maxX = MatchController.Instance.PlayerAreaBottomRightPos.x;

            var clampedX = Mathf.Clamp(firstMoveDestination.x, minX + _safeOffset, maxX - _safeOffset);
            
            var clampedDestination = new Vector3(clampedX, firstMoveDestination.y, firstMoveDestination.z);
            _initialDestination = clampedDestination;
            return;
        }
        
        _initialDestination = firstMoveDestination;
    }

    public void ClearParticles()
    {
        if (_particleSystem == null) return; 
        _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _particleSystem.Play();
    }

    public void StopAndClearParticles()
    {
        if (_particleSystem == null) return; 
        _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void OnEnable()
    {
        Reset();
        _rigidbody.isKinematic = true;
        
        _healthController.SetupHealth(maxHitPoints, () => OnEnemyKilledEvent.Raise());
    }

    void SetupRigidbody()
    {
        if (enemyType != EnemyType.Moving)
        {
            _rigidbody.isKinematic = true;
            return;
        }

        _rigidbody.isKinematic = false;
    }

    void Start()
    {
        _rigidbody.isKinematic = true;
    }

    void Update()
    {
        if (!hasReachedStartPoint && Vector2.Distance(transform.position, _initialDestination) > 1f)
        {
            HandleInitialMove();
            return;
        }
        
        ExecuteAttackPattern();
    }

    void HandleInitialMove()
    { 
        transform.position = Vector2.MoveTowards(transform.position, _initialDestination, moveSpeed * Time.deltaTime);
        
        if (Vector2.Distance(transform.position, _initialDestination) <= 1f)
        {
            hasReachedStartPoint = true;
            SetupRigidbody();
        }
    }

    void ExecuteAttackPattern()
    {
        switch (enemyType)
        {
            case EnemyType.Ramming:
                StartChargeAttack();
                break;
            
            case EnemyType.Moving:
                RandomWalk();
                break;
            
            default:
                transform.Translate(Vector2.down * (moveSpeed * Time.deltaTime));
                break;
        }
    }
    
    void RandomWalk()
    {
        _walkTime += Time.deltaTime;

        if (_walkTime >= walkingInterval)
        {
            _upperPos = MatchController.Instance.BossAreaTopLeftPos;
            _bottomPos = MatchController.Instance.PlayerAreaBottomRightPos;
            var point = MathExtensions.GetRandomPointBetweenPoints(_upperPos, _bottomPos);
        
            // Calculate movement direction towards the new point
            _rigidbody.AddForce(point - transform.position, ForceMode2D.Impulse);
            
            _walkTime -= walkingInterval;
        }
    }

    void StartChargeAttack()
    {
        if (!_isCharging)
        {
            StartCoroutine(nameof(ChargeAfterTime));
        }
                
        _isCharging = true;
                
        if (_chargeSpeed > 0)
        {
            transform.Translate(_chargeDirection * (_chargeSpeed * Time.deltaTime));
                    
            if (Vector2.Distance(_chargePos, transform.position) < 1f)
            {
                _hasCharged = false;
            }
        }
    }
    
    IEnumerator ChargeAfterTime()
    {
        yield return _chargeWaitTime;
        
        _chargePos = MatchController.Instance.Player.transform.position;
        var direction = _chargePos - transform.position;
        var directionNorm = new Vector2(direction.x, direction.y).normalized;
        
        _chargeDirection = new Vector3(directionNorm.x, directionNorm.y, 0f);
        _hasCharged = true;
        _chargeSpeed = 3f;
    }

    void OnDisable()
    {
        Reset();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _healthController.OnDie();
            
            if (other.gameObject.TryGetComponent<PlayerController>(out var player))
            {
                if (player.IsShieldActive) return;
            }
            
            if (!other.gameObject.TryGetComponent<HealthController>(out var health)) { return; }

            health.Hit();
        }
    }
}
