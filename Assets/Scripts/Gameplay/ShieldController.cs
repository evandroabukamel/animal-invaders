using System;
using System.Collections;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Color shieldColor = new Color(61, 255, 85, 255);
    [SerializeField, Range(1f, 60f)] float rotateSpeed = 10f;
    [SerializeField] int shieldDurationSecs = 10;

    Coroutine _disableCoroutine;

    void OnValidate()
    {
        spriteRenderer.color = shieldColor;
    }

    void OnEnable()
    {
        StartCoroutine(Blink());
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime, Space.Self);
    }

    /// <summary>
    ///  Cancel previous deactivation and add more time to the shield.
    /// </summary>
    public void Activate()
    {
        CancelInvoke();
        if (_disableCoroutine != null)
        {
            StopAllCoroutines();
            _disableCoroutine = null;
        }
        StartCoroutine(Blink());
        
        // Renew disable time.
        Invoke(nameof(Disable), shieldDurationSecs);
    }

    public void Disable()
    {
        _disableCoroutine = StartCoroutine(Blink(() => gameObject.SetActive(false)));
    }
    
    IEnumerator Blink(Action callback = null)
    {
        for (var i = 0; i < 3; i++)
        {
            spriteRenderer.color = Color.clear;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = shieldColor;
            yield return new WaitForSeconds(0.2f);
        }
        
        callback?.Invoke();
    }
}
