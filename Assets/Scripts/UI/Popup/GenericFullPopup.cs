using UnityEngine;
using UnityEngine.UI;

public class GenericFullPopup : MonoBehaviour
{
    [SerializeField] private Text _text;

    public void SetMessage(string message)
    {
        _text.text = message;
    }
}
