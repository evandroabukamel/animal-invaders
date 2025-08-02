using UnityEngine;
using Wildlife.Cheetah.Loader;
using Wildlife.Cheetah.ScriptableEvents;

public enum MainMenuOptions { None, MainMenu, Items, Config, Tutorial }

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject items;
    public GameObject config;
    public GameObject notEnoughCurrencyPopUp;
    public AsyncLoaderSystem asyncLoaderSystem;

    [SerializeField] ScriptableEvent notEnoughCurrencyEvent;
    [SerializeField] FadeController fadeController;
    [SerializeField] string gameScene;

    public void OnPlayClicked()
    {
        asyncLoaderSystem.LoadSceneAsync(gameScene, 2.0f);
    }

    public void FadeIn(bool showMessage = false, bool setMessage = false)
    {
        if (setMessage)
        {
            fadeController.SetRandomMessage();
        }

        fadeController.FadeIn(showMessage);
    }

    public void FadeOut(bool showMessage = false, bool setMessage = false)
    {
        if (setMessage)
        {
            fadeController.SetRandomMessage();
        }

        fadeController.FadeOut(showMessage);
    }

    void Awake() {

        if (notEnoughCurrencyEvent != null)
        {
            notEnoughCurrencyEvent.OnRaise += ShowNotEnoughCurrencyPopUp;
        }
    }

    void ShowNotEnoughCurrencyPopUp()
    {
        notEnoughCurrencyPopUp?.SetActive(true);
    }

    public void ShowMainMenu() {
        SetMenu(MainMenuOptions.MainMenu);
    }

    public void ShowSettingsMenu() {
        SetMenu(MainMenuOptions.Config);
    }

    public void ShowStoreMenu() {
        SetMenu(MainMenuOptions.Items);
    }

    public void SetMenu(MainMenuOptions m) {
        mainMenu.SetActive(m == MainMenuOptions.MainMenu);
        items.SetActive(m == MainMenuOptions.Items);
        config.SetActive(m == MainMenuOptions.Config);
    }
}
