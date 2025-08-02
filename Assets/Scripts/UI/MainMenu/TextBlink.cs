using UnityEngine;

public class TextBlink : MonoBehaviour
{
    bool _active;
    
    void Start()
    {
        InvokeRepeating(nameof(Switch), .5f, .5f);
    }

    void Switch()
    {
        _active = !_active;
        
        gameObject.SetActive(_active);
    }
}
