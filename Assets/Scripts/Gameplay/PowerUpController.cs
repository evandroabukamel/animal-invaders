using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public enum PowerUpType
    {
        ShotRate,
        AddGun,
        Shield,
        RestoreOneHit,
        RestoreAllHits,
        Nuke,
        GainOneHitPoint,
    }

    [SerializeField] PowerUpType powerUpType;
    [SerializeField] Sprite sprite;
    [SerializeField] Color color;
    [SerializeField] SpriteRenderer spriteRenderer;

    int _moveSpeed;

    public bool HasReachedStartPoint => transform.position.y < MatchController.Instance.BossAreaBottomRightPos.y;

    void OnValidate()
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;
    }

    void Awake()
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;
        _moveSpeed = Random.Range(1, 5);
    }

    void Update()
    {
        transform.Translate(Vector2.down * _moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponentInParent<PlayerController>().ReceivePowerUp(powerUpType);
            Destroy(gameObject);
        }
    }
}
