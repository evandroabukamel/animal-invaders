using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] HealthController _playerHealthController;
    [SerializeField] Heart _heartPrefab;

    List<Heart> _hearts;

    bool _isSetup;

    public void OnEnable()
    {
        _playerHealthController.OnHitPointsChange += HandleOnHitPointsChange;
        _playerHealthController.OnMaxHitPointsChange += HandleOnMaxHitPointsChange;
    }

    void OnDestroy()
    {
        _playerHealthController.OnMaxHitPointsChange -= HandleOnHitPointsChange;
        _playerHealthController.OnMaxHitPointsChange -= HandleOnMaxHitPointsChange;
    }
    
    void Setup()
    {
        _isSetup = true;
        _hearts = new List<Heart>(_playerHealthController.MaxHitPoints);
        
        for (var i = 0; i < _playerHealthController.MaxHitPoints; i++)
        {
            _hearts.Add(Instantiate(_heartPrefab, transform));
        }
    }

    void HandleOnHitPointsChange(int currentHitPoints)
    {
        if (!_isSetup) Setup();
        
        for (var i = 0; i < _hearts.Count; i++)
        {
            if (i < currentHitPoints)
            {
                _hearts[i].SetFull();
                continue;
            }
            
            _hearts[i].SetEmpty();
        }
    }
    
    void HandleOnMaxHitPointsChange(int maxHitPoints)
    {
        var diff = maxHitPoints - _hearts.Count;
        if (diff <= 0) return;
        
        for (var i = 0; i < diff; i++)
        {
            _hearts.Add(Instantiate(_heartPrefab, transform));
        }
    }
}
