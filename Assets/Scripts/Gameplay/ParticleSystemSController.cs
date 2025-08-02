using UnityEngine;

public class ParticleSystemSController : MonoBehaviour
{
    void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            if (player.IsShieldActive) return;
        }
        
        if (!other.TryGetComponent<HealthController>(out var health)) { return; }

        health.Hit();
    }
}
