using System;
using System.Collections;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _explosionAudioClip;
    [SerializeField] AudioClip _hitAudioClip;
    [SerializeField] SpriteRenderer _actorRenderer;
    [SerializeField] Color _damageFlashColor = Color.red;
    
    public Action<int> OnHitPointsChange;
    public Action<int> OnMaxHitPointsChange;
    
    public int MaxHitPoints { get; private set; }
    public int CurrentHitPoints { get; private set; }

    Color _originalRendererColor;
    Action _onDieCallback;

    void Awake()
    {
        _originalRendererColor = _actorRenderer.color;
    }

    void OnEnable()
    {
        _actorRenderer.color = _originalRendererColor;
    }

    public void SetupHealth(int maxHitPoints, Action onDieCallback = null)
    {
        MaxHitPoints = maxHitPoints;
        CurrentHitPoints = MaxHitPoints;
        _onDieCallback = onDieCallback;
        
        OnHitPointsChange?.Invoke(CurrentHitPoints);
        OnMaxHitPointsChange?.Invoke(MaxHitPoints);
    }
    
    public void Hit()
    {
        if (IsDead()) { return; }
        
        CurrentHitPoints--;
        OnHitPointsChange?.Invoke(CurrentHitPoints);
        
        if (_hitAudioClip != null)
        {
            _audioSource.PlayOneShot(_hitAudioClip);
        }

        if (_actorRenderer != null)
        {
            StartCoroutine(FlashDamage());
        }
        
        if (CurrentHitPoints <= 0)
        {
            OnDie();
        }
    }

    IEnumerator FlashDamage()
    {
        _actorRenderer.color = _damageFlashColor;
        yield return new WaitForSeconds(0.15f);
        _actorRenderer.color = _originalRendererColor;
    }

    public bool IsDead()
    {
        return CurrentHitPoints <= 0;
    }

    public void OnDie()
    {
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(_explosionAudioClip, MatchController.Instance.MainCamera.transform.position);
        
        _onDieCallback?.Invoke();
        gameObject.SetActive(false);
    }

    public void IncrementMaxHitPoints(int increment = 1)
    {
        MaxHitPoints += increment;
        CurrentHitPoints = MaxHitPoints;
        OnMaxHitPointsChange?.Invoke(MaxHitPoints);
        OnHitPointsChange?.Invoke(CurrentHitPoints);
    }

    public void RestoreHisPoints(int restore = 1)
    {
        if (CurrentHitPoints >= MaxHitPoints) return;

        var diff = Math.Abs(MaxHitPoints - (CurrentHitPoints + restore));
        CurrentHitPoints += Math.Max(restore, diff);
        OnHitPointsChange?.Invoke(CurrentHitPoints);
    }

    public void RestoreAllHitPoints()
    {
        CurrentHitPoints = MaxHitPoints;
        OnHitPointsChange?.Invoke(CurrentHitPoints);
    }
}
