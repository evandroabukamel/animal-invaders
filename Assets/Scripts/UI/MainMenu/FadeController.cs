using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FadeController : MonoBehaviour
{
    [SerializeField] string[] gameOverMessages;
    [SerializeField] Text labelMessage;
    [SerializeField] PlayableDirector fadeOutDirector;
    [SerializeField] PlayableDirector fadeInDirector;
    [SerializeField] GameObject _victoryContainer;
    [SerializeField] GameObject _mainMenuButton;
    [SerializeField] Animator animator;
    
    Action _bossEntranceCallback;

    public void SetMessage(string textMessage)
    {
        labelMessage.text = textMessage;
    }

    public void SetRandomMessage()
    {
        labelMessage.text = gameOverMessages[Random.Range(0, gameOverMessages.Length)];
    }

    void ShowBossOverlay(bool show)
    { 
        _mainMenuButton.SetActive(!show);
        _victoryContainer.SetActive(!show);
    }
    
    public void FadeIn(bool showMessage)
    {
        labelMessage.gameObject.SetActive(showMessage);
        fadeOutDirector.Stop();
        fadeInDirector.Stop();
        fadeInDirector.Play();
    }

    public void FadeOut(bool showMessage)
    {
        labelMessage.gameObject.SetActive(showMessage);
        fadeOutDirector.Stop();
        fadeInDirector.Stop();
        fadeOutDirector.Play();
    }

    public void PlayBossEntrance(Action callback)
    {
        _bossEntranceCallback = callback;
        ShowBossOverlay(true);
        animator.Play("BossEntrance");
    }
    
    // Called by Animator
    public void ExecuteBossCallback()
    {
        _bossEntranceCallback?.Invoke();
        ShowBossOverlay(false);
    }
}
