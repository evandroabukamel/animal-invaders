using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wildlife.Cheetah.Loader;
using Wildlife.Cheetah.ScriptableEvents;

public class GameplayViewController : MonoBehaviour
{
    [Header("UI")] // 
    public AsyncLoaderSystem asyncLoaderSystem;
    [SerializeField] string mainMenuScene = "MainMenu";
    [SerializeField] FadeController blackOverlay;
    [SerializeField] Canvas joystickCanvas;
    [SerializeField] RectTransform victoryView;
    [SerializeField] Text enemiesKilled;
    
    [Header("Events")] //
    [SerializeField] ScriptableEvent OnPlayerDiedEvent;
    [SerializeField] ScriptableEvent OnBossKilledEvent;

    void OnEnable()
    {
        OnPlayerDiedEvent.OnRaise += OnPlayerDied;
        OnBossKilledEvent.OnRaise += OnPlayerVictory;
    }

    void OnDestroy()
    {
        OnPlayerDiedEvent.OnRaise -= OnPlayerDied;
        OnBossKilledEvent.OnRaise -= OnPlayerVictory;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        asyncLoaderSystem.LoadSceneAsync(mainMenuScene, 2.0f);
    }

    void HandleMatchEnd()
    {
        joystickCanvas.gameObject.SetActive(false);
    }

    void OnPlayerDied()
    {
        HandleMatchEnd();
        FadeOut(true, true, false);
    }

    void OnPlayerVictory()
    {
        HandleMatchEnd();
        enemiesKilled.text = MatchController.Instance.EnemiesKilled.ToString();
        FadeOut(false, false, true);
    }
    
    public void FadeOut(bool showMessage = false, bool setMessage = false, bool showVictory = false)
    {
        if (setMessage)
        {
            blackOverlay.SetRandomMessage();
        }

        victoryView.gameObject.SetActive(showVictory);
        
        blackOverlay.FadeOut(showMessage);
    }

    public void BossEntrance(Action callback)
    {
        blackOverlay.PlayBossEntrance(callback);
    }
}
