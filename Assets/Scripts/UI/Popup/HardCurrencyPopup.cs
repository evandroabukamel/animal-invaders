using System.Collections;
using TFG.Modules;
using UnityEngine;
using UnityEngine.UI;
using Wildlife.Cheetah.StoreSystem;

public class HardCurrencyPopup : MonoBehaviour
{
    [System.Serializable]
    public class HardCurrencyInfo
    {
        public HardCurrencyPack pack;
        public Text priceLabel;
        public Text quantityLabel;
    }

    [SerializeField] string itemName = "Coins";
    [SerializeField] HardCurrencyInfo[] packList;
    [SerializeField] FadeController purchaseOverlay;
    [SerializeField] StoreSystem storeSystem;

    void OnEnable()
    {
        StoreSystem.OnIapTransactionReceived = OnIapTransactionReceived;
        SetupText();

        if (!StoreSystem.IapAvailable)
        {
            StartCoroutine(UpdateButtonLabel());
        }
    }

    void OnIapTransactionReceived(Iap.Transaction transaction)
    {
        purchaseOverlay.SetMessage(transaction.Success ? "Purchase Completed" : "Purchase Failed");
        Invoke(nameof(HideOverlayAnimated), 2);
    }

    void HideOverlayAnimated()
    {
        purchaseOverlay.FadeIn(true);
        Invoke(nameof(DisableOverlay), 2);
    }

    void DisableOverlay()
    {
        purchaseOverlay.gameObject.SetActive(false);
    }

    IEnumerator UpdateButtonLabel()
    {
        while (!StoreSystem.IapAvailable)
        {
            yield return null;
        }

        SetupText();
    }

    void SetupText()
    {
        foreach (var item in packList)
        {
            if(item.pack == null)
            {
                item.priceLabel.text = "-";
                item.quantityLabel.text = "-";
            }
            else
            {
                item.priceLabel.text = item.pack.GetItemPrice(storeSystem);
                item.quantityLabel.text = "x" + item.pack.GetAmount() + " " + itemName;
            }

        }
    }

    public void Buy(int id)
    {
        purchaseOverlay.gameObject.SetActive(true);
        purchaseOverlay.SetMessage("Processing...");
        purchaseOverlay.FadeOut(true);

        if(id < 0 || id >= packList.Length || packList[id].pack == null)
        {
            return;
        }

        storeSystem.BuyItem(packList[id].pack);
    }
}
